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

namespace LockEx.Extensions
{

    public partial class SettingsPage : PhoneApplicationPage
    {

        private const string UICMARPrefix = "res://UIXMobileAssets{ScreenResolution}!";
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
            ToggleMain.IsChecked = ExtensibilityApp.IsLockScreenApplicationRegistered();
            badgeImages = new Image[] { TestImage0, TestImage1, TestImage2, TestImage3, TestImage4 };
            indicatorImages = new Image[] { TestImageA, TestImageB, TestImageC };
            badgeTexts = new TextBlock[] { TestText0, TestText1, TestText2, TestText3, TestText4 };
            detailedTexts = new TextBlock[] { TestTextA, TestTextB, TestTextC };
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
                    if (snap.Badges[i].IconUri.StartsWith(UICMARPrefix))
                    {
                        bitmapImage.SetSource(new MemoryStream(NAPI.GetUIXMAResource(snap.Badges[i].IconUri.Substring(UICMARPrefix.Length))));
                    }
                    else if (snap.Badges[i].IconUri.StartsWith(FilePrefix))
                    {
                        bitmapImage.SetSource(new FileStream(snap.Badges[i].IconUri.Substring(FilePrefix.Length), FileMode.Open));
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
                if(iconUris[i] != "" && iconUris[i].StartsWith(UICMARPrefix))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(new MemoryStream(NAPI.GetUIXMAResource(iconUris[i].Substring(UICMARPrefix.Length))));
                    indicatorImages[i].Source = bitmapImage;
                }
                else
                {
                    //testImages2[i].Source = null
                }
            }
            base.OnNavigatedTo(e);
        }

    }

}