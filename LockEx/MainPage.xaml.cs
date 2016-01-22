using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Windows.Phone.System;
using System.Windows.Threading;

namespace LockEx
{

    public partial class MainPage : PhoneApplicationPage
    {

        private DispatcherTimer secondsTimer;

        public MainPage()
        {
            InitializeComponent();
            secondsTimer = new DispatcherTimer();
            secondsTimer.Interval = new TimeSpan(0, 0, 1);
            secondsTimer.Tick += secondsTimer_Tick;
            secondsTimer.Start();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!SystemProtection.ScreenLocked)
            {
                NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
                base.OnNavigatedTo(e);
                return;
            }
            App.MainViewModel.WeatherView.PopulateData();
            base.OnNavigatedTo(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void secondsTimer_Tick(object sender, EventArgs e)
        {
            App.MainViewModel.DateTimeView.Value = DateTime.Now;
        }

        private void ButtonUnlock_Click(object sender, RoutedEventArgs e)
        {
            if (SystemProtection.ScreenLocked) SystemProtection.RequestScreenUnlock();
        }

    }

}