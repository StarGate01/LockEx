using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using LockEx.Resources;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Net;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Data;

using LockEx.OWM;

namespace LockEx.Models.WeatherControl
{

    public class WeatherControlEntry : INotifyPropertyChanged
    {

        public enum WeatherStates
        {
            Clear, FewClouds, ScatteredClouds, 
            BrokenClouds, ShowerRain, Rain, 
            Thunderstorm, Snow, Mist
        }
        public enum DateDescriptions
        {
            Today, Tomorrow, TheDayAfter, InAWeek, InNDays
        }

        #region Maps

        private static Dictionary<WeatherStates, string> WeatherStatesTextMap = new Dictionary<WeatherStates, string>()
        {
            { WeatherStates.Clear, AppResources.Clear },
            { WeatherStates.FewClouds, AppResources.FewClouds },
            { WeatherStates.ScatteredClouds, AppResources.ScatteredClouds },
            { WeatherStates.BrokenClouds, AppResources.BrokenClouds },
            { WeatherStates.ShowerRain, AppResources.ShowerRain },
            { WeatherStates.Rain, AppResources.Rain },
            { WeatherStates.Thunderstorm, AppResources.Thunderstorm },
            { WeatherStates.Snow, AppResources.Snow },
            { WeatherStates.Mist, AppResources.Mist }
        };
        private static Dictionary<WeatherStates, Uri> WeatherStatesImageUriMap = new Dictionary<WeatherStates, Uri>()
        {
            { WeatherStates.Clear, new Uri("/Assets/Weather/clear.png", UriKind.Relative) },
            { WeatherStates.FewClouds, new Uri("/Assets/Weather/cloud1.png", UriKind.Relative) },
            { WeatherStates.ScatteredClouds, new Uri("/Assets/Weather/cloud.png", UriKind.Relative)},
            { WeatherStates.BrokenClouds, new Uri("/Assets/Weather/cloud.png", UriKind.Relative) },
            { WeatherStates.ShowerRain, new Uri("/Assets/Weather/rain2.png", UriKind.Relative) },
            { WeatherStates.Rain, new Uri("/Assets/Weather/rain1.png", UriKind.Relative) },
            { WeatherStates.Thunderstorm, new Uri("/Assets/Weather/thunderstorm.png", UriKind.Relative) },
            { WeatherStates.Snow, new Uri("/Assets/Weather/snow.png", UriKind.Relative) },
            { WeatherStates.Mist, new Uri("/Assets/Weather/mist.png", UriKind.Relative) }
        };
        private static Dictionary<DateDescriptions, string> DateDescriptionsTextMap = new Dictionary<DateDescriptions, string>()
        {
            { DateDescriptions.Today, AppResources.Today },
            { DateDescriptions.Tomorrow, AppResources.Tomorrow },
            { DateDescriptions.TheDayAfter, AppResources.TheDayAfter },
            { DateDescriptions.InAWeek, AppResources.InAWeek },
            { DateDescriptions.InNDays, AppResources.InNDays }
        };
        #endregion

        private DateTime _day;
        public DateTime Day
        {
            get
            {
                return _day;
            }
            set
            {
                _day = value;
                RaisePropertyChanged("Day");
                RaisePropertyChanged("DateAndWeatherText");
            }
        }
        private WeatherStates _weatherState;
        public WeatherStates WeatherState 
        {
            get
            {
                return _weatherState;
            }
            set
            {
                _weatherState = value;
                RaisePropertyChanged("WeatherState");
                RaisePropertyChanged("DateAndWeatherText");
                RaisePropertyChanged("ImageUri");
            }
        }
        public DateDescriptions DateDescription
        {
            get
            {
                if (Day == DateTime.Today) return DateDescriptions.Today;
                if (Day == DateTime.Today.AddDays(1)) return DateDescriptions.Tomorrow;
                if (Day == DateTime.Today.AddDays(2)) return DateDescriptions.TheDayAfter;
                if (Day == DateTime.Today.AddDays(7)) return DateDescriptions.InAWeek;
                return DateDescriptions.InNDays;
            }
        }
        private string _info;
        public string Info
        {
            get
            {
                return _info;
            }
            set
            {
                _info = value;
                RaisePropertyChanged("Info");
            }
        }
        private double _minTemp;
        public double MinTemp
        {
            get
            {
                return _minTemp;
            }
            set
            {
                _minTemp = value;
                RaisePropertyChanged("MinTemp");
                RaisePropertyChanged("TemperatureText");
            }
        }
        private double _maxTemp;
        public double MaxTemp
        {
            get
            {
                return _maxTemp;
            }
            set
            {
                _maxTemp = value;
                RaisePropertyChanged("MaxTemp");
                RaisePropertyChanged("TemperatureText");
            }
        }

