using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Windows.Phone.System;
using System.Windows.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Windows.Phone.System.LockScreenExtensibility;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Media;

namespace LockEx
{

    public partial class MainPage : PhoneApplicationPage
    {

        private DispatcherTimer secondsTimer;
        private DispatcherTimer musicTimer;

        private bool swipeComplete = false;

        public MainPage()
        {
            InitializeComponent();
            secondsTimer = new DispatcherTimer();
            secondsTimer.Interval = new TimeSpan(0, 0, 1);
            secondsTimer.Tick += secondsTimer_Tick;
            musicTimer = new DispatcherTimer();
            musicTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            musicTimer.Tick += musicTimer_Tick;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!SystemProtection.ScreenLocked)// && false)
            {
                NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
                base.OnNavigatedTo(e);
                return;
            }
            App.MainViewModel.PopulateShellChromeData();
            App.MainViewModel.WeatherView.PopulateData();
            FrameworkDispatcher.Update();
            secondsTimer.Start();
            musicTimer.Start();
            App.MainViewModel.GlobalYOffset = 0;
            base.OnNavigatedTo(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            ExtensibilityApp.EndUnlock();
            MainSnapBack.Begin();
            e.Cancel = true;
        }

        private void secondsTimer_Tick(object sender, EventArgs e)
        {
            App.MainViewModel.DateTimeView.Value = DateTime.Now;
            App.MainViewModel.PopulateShellChromeData();
        }

        void musicTimer_Tick(object sender, EventArgs e)
        {
            FrameworkDispatcher.Update();
            App.MainViewModel.MusicView.RaisePropertyChanged("Position");
        }

        private void ButtonUnlock_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            ExtensibilityApp.BeginUnlock();
        }

        private void ButtonUnlock_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            double targetYOffset = App.MainViewModel.GlobalYOffset += e.DeltaManipulation.Translation.Y;
            if (-App.MainViewModel.GlobalYOffset < App.MainViewModel.SwipeMax)
            {
                App.MainViewModel.GlobalYOffset = targetYOffset;
                swipeComplete = false;
            }
            else
            {
                swipeComplete = true;
            }
        }

        private void ButtonUnlock_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (!swipeComplete)
            {
                ExtensibilityApp.EndUnlock();
                MainSnapBack.Begin();
            }
            else
            {
                if(!App.MainViewModel.HasPasscode)
                {
                    SystemProtection.RequestScreenUnlock();
                }
            }
        }

        private void SnapBackAnimation_Completed(object sender, EventArgs e)
        {
            Binding binding = new Binding("GlobalYOffset");
            binding.Source = App.MainViewModel;
            BindingOperations.SetBinding(GlobalYO, CompositeTransform.TranslateYProperty, binding);
            App.MainViewModel.GlobalYOffset = 0;
        }

    }

}