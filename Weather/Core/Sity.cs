namespace Weather.Core
{
    /// <summary> Город / Населенный пункт </summary>
    public class Sity
    {
        /// <summary> Наименование </summary>
        public string Name { get; set; }

        /// <summary> Адрес </summary>
        public string Url { get; set; }

        public override string ToString() => $"{Name} ({Url})";
    }
}
