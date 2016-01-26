using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Devices.Enumeration;
using System.Threading;
using System.ComponentModel;

namespace LockEx.Hardware
{

    //Thanks to http://stackoverflow.com/questions/24847510/toggle-flashlight-in-windows-phone-8-1
    public class Flashlight : INotifyPropertyChanged
    {

        private MediaCapture captureManager;
        private TorchControl torch;

        public async Task InitCamera()
        {
            var cameraID = await GetCameraID(Windows.Devices.Enumeration.Panel.Back);
            captureManager = new MediaCapture();
            await captureManager.InitializeAsync(new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = StreamingCaptureMode.Video,
                PhotoCaptureSource = PhotoCaptureSource.VideoPreview,
                AudioDeviceId = string.Empty,
                VideoDeviceId = cameraID.Id
            });
            torch = captureManager.VideoDeviceController.TorchControl;
            RaisePropertyChanged("IsTurnedOn");
        }

        public void UnInitCamera()
        {
            torch = null;
            if(captureManager!= null) captureManager.Dispose();
            captureManager = null;
            RaisePropertyChanged("IsTurnedOn");
        }

        private async Task<DeviceInformation> GetCameraID(Panel desiredCamera)
        {
            DeviceInformation deviceID = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture))
                .FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredCamera);
            if (deviceID != null) return deviceID;
            else throw new Exception(string.Format("Camera of type {0} doesn't exist.", desiredCamera));
        }

        private bool _isTurnedOn;
        public bool  IsTurnedOn
        {
            get
            {
                if (DesignerProperties.IsInDesignTool) return _isTurnedOn;
                return torch != null && torch.Enabled;
            }
            set
            {
                _isTurnedOn = value;
                if (!DesignerProperties.IsInDesignTool && torch != null && torch.Supported) torch.Enabled = value;
                RaisePropertyChanged("IsTurnedOn");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public Flashlight GetCopy()
        {
            Flashlight copy = (Flashlight)this.MemberwiseClone();
            return copy;
        }

    }

}