        public string DateAndWeatherText
        {
            get
            {
                if (DateDescription != DateDescriptions.InNDays)
                {
                    return DateDescriptionsTextMap[DateDescription] + " " + WeatherStatesTextMap[WeatherState];
                }
                else
                {
                    int daysDiff = (int)(Day - DateTime.Today).TotalDays;
                    return DateDescriptionsTextMap[DateDescription].Replace("%d", daysDiff.ToString()) + " " + WeatherStatesTextMap[WeatherState];
                }
            }
        }
        public string TemperatureText
        {
            get
            {
                double newMin = MinTemp;
                double newMax = MaxTemp;
                switch (WeatherControlView.TempSuffix)
                {
                    case WeatherControlView.TempSuffixes.Farenheit:
                        newMin = (1.8 * MinTemp) + 32;
                        newMax = (1.8 * MaxTemp) + 32;
                        break;
                    case WeatherControlView.TempSuffixes.Kelvin:
                        newMin = MinTemp + 273.15;
                        newMax = MaxTemp + 273.15;
                        break;
                }
                return newMin + "°" + WeatherControlView.TempSuffixesCharMap[WeatherControlView.TempSuffix] + 
                    " " + AppResources.TempTo + " " + newMax + "°" +  WeatherControlView.TempSuffixesCharMap[WeatherControlView.TempSuffix];
            }
        }
        public Uri ImageUri
        {
            get
            {
                return WeatherStatesImageUriMap[WeatherState];
            }
        }

        public WeatherControlEntry(DateTime day, WeatherStates weatherState, string info, double minTemp, double maxTemp)
        {
            _day = day;
            _weatherState = weatherState;
            _info = info;
            _minTemp = minTemp;
            _maxTemp = maxTemp;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public WeatherControlEntry GetCopy()
        {
            WeatherControlEntry copy = (WeatherControlEntry)this.MemberwiseClone();
            return copy;
        }

    }

