using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Phone.System.LockScreenExtensibility;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.IO;

using RTComponent;
using RTComponent.NotificationsSnapshot;

namespace LockEx.Extensions
{

    public partial class SettingsPage : PhoneApplicationPage
    {

        NativeAPI NAPI;
        Image[] testImages;
        TextBlock[] testTexts;

        public SettingsPage()
        {
            InitializeComponent();
            NAPI = new NativeAPI();
            var cont = Application.Current.Host.Content;
            NAPI.InitUIXMAResources((int) Math.Ceiling(cont.ActualWidth * cont.ScaleFactor * 0.01), (int) Math.Ceiling(cont.ActualHeight * cont.ScaleFactor * 0.01));
            ToggleMain.IsChecked = ExtensibilityApp.IsLockScreenApplicationRegistered();
            testImages = new Image[] {
                TestImage0, TestImage1, TestImage2, TestImage3, TestImage4
            };
            testTexts = new TextBlock[] {
                TestText0, TestText1, TestText2, TestText3, TestText4
            };
        }

        private void ToggleMain_Checked(object sender, RoutedEventArgs e)
        {
            if (!ExtensibilityApp.IsLockScreenApplicationRegistered()) ExtensibilityApp.RegisterLockScreenApplication();
        }

        private void ToggleMain_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ExtensibilityApp.IsLockScreenApplicationRegistered()) ExtensibilityApp.UnregisterLockScreenApplication();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.RemoveBackEntry();
            Snapshot snap = NAPI.GetNotificationsSnapshot();
            TestText.Text = snap.DetailedTexts.Select(p => p.DetailedText).Aggregate((a, b) => a + Environment.NewLine + b);
            for(int i=0; i<snap.BadgeCount && i<testImages.Length; i++)
            {
                if(snap.Badges[i].Type != BadgeValueType.None)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    string[] glypghUri = snap.Badges[i].IconUri.Split(new string[] { "://" }, StringSplitOptions.None);
                    if (glypghUri[0] == "res")
                    {
                        string[] resNames = glypghUri[1].Split(new String[] { "!" }, StringSplitOptions.None);
                        if(resNames[0] == "UIXMobileAssets{ScreenResolution}") bitmapImage.SetSource(new MemoryStream(NAPI.GetUIXMAResource(resNames[1])));
                    }
                    else if (glypghUri[0] == "file") bitmapImage.SetSource(new FileStream(glypghUri[1], FileMode.Open));
                    testImages[i].Source = bitmapImage;
                    if (snap.Badges[i].Type == BadgeValueType.Count) testTexts[i].Text = snap.Badges[i].Value;
                }
                else
                {
                    testImages[i].Source = null;
                    testTexts[i].Text = "";
                }
            }
            base.OnNavigatedTo(e);
        }

    }

}