using System;
using System.Globalization;
using System.ComponentModel;
using LockEx.Resources;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Collections.Generic;

namespace LockEx.Models.DateTimeControl
{

    public class DateTimeControlView : INotifyPropertyChanged
    {

        private static Visibility StringToVisibility(string value)
        {
            bool bvalue;
            try
            {
                bvalue = Convert.ToBoolean(value);
            }
            catch
            {
                bvalue = true;
            }
            return bvalue ? Visibility.Visible : Visibility.Collapsed;
        }
        private static string VisibilityToString(Visibility value)
        {
            return (value == Visibility.Visible) ? "true" : "false";
        }

        private DateTime _value;
        public DateTime Value 
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                RaisePropertyChanged("Value");
                RaisePropertyChanged("AMPM");
                RaisePropertyChanged("HoursMinutes");
                RaisePropertyChanged("Seconds");
                RaisePropertyChanged("DayOfWeek");
                RaisePropertyChanged("Date");
            }
        }
        private static Visibility _secondsVisible;
        public static Visibility SecondsVisible
        {
            get
            {
                if (DesignerProperties.IsInDesignTool) return _secondsVisible;
                return (IsolatedStorageSettings.ApplicationSettings.Contains("SecondsVisible")) ?
                    ((Convert.ToBoolean(IsolatedStorageSettings.ApplicationSettings["SecondsVisible"]))? Visibility.Visible : Visibility.Collapsed) :
                    ((Convert.ToBoolean(AppResources.DefaultSecondsVisible))? Visibility.Visible : Visibility.Collapsed);
            }
            set
            {
                _secondsVisible = value;
                if (!DesignerProperties.IsInDesignTool)
                {
                    IsolatedStorageSettings.ApplicationSettings["SecondsVisible"] = (value == Visibility.Visible);
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
            }
        }
        public static bool SecondsVisibleBool
        {
            get
            {
                return SecondsVisible == Visibility.Visible;
            }
            set
            {
                SecondsVisible = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility AMPMVisible
        {
            get
            {
                return (HourFormat == "24") ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public string AMPM
        {
            get
            {
                return Value.ToString("tt", new CultureInfo("en-US")).ToUpper();
            }
        }
        private static string _hourFormat;
        public static string HourFormat
        {
            get
            {
                if (DesignerProperties.IsInDesignTool) return _hourFormat;
                return (IsolatedStorageSettings.ApplicationSettings.Contains("HourFormat")) ?
                    (string)IsolatedStorageSettings.ApplicationSettings["HourFormat"] :
                    AppResources.DefaultHourFormat;
            }
            set
            {
                _hourFormat = value;
                if (!DesignerProperties.IsInDesignTool)
                {
                    IsolatedStorageSettings.ApplicationSettings["HourFormat"] = value;
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
            }
        }
        public static bool HourFormatIs12
        {
            get
            {
                return HourFormat == "12";
            }
            set
            {
                HourFormat = value ? "12" : "24";
            }
        }
        public string HoursMinutes
        {
            get
            {
                if(HourFormat == "24")
                {
                    return Value.ToString("HH:mm");
                }
                else
                {
                    return Value.ToString("hh:mm");
                }
            }
        }
        public string Seconds
        {
            get
            {
                return Value.ToString(":ss");
            }
        }
        public string DayOfWeek
        {
            get
            {
                return Value.ToString("dddd");
            }
        }
        public string Date
        {
            get
            {
                return Value.ToString(", d. MMMM yyyy");
            }
        }

        public DateTimeControlView()
            : this(DateTime.Now, "24", Visibility.Visible) { }
        public DateTimeControlView(DateTime value, string hourFormat, Visibility secondsVisible)
        {
            _value = value;
            _hourFormat = hourFormat;
            _secondsVisible = secondsVisible;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public DateTimeControlView GetCopy()
        {
            DateTimeControlView copy = (DateTimeControlView)this.MemberwiseClone();
            return copy;
        }

    }

}
