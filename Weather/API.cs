using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Weather.Core;

namespace Weather
{
    public class API
    {
        public static StringEventHandler Messages;
        public static ExceptionEventHandler Exceptions;

        public async Task<List<Country>> Catalog()
        {
            List<Country> ret = new List<Country>();
            using (var c = new HttpClient())
            {
                var htmlDoc = new HtmlDocument();
                try
                {
                    var html = await c.GetStringAsync("https://www.gismeteo.ru/catalog/");
                    htmlDoc.LoadHtml(html);
                }
                catch (Exception e)
                {
                    Exceptions?.Invoke(this, e);
                    return ret;
                }

                var tmp = htmlDoc.DocumentNode.SelectNodes("//a[@class='link-item']");
                ret.AddRange(tmp.Select(node => new Country
                    { Name = node.InnerText, Url = node.Attributes["href"].Value }));
            }

            return ret;
        }

        public async Task<List<Region>> Regiones(Country country)
        {
            List<Region> ret = new List<Region>();
            using (var c = new HttpClient())
            {
                var htmlDoc = new HtmlDocument();
                try
                {
                    var html = await c.GetStringAsync($"https://www.gismeteo.ru/{country.Url}");
                    htmlDoc.LoadHtml(html);
                }
                catch (Exception e)
                {
                    Exceptions?.Invoke(this, e);
                    return ret;
                }

                var reg = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='catalog-subtitle']");
                var subtitle = reg.InnerText;
                if (subtitle == "Все пункты")
                    ret = new List<Region> { new Region { Name = "Все пункты", Url = country.Url } };
                else
                {
                    reg = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='catalog-list']");
                    List<HtmlNode> nodes = new List<HtmlNode>();
                    foreach (HtmlNode div in reg.ChildNodes)
                    {
                        foreach (HtmlNode ddiv in div.ChildNodes)
                        foreach (HtmlNode dddiv in ddiv.ChildNodes)
                        {
                            if (dddiv.Name == "a")
                                nodes.Add(dddiv);
                        }
                    }

                    ret.AddRange(nodes.Select(node => new Region
                        { Name = node.InnerText, Url = node.Attributes["href"].Value }));
                }
            }

            return ret;
        }

        public async Task<List<Sity>> Sitys(Region reg)
        {
            List<Sity> ret = new List<Sity>();
            using (var c = new HttpClient())
            {
                var htmlDoc = new HtmlDocument();
                try
                {
                    var html = await c.GetStringAsync($"https://www.gismeteo.ru/{reg.Url}");
                    htmlDoc.LoadHtml(html);
                }
                catch (Exception e)
                {
                    Exceptions?.Invoke(this, e);
                    return ret;
                }

                var tmp = htmlDoc.DocumentNode.SelectNodes("//a[@class='link-item']");
                ret.AddRange(
                    tmp.Select(node => new Sity { Name = node.InnerText, Url = node.Attributes["href"].Value }));
            }

            return ret;
        }

        public async Task<DayWeather> Today(Sity sity)
        {
            DayWeather ret = new DayWeather();

            using (var c = new HttpClient())
            {
                var htmlDoc = new HtmlDocument();
                try
                {
                    var html = await c.GetStringAsync($"https://www.gismeteo.ru/{sity.Url}/now");
                    htmlDoc.LoadHtml(html);
                }
                catch (Exception e)
                {
                    Exceptions?.Invoke(this, e);
                    return ret;
                }
                ret.NameSity = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='page-title']")?.InnerText?.Replace("&#32;", " ");
                ret.Current = new HourWeather();
                var curr = htmlDoc.DocumentNode.SelectNodes("//a[@class='weathertab weathertab-link tooltip']")
                    .ToArray();
                if (curr.Length > 0)
                {
                    ret.Date = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='now-localdate']").InnerText;
                    ret.Current.Temp = curr[0].SelectSingleNode("//span[@class='unit unit_temperature_c']").InnerText;
                    ret.Current.Wind = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='unit unit_wind_m_s']").InnerHtml
                        .Replace(@"<div class=""item-measure""><div>"," ")
                        .Replace("<div>", " ").Replace("</div>", " ");
                    ret.Current.Pressure = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='unit unit_pressure_mm_hg_atm']").InnerHtml
                        .Replace(@"<div class=""item-measure""><div>", " ")
                        .Replace("<div>", " ").Replace("</div>", " ");
                    ret.Current.Humidity = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='now-info-item humidity']").InnerHtml
                        .Replace(@"<div class=""now-info-item humidity""><div class=""item-title"">Влажность</div><div class=""item-value"">", " ");
                    ret.Current.Humidity = ret.Current.Humidity.Substring(0,ret.Current.Humidity.IndexOf("<", StringComparison.Ordinal)) + " %";
                    ret.Current.State = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='now-desc']").InnerText;
                    string url = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'now d')]")?.Attributes["style"].Value;
                    if (!string.IsNullOrEmpty(url))
                    {
                        url = url?.Substring(url.IndexOf("'", StringComparison.Ordinal)+1);
                        ret.Current.BackgroundUrl = url.Substring(0,url.IndexOf("'", StringComparison.Ordinal));
                    }
                }
            }
            return ret;
        }

        public async Task<DayWeather> Day10(Sity sity)
        {
            DayWeather ret = new DayWeather();

            using (var c = new HttpClient())
            {
                var htmlDoc = new HtmlDocument();
                try
                {
                    var html = await c.GetStringAsync($"https://www.gismeteo.ru/{sity.Url}/10-days/");
                    htmlDoc.LoadHtml(html);
                }
                catch (Exception e)
                {
                    Exceptions?.Invoke(this, e);
                    return ret;
                }
                ret.NameSity = htmlDoc.DocumentNode
                    .SelectSingleNode("//div[@class='page-title']")?
                    .InnerText?.Replace("&#32;", " ")
                    .Replace("10","5");
                var dates = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='widget-row widget-row-days-date']")?
                     .ChildNodes.Select(d => d.InnerText).Select(s=>s.Insert(2,"\r\n")).ToArray();
                var states = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='widget-row widget-row-icon']")?
                    .ChildNodes.Select(d => d.ChildNodes[0]?.Attributes["data-text"].Value).ToArray();
                var temps = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='values']")?
                    .ChildNodes.Select(d => d.ChildNodes[0]?.ChildNodes[0].InnerText +"\r\n"+ d.ChildNodes[1]?.ChildNodes[0].InnerText).ToArray();
                var wilds = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='widget-row widget-row-wind-gust row-with-caption']")?
                    .ChildNodes.ToList().Skip(1).Select(d => d.ChildNodes[1]?.InnerText).ToArray();
                var humiditys = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-row='precipitation-bars']")?
                    .ChildNodes.ToList().Skip(1).Select(d => d.ChildNodes[0]?.InnerText).ToArray();
                ret.Hours = new HourWeather[10];
                for (int i = 0; i < ret.Hours.Length; i++)
                {
                    try
                    {
                        ret.Hours[i] = new HourWeather
                            { Time = dates[i], State = states[i], Temp = temps[i], Wind = wilds[i], Humidity = humiditys[i]};
                    }
                    catch (Exception e)
                    {
                        
                    }
                }
                ret.Hours = ret.Hours.Where(d => d != null).ToArray();
            }
            return ret;
        }
    }
}