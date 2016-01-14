using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Windows.Phone.System;

namespace LockEx
{
    public partial class MainPage : PhoneApplicationPage
    {

        public MainPage()
        {
            InitializeComponent();
            UpdateLockscreenSnapshot();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!SystemProtection.ScreenLocked) NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
            base.OnNavigatedTo(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void ButtonUnlock_Click(object sender, RoutedEventArgs e)
        {
            if (SystemProtection.ScreenLocked) SystemProtection.RequestScreenUnlock();
        }

        private void UpdateLockscreenSnapshot()
        {

        }

    }

}