using System.Collections.Generic;

namespace Weather.Core
{
    /// <summary> Регион </summary>
    public class Region
    {
        /// <summary> Наименование </summary>
        public string Name { get; set; }

        /// <summary> Адрес </summary>
        public string Url { get; set; }

        /// <summary> Города региона </summary>
        public List<Sity> Sitys { get; set; } = new List<Sity>();

        public override string ToString() => $"{Name} ({Url})";
    }
}
