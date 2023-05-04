
using System.Windows.Media;

namespace Weather.Core
{
    /// <summary> Прогноз погоды на день </summary>
    public class DayWeather
    {
        /// <summary> Имя населенного пункта </summary>
        public string NameSity { get; set; }

        /// <summary> Дата прогноза </summary>
        public string Date { get; set; }

        public HourWeather Current { get; set; }

        public HourWeather[] Hours { get; set; }
    }

    /// <summary> Прогноз погоды на определенный час </summary>
    public class HourWeather
    {
        /// <summary> Время </summary>
        public string Time { get; set; }

        /// <summary> Темпиратура </summary>
        public string Temp { get; set; }

        /// <summary> Статус погоды </summary>
        public string State { get; set; }
        
        /// <summary> Ветер </summary>
        public string Wind { get; set; }

        /// <summary> давление </summary>
        public string Pressure { get; set; }

        /// <summary> влажность </summary>
        public string Humidity { get; set; }

        public string BackgroundUrl { get; set; }
    }
}
