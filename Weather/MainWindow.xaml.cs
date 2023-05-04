using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Weather.Annotations;
using Weather.Core;
using Weather.Properties;

namespace Weather
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static MainWindow Instance = null;
        public API Api = null;


        public MainWindow()
        {
            Instance = this;
            DataContext = this;
            InitializeComponent(); 
            Api = new API();
            Loaded+= OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            listCountry.ItemsSource = await Api.Catalog();
            if (!string.IsNullOrEmpty(Settings.Default.LastUrlSity))
            {
                await detail.Set(null, null, new Sity { Url = Settings.Default.LastUrlSity });
                tabControl.SelectedIndex = 1;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void CountrySelectChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            listRegiones.ItemsSource=null;
            listSitys.ItemsSource = null;
            Country c = (Country)listCountry.SelectedItem;
            if(c==null) return;
            if (c.Regions.Count == 0)
                c.Regions = await Api.Regiones(c);
            listRegiones.ItemsSource = c.Regions;
            if ((c.Regions.Count >0))
                listRegiones.SelectedIndex = 0;
        }

        private async void RegionSelectChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Region r = (Region)listRegiones.SelectedItem;
            if(r==null) return;
            if(r.Sitys.Count==0)
                r.Sitys.AddRange( await Api.Sitys(r));
            listSitys.ItemsSource = r.Sitys;
        }

        private async void SitySelect(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Country c = (Country)listCountry.SelectedItem;
            Region r = (Region)listRegiones.SelectedItem;
            Sity s = (Sity)listSitys.SelectedItem;
            await detail.Set(c,r,s);
            tabControl.SelectedIndex = 1;
            Settings.Default.LastUrlSity = s.Url;
            Settings.Default.Save();
        }
    }
}
