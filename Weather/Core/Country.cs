using System.Collections.Generic;

namespace Weather.Core
{
    /// <summary> Страна </summary>
    public class Country
    {
        /// <summary> Наименование </summary>
        public string Name { get; set; }

        /// <summary> Адрес </summary>
        public string Url { get; set; }

        /// <summary> Регионы </summary>
        public List<Region> Regions { get; set; } = new List<Region>();

        public override string ToString() => $"{Name} ({Url})";
    }
}
