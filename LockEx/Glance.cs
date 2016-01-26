using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using System.Windows.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;

namespace LockEx.Hardware
{

    public class Glance : INotifyPropertyChanged
    {

        public enum GlanceEventArgs
        {
            Dark, Light
        }

        private LightSensor sensor;
        private DispatcherTimer _updateSensor;
        public DispatcherTimer UpdateSensor
        {
            get
            {
                return _updateSensor;
            }
        }
        private double defaultDark = 4;
        private double _dark;
        public double Dark
        {
            get
            {
                if (DesignerProperties.IsInDesignTool) return _dark;
                return (IsolatedStorageSettings.ApplicationSettings.Contains("GlanceDark")) ?
                    (Convert.ToDouble(IsolatedStorageSettings.ApplicationSettings["GlanceDark"])) :
                    defaultDark;
            }
            set
            {
                _dark = value;
                if (!DesignerProperties.IsInDesignTool)
                {
                    IsolatedStorageSettings.ApplicationSettings["GlanceDark"] = value.ToString();
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
                RaisePropertyChanged("Dark");
            }
        }
        private TimeSpan _graceTime;
        public TimeSpan GraceTime
        {
            get
            {
                return _graceTime;
            }
            set
            {
                _graceTime = value;
                RaisePropertyChanged("DeltaTime");
            }
        }
        private DateTime lastEffectiveReadingTime;
        private GlanceEventArgs lastState;

        public delegate void GlanceEventHandler(object sender, GlanceEventArgs args);
        public event GlanceEventHandler GlanceEvent;
        private void RaiseGlanceEvent(GlanceEventArgs args)
        {
            if (this.GlanceEvent != null) this.GlanceEvent(this, args);
        }

        public Glance()
        {
            _updateSensor = new DispatcherTimer();
            _updateSensor.Interval = TimeSpan.FromSeconds(0.25);
            _updateSensor.Tick += UpdateSensor_Tick;
            sensor = LightSensor.GetDefault();
            if (sensor != null)
            {
                //sensor.ReportInterval = sensor.MinimumReportInterval;
                lastEffectiveReadingTime = DateTime.Now;
                lastState = sensor.GetCurrentReading().IlluminanceInLux <= Dark ? GlanceEventArgs.Dark : GlanceEventArgs.Light;
            }
        }

        void UpdateSensor_Tick(object sender, EventArgs e)
        {
            if(sensor == null) return;
            try
            {
                lock (sensor)
                {
                    float lux = sensor.GetCurrentReading().IlluminanceInLux;
                    if (lastEffectiveReadingTime < DateTime.Now - GraceTime)
                    {
                        if (lux <= Dark && lastState == GlanceEventArgs.Light)
                        {
                            lastEffectiveReadingTime = DateTime.Now;
                            lastState = GlanceEventArgs.Dark;
                            RaiseGlanceEvent(GlanceEventArgs.Dark);
                        }
                        else if (lux > Dark && lastState == GlanceEventArgs.Dark)
                        {
                            lastEffectiveReadingTime = DateTime.Now;
                            lastState = GlanceEventArgs.Light;
                            RaiseGlanceEvent(GlanceEventArgs.Light);
                        }
                    }
                }
            }
            catch { }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public Glance GetCopy()
        {
            Glance copy = (Glance)this.MemberwiseClone();
            return copy;
        }

    }

}
