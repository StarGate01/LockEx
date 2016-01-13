using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LockEx.Resources;
using Windows.Phone.System;
using Windows.Graphics.Display;
using Windows.UI.Core;
//using Facet_Lockscreen_Bridge;
using Microsoft.Phone.Scheduler;
using System.Runtime.InteropServices;

namespace LockEx
{
    public partial class MainPage : PhoneApplicationPage
    {

        public MainPage()
        {
            InitializeComponent();
            IEnumerable<ScheduledNotification> notifications = ScheduledActionService.GetActions<ScheduledNotification>();
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
            var cont = Application.Current.Host.Content;
            /*LockScreenSnapshot snap = new LockScreenSnapshot((int)Math.Ceiling(cont.ActualWidth * cont.ScaleFactor / 100.0), (int)Math.Ceiling(cont.ActualHeight * cont.ScaleFactor / 100.0));
            FirstLine.Text = snap.DetailedTexts[0].Text;
            FirstLine.FontWeight = snap.DetailedTexts[0].IsBold ? FontWeights.Bold : FontWeights.Normal;
            SecondLine.Text = snap.DetailedTexts[1].Text;
            SecondLine.FontWeight = snap.DetailedTexts[1].IsBold ? FontWeights.Bold : FontWeights.Normal;
            ThirdLine.Text = snap.DetailedTexts[2].Text;
            ThirdLine.FontWeight = snap.DetailedTexts[2].IsBold ? FontWeights.Bold : FontWeights.Normal;*/
        }

    }

}