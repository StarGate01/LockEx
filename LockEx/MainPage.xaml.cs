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
using Microsoft.Phone.BackgroundAudio;
using System.Windows.Media;
using System.Threading.Tasks;
using LockEx.Models.Main;

namespace LockEx
{

    public partial class MainPage : PhoneApplicationPage
    {

        private bool swipeComplete = false;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!SystemProtection.ScreenLocked)
            {
                NavigationService.Navigate(new Uri("/ExtendedSettingsPage.xaml", UriKind.Relative));
                base.OnNavigatedTo(e);
                return;
            }
            App.MainViewModel.PopulateShellChromeData();
            switch (App.MainViewModel.LeftControl)
            {
                case MainView.LeftControls.NewsControl:
                    App.MainViewModel.NewsView.PopulateData();
                    break;
                case MainView.LeftControls.WeatherControl:
                    App.MainViewModel.WeatherView.PopulateData();
                    break;
            }
            App.MainViewModel.GlobalYOffset = 0;
            base.OnNavigatedTo(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            ExtensibilityApp.EndUnlock();
            MainSnapBack.Begin();
            e.Cancel = true;
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

        private void ButtonFlashlight_Click(object sender, RoutedEventArgs e)
        {
            App.MainViewModel.Flashlight.IsTurnedOn = !App.MainViewModel.Flashlight.IsTurnedOn;
        }

    }

}