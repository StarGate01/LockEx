using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.System;

namespace LockEx
{

    public partial class ExtendedSettingsPage : PhoneApplicationPage
    {

        public ExtendedSettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.RemoveBackEntry();
            base.OnNavigatedTo(e);
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(new Uri((string)((HyperlinkButton)sender).Tag));
        }

    }

}