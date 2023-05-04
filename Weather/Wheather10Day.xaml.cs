using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using Weather.Annotations;
using Weather.Core;

namespace Weather
{
    public partial class Wheather10Day : UserControl, INotifyPropertyChanged
    {
        public HourWeather Values = null;
        private string _date,_status;
        private string _temp;
        private string _wind;
        private string _humidity;

        public string Date {
            get => _date;
            set
            {
                if (value == _date) return;
                _date = value;
                OnPropertyChanged();
            }
        }

        public string Status {
            get => _status;
            set
            {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged();
            }
        }

        public string Temp {
            get => _temp;
            set
            {
                if (value == _temp) return;
                _temp = value;
                OnPropertyChanged();
            }
        }

        public string Wind {
            get => _wind;
            set
            {
                if (value == _wind) return;
                _wind = value;
                OnPropertyChanged();
            }
        }

        public string Humidity {
            get => _humidity;
            set
            {
                if (value == _humidity) return;
                _humidity = value;
                OnPropertyChanged();
            }
        }

        public Wheather10Day()
        {
            DataContext = this;
            InitializeComponent();
        }

        public async Task Set(HourWeather item)
        {
            await Task.Delay(10);
            Values = item;
            Date = item.Time;
            Status = item.State;
            Temp = item.Temp;
            Wind = item.Wind;
            Humidity = item.Humidity;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
