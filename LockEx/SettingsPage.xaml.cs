using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Windows.Phone.System.LockScreenExtensibility;
using System.Windows.Media.Imaging;
using System.IO;

using RTComponent;
using RTComponent.NotificationsSnapshot;

namespace LockEx
{

    public partial class SettingsPage : PhoneApplicationPage
    {

        private const string UIXMARPrefix = "res://UIXMobileAssets{ScreenResolution}!";
        private const string FilePrefix = "file://";
        private NativeAPI NAPI;
        private Image[] badgeImages;
        private Image[] indicatorImages;
        private TextBlock[] badgeTexts;
        private TextBlock[] detailedTexts;

        public SettingsPage()
        {
            InitializeComponent();
            NAPI = new NativeAPI();
            var cont = Application.Current.Host.Content;
            NAPI.InitUIXMAResources((int) Math.Ceiling(cont.ActualWidth * cont.ScaleFactor * 0.01), (int) Math.Ceiling(cont.ActualHeight * cont.ScaleFactor * 0.01));
            badgeImages = new Image[] { BadgeImage0, BadgeImage1, BadgeImage2, BadgeImage3, BadgeImage4 };
            indicatorImages = new Image[] { IndicatorImage0, IndicatorImage1, IndicatorImage2 };
            badgeTexts = new TextBlock[] { BadgeValue0, BadgeValue1, BadgeValue2, BadgeValue3, BadgeValue4 };
            detailedTexts = new TextBlock[] { DetailedText0, DetailedText1, DetailedText2 };
            ToggleMain.IsChecked = ExtensibilityApp.IsLockScreenApplicationRegistered();
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
            for (int i = 0; i < 3; i++)
            {
                detailedTexts[i].Text = snap.DetailedTexts[i].DetailedText;
                detailedTexts[i].FontWeight = (snap.DetailedTexts[i].IsBoldText) ? FontWeights.Bold : FontWeights.Normal;
            }
            for (int i = 0; i < snap.BadgeCount && i < badgeImages.Length; i++)
            {
                if (snap.Badges[i].Type != BadgeValueType.None)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    if (snap.Badges[i].IconUri.StartsWith(UIXMARPrefix))
                    {
                        bitmapImage.SetSource(new MemoryStream(NAPI.GetUIXMAResource(snap.Badges[i].IconUri.Substring(UIXMARPrefix.Length))));
                    }
                    else if (snap.Badges[i].IconUri.StartsWith(FilePrefix))
                    {
                        MemoryStream ms = new MemoryStream();
                        using (FileStream fs = new FileStream(snap.Badges[i].IconUri.Substring(FilePrefix.Length), FileMode.Open))
                        {
                            byte[] bytes = new byte[fs.Length];
                            fs.Read(bytes, 0, (int)fs.Length);
                            ms.Write(bytes, 0, (int)fs.Length);
                        }
                        bitmapImage.SetSource(ms);
                    }
                    badgeImages[i].Source = bitmapImage;
                    if (snap.Badges[i].Type == BadgeValueType.Count) badgeTexts[i].Text = snap.Badges[i].Value;
                }
                else
                {
                    //testImages[i].Source = null;
                    //testTexts[i].Text = "";
                }
            }
            string[] iconUris = new string[] { snap.AlarmIconUri, snap.DoNotDisturbIconUri, snap.DrivingModeIconUri };
            for (int i = 0; i < 3; i++)
            {
                if(iconUris[i] != "" && iconUris[i].StartsWith(UIXMARPrefix))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(new MemoryStream(NAPI.GetUIXMAResource(iconUris[i].Substring(UIXMARPrefix.Length))));
                    indicatorImages[i].Source = bitmapImage;
                }
                else
                {
                    //testImages2[i].Source = null
                }
            }
            base.OnNavigatedTo(e);
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            NAPI.TestReminders();
        }

    }

}