using System;
using LockEx.Models.WeatherControl;
using LockEx.Models.BadgesControl;
using LockEx.Models.DateTimeControl;
using LockEx.Models.DetailedTextControl;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Windows;
using System.IO;
using System.IO.IsolatedStorage;
using Windows.Phone.System.LockScreenExtensibility;
using LockEx.Resources;

using RTComponent;
using RTComponent.NotificationsSnapshot;

namespace LockEx.Models.Main
{

    public class MainView : INotifyPropertyChanged
    {

        private WeatherControlView _weatherView;
        public WeatherControlView WeatherView
        {
            get
            { 
                return _weatherView;
            }
            set
            {
                _weatherView = value; 
                RaisePropertyChanged("WeatherView");
            }
        }
        private BadgesControlView _badgesView;
        public BadgesControlView BadgesView 
        { 
            get
            {
                return _badgesView;
            }
            set
            {
                _badgesView = value;
                RaisePropertyChanged("BadgesView");
            }
        }
        private DateTimeControlView _dateTimeView;
        public DateTimeControlView DateTimeView 
        {
            get
            {
                return _dateTimeView;
            }
            set
            {
                _dateTimeView = value;
                RaisePropertyChanged("DateTimeView");
            }
        }
        private DetailedTextControlView _detailedTextView;
        public DetailedTextControlView DetailedTextView
        {
            get
            {
                return _detailedTextView;
            }
            set
            {
                _detailedTextView = value;
                RaisePropertyChanged("DetailedTextView");
            }
        }
        private Uri _imageUri;
        public Uri ImageUri
        {
            get
            {
                return _imageUri;
            }
            set
            {
                _imageUri = value;
                RaisePropertyChanged("ImageUri");
            }
        }
        private bool _isLockscreen;
        public bool IsLockscreen
        {
            get
            {
                if (DesignerProperties.IsInDesignTool) return _isLockscreen;
                return ExtensibilityApp.IsLockScreenApplicationRegistered();
            }
            set
            {
                if (DesignerProperties.IsInDesignTool) _isLockscreen = value;
                if (value && !ExtensibilityApp.IsLockScreenApplicationRegistered())
                {
                    ExtensibilityApp.RegisterLockScreenApplication();
                    RaisePropertyChanged("IsLockscreen");
                }
                else if (!value && ExtensibilityApp.IsLockScreenApplicationRegistered())
                {
                    ExtensibilityApp.UnregisterLockScreenApplication();
                    RaisePropertyChanged("IsLockscreen");
                }
            }
        }
        private bool _longTextMode;
        public bool LongTextMode
        {
            get
            {
                if (DesignerProperties.IsInDesignTool) return _longTextMode;
                return (IsolatedStorageSettings.ApplicationSettings.Contains("LongTextMode")) ?
                    (Convert.ToBoolean(IsolatedStorageSettings.ApplicationSettings["LongTextMode"])) :
                    (Convert.ToBoolean(AppResources.DefaultLongTextMode));
            }
            set
            {
                _longTextMode = value;
                if (!DesignerProperties.IsInDesignTool)
                {
                    IsolatedStorageSettings.ApplicationSettings["LongTextMode"] = value.ToString();
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
                RaisePropertyChanged("LongTextMode");
                RaisePropertyChanged("RightPanelRowSpan");
            }
        }
        public int RightPanelRowSpan
        {
            get
            {
                return LongTextMode ? 3 : 1;
            }
        }

        private Uri defaultImageUri = new Uri("/Assets/Backgrounds/blue_mountains_lq.jpg", UriKind.Relative);

        private NativeAPI NAPI;
        private const string UIXMARPrefix = "res://UIXMobileAssets{ScreenResolution}!";
        private const string FilePrefix = "file://";

        public MainView()
        {
            _weatherView = new WeatherControlView();
            _badgesView = new BadgesControlView();
            _dateTimeView = new DateTimeControlView();
            _detailedTextView = new DetailedTextControlView();
            if(!DesignerProperties.IsInDesignTool)
            { 
                NAPI = new NativeAPI();
                var cont = Application.Current.Host.Content;
                NAPI.InitUIXMAResources((int)Math.Ceiling(cont.ActualWidth * cont.ScaleFactor * 0.01), (int)Math.Ceiling(cont.ActualHeight * cont.ScaleFactor * 0.01));
            }
            else
            {
                PopulateDesignerData();
            }
        }

        public void PopulateData()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("CustomImage"))
                _imageUri = (Uri) IsolatedStorageSettings.ApplicationSettings["CustomImage"];
            else _imageUri = defaultImageUri;
        }

        public void PopulateShellChromeData()
        {
            Snapshot snap = NAPI.GetNotificationsSnapshot();
            ObservableCollection<DetailedTextControlEntry> newTextControlEntries = new ObservableCollection<DetailedTextControlEntry>();
            for (int i = 0; i < 3; i++)
            {
                newTextControlEntries.Add(new DetailedTextControlEntry(
                    snap.DetailedTexts[i].DetailedText,
                    snap.DetailedTexts[i].IsBoldText
                ));
            }
            DetailedTextView.Entries = newTextControlEntries;
            ObservableCollection<BadgesControlEntry> newBadgeControlEntries = new ObservableCollection<BadgesControlEntry>();
            for (int i = 0; i < snap.BadgeCount && i < 5; i++)
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
                    newBadgeControlEntries.Add(new BadgesControlEntry(bitmapImage, snap.Badges[i].Value));
                }
            }
            BadgesView.Entries = newBadgeControlEntries;
            /*string[] iconUris = new string[] { snap.AlarmIconUri, snap.DoNotDisturbIconUri, snap.DrivingModeIconUri };
            for (int i = 0; i < 3; i++)
            {
                if (iconUris[i] != "" && iconUris[i].StartsWith(UIXMARPrefix))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(new MemoryStream(NAPI.GetUIXMAResource(iconUris[i].Substring(UIXMARPrefix.Length))));
                    indicatorImages[i].Source = bitmapImage;
                }
                else
                {
                    //testImages2[i].Source = null
                }
            }*/
        }

        public void PopulateDesignerData()
        {
            _imageUri = defaultImageUri;
            _isLockscreen = true;
            _longTextMode = false;
            WeatherView.Entries = new ObservableCollection<WeatherControlEntry>()
            {
                new WeatherControlEntry(DateTime.Today, WeatherControlEntry.WeatherStates.Clear, "Lorem ipsum", 20.4, 30.6),
                new WeatherControlEntry(DateTime.Today.AddDays(1), WeatherControlEntry.WeatherStates.FewClouds, "Sit dolor amet", -10.4, 3),
                new WeatherControlEntry(DateTime.Today.AddDays(2), WeatherControlEntry.WeatherStates.Rain, "Consecutivis nam", -10.4, 3),
                new WeatherControlEntry(DateTime.Today.AddDays(3), WeatherControlEntry.WeatherStates.Snow, "Labor hams", -20, 23.5),
                new WeatherControlEntry(DateTime.Today.AddDays(7), WeatherControlEntry.WeatherStates.Thunderstorm, "Mongolis plebeiis", 3, 11),
                new WeatherControlEntry(DateTime.Today.AddDays(14), WeatherControlEntry.WeatherStates.BrokenClouds, "Ay Macarena", 14, 18.7)
            };
            WeatherControlView.City = "München";
            WeatherControlView.TempSuffix = WeatherControlView.TempSuffixes.Celsius;
            WeatherView.ErrorVisible = Visibility.Collapsed;
            ObservableCollection<BadgesControlEntry> badgesEntries = new ObservableCollection<BadgesControlEntry>();
            BitmapImage placeholder = new BitmapImage(new Uri("/Assets/ApplicationIcon.png", UriKind.Relative));
            for (int i = 0; i < 5; i++) badgesEntries.Add(new BadgesControlEntry(placeholder, "0"));
            BadgesView.Entries = badgesEntries;
            DateTimeView.Value = DateTime.Now;
            DateTimeControlView.HourFormat = "24";
            DateTimeControlView.SecondsVisible = Visibility.Visible;
            DetailedTextView.Entries = new ObservableCollection<DetailedTextControlEntry>()
            {
                new DetailedTextControlEntry("Lockscreen fertig programmieren, und diese Zeile ist auch sehr lang", true),
                new DetailedTextControlEntry("Zuhause, am Rechner", false),
                new DetailedTextControlEntry("Morgen: 12:00 - 15:00 Uhr", false)
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public MainView GetCopy()
        {
            MainView copy = (MainView)this.MemberwiseClone();
            return copy;
        }

    }

}
