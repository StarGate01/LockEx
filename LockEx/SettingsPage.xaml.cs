using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

using System.Windows.Media.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Phone.System.UserProfile;

namespace LockEx
{

    public partial class SettingsPage : PhoneApplicationPage
    {

        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.RemoveBackEntry();
            base.OnNavigatedTo(e);
        }

    }

}