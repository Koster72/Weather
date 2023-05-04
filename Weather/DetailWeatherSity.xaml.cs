using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Weather.Annotations;
using Weather.Core;

namespace Weather
{
    public partial class DetailWeatherSity : UserControl, INotifyPropertyChanged
    {
        private Country country = null;
        private Region region = null;
        private Sity sity = null;

        public Region Region
        {
            get => region;
            set
            {
                if (Equals(value, region)) return;
                region = value;
                OnPropertyChanged();
            }
        }

        public Country Country
        {
            get => country;
            set
            {
                if (Equals(value, country)) return;
                country = value;
                OnPropertyChanged();
            }
        }

        public DayWeather Day
        {
            get => _day;
            set
            {
                if (Equals(value, _day)) return;
                _day = value;
                OnPropertyChanged();
            }
        }
        private DayWeather _day;

        public Image Back
        {
            get => _back;
            set
            {
                if (Equals(value, _back)) return;
                _back = value;
                OnPropertyChanged();
            }
        }

        public HourWeather Current
        {
            get => _current;
            set
            {
                if (Equals(value, _current)) return;
                _current = value;
                OnPropertyChanged();
            }
        }
        private HourWeather _current;
        private Image _back;

        public DetailWeatherSity()
        {
            DataContext = this;
            InitializeComponent();
            Directory.CreateDirectory("img");
        }

        public async Task Set(Country c, Region r, Sity s)
        {
            country = c;
            region = r;
            sity = s;
            Day = await MainWindow.Instance.Api.Today(s);
            Current = Day?.Current;
            tabControl.SelectedIndex = 0;
            string file = $"{AppDomain.CurrentDomain.BaseDirectory}img\\{Current?.State}.jpg";
            Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}img\\");
            if (!File.Exists(file) && !string.IsNullOrEmpty(Current?.BackgroundUrl))
            {
                var httpClient = new HttpClient();
                var buffer = await httpClient.GetByteArrayAsync(Current?.BackgroundUrl);
                File.WriteAllBytes(file, buffer);
            }

            if (File.Exists(file))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(file);
                bitmap.EndInit();
                backimg.ImageSource = bitmap;
            }
        }

        private async Task Set10()
        {
            if (sity == null) return;
            Day = await MainWindow.Instance.Api.Day10(sity);
            var d = new[] { w1, w2, w3, w4, w5 };
            int i = 0;
            foreach (Wheather10Day wheather10Day in d)
                await wheather10Day.Set(Day.Hours[i++]);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == 1) Set10();
        }
    }
}