    public class TempSuffixesIntConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)(WeatherControlView.TempSuffixes)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Enum.GetValues(typeof(WeatherControlView.TempSuffixes)).GetValue((int)value);
        }
    }

    public class WeatherControlView: INotifyPropertyChanged
    {

        public enum TempSuffixes
        {
            Celsius, Farenheit, Kelvin
        }

        #region Maps
        private static Dictionary<int, WeatherControlEntry.WeatherStates> OWAIconsWeatherStatesMap = new Dictionary<int, WeatherControlEntry.WeatherStates>()
        {
            { 1, WeatherControlEntry.WeatherStates.Clear},
            { 2, WeatherControlEntry.WeatherStates.FewClouds},
            { 3, WeatherControlEntry.WeatherStates.ScatteredClouds},
            { 4, WeatherControlEntry.WeatherStates.BrokenClouds },
            { 9, WeatherControlEntry.WeatherStates.ShowerRain},
            { 10, WeatherControlEntry.WeatherStates.Rain },
            { 11, WeatherControlEntry.WeatherStates.Thunderstorm },
            { 13, WeatherControlEntry.WeatherStates.Snow },
            { 50, WeatherControlEntry.WeatherStates.Mist }
        };
        public static Dictionary<TempSuffixes, string> TempSuffixesCharMap = new Dictionary<TempSuffixes, string>()
        {
            { TempSuffixes.Celsius, "C" },
            { TempSuffixes.Farenheit, "F" },
            { TempSuffixes.Kelvin, "K" }
        };
        private static Dictionary<string, TempSuffixes> TempSuffixesReverseCharMap = new Dictionary<string, TempSuffixes>()
        {
            { "C", TempSuffixes.Celsius },
            { "F", TempSuffixes.Farenheit },
            { "K", TempSuffixes.Kelvin }
        };
        #endregion

        private ObservableCollection<WeatherControlEntry> _entries;
        public ObservableCollection<WeatherControlEntry> Entries
        { 
            get
            {
                return _entries;
            }
            set
            {
                _entries = value;
                RaisePropertyChanged("Entries");
            }
        }
        private static string _city;
        public static string City
        {
            get
            {
                if (DesignerProperties.IsInDesignTool) return _city;
                return (IsolatedStorageSettings.ApplicationSettings.Contains("City")) ?
                    (string)IsolatedStorageSettings.ApplicationSettings["City"] :
                    AppResources.DefaultCity;
            }
            set
            {
                _city = value;
                if (!DesignerProperties.IsInDesignTool)
                {
                    IsolatedStorageSettings.ApplicationSettings["City"] = value;
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
            }
        }
        private Visibility _errorVisible;
        public Visibility ErrorVisible
        {
            get
            {
                return _errorVisible;
            }
            set
            {
                _errorVisible = value;
                RaisePropertyChanged("ErrorVisible");
                RaisePropertyChanged("UIVisible");
            }
        }
        public Visibility UIVisible
        {
            get
            {
                return _errorVisible == Visibility.Visible? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private static TempSuffixes _tempSuffix;
        public static TempSuffixes TempSuffix
        {
            get
            {
                if (DesignerProperties.IsInDesignTool) return _tempSuffix;
                return (IsolatedStorageSettings.ApplicationSettings.Contains("TempSuffix")) ?
                    TempSuffixesReverseCharMap[(string)IsolatedStorageSettings.ApplicationSettings["TempSuffix"]] :
                    TempSuffixesReverseCharMap[AppResources.DefaultTempSuffix];
            }
            set
            {
                _tempSuffix = value;
                if (!DesignerProperties.IsInDesignTool)
                {
                    IsolatedStorageSettings.ApplicationSettings["TempSuffix"] = TempSuffixesCharMap[value];
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
            }
        }

        private Array _tempSuffixesStrings = Enum.GetValues(typeof(TempSuffixes));
        public Array TempSuffixesStrings
        {
            get
            {
                return _tempSuffixesStrings;
            }
        }

        private OWMClient owmClient;

        public WeatherControlView()
            : this(new ObservableCollection<WeatherControlEntry>(), "", Visibility.Visible) { }
        public WeatherControlView(ObservableCollection<WeatherControlEntry> entries, string city, Visibility errorVisible)
        {
            _entries = entries;
            _city = city;
            _errorVisible = errorVisible;
            owmClient = new OWMClient("fdf8b34d6165a7a562fcfaa73f7d9d57");
        }

        public async Task PopulateData()
        {
            ErrorVisible = Visibility.Visible;
            MultipleDaysForecast resObj = await owmClient.GetDaysForcast(City, AppResources.OWMlang, 9);
            if (resObj != null)
            {
                ErrorVisible = Visibility.Collapsed;
                resObj.Forecasts.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));
                Entries.Clear();
                foreach (Forecast forecast in resObj.Forecasts)
                {
                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(forecast.Timestamp);
                    dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
                    if (dateTime >= DateTime.Today)
                    {
                        int icon = Convert.ToInt32(Regex.Replace(forecast.Weathers[0].Icon, "[^0-9]", ""));
                        string info = char.ToUpper(forecast.Weathers[0].Description[0]) + forecast.Weathers[0].Description.Substring(1);
                        Entries.Add(new WeatherControlEntry(dateTime, OWAIconsWeatherStatesMap[icon],
                            info, Math.Round(forecast.Temperature.Min), Math.Round(forecast.Temperature.Max)));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public WeatherControlView GetCopy()
        {
            WeatherControlView copy = (WeatherControlView)this.MemberwiseClone();
            return copy;
        }

    }

}
