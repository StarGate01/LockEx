using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.System;
using Microsoft.Phone.Tasks;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using Windows.Phone.System.UserProfile;
using System.Threading;
using System.Threading.Tasks;
using LockEx.Resources;

namespace LockEx
{

    public partial class ExtendedSettingsPage : PhoneApplicationPage
    {

        private PhotoChooserTask photoChooserTask;
        private IsolatedStorageFile isoStore;
        private const string imageFileName = "custom_background.jpg";

        public ExtendedSettingsPage()
        {
            isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.PixelHeight = (int)Application.Current.Host.Content.ActualHeight;
            photoChooserTask.PixelWidth = (int)Application.Current.Host.Content.ActualWidth;
            photoChooserTask.ShowCamera = true;
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            while (NavigationService.RemoveBackEntry() != null);
            base.OnNavigatedTo(e);
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(new Uri((string)((HyperlinkButton)sender).Tag));
        }

        private void ButtonChooseImage_Click(object sender, RoutedEventArgs e)
        {
            photoChooserTask.Show();
        }

        private async void ButtonResetImage_Click(object sender, RoutedEventArgs e)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("ImageUri"))
            {
                IsolatedStorageSettings.ApplicationSettings.Remove("ImageUri");
                IsolatedStorageSettings.ApplicationSettings.Save();
                App.MainViewModel.RaisePropertyChanged("ImageUri");
                await RequestPermissions();
            }
        }

        private async void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if(e.ChosenPhoto != null && e.Error == null)
            {
                IsolatedStorageFileStream stream = isoStore.OpenFile(imageFileName, FileMode.OpenOrCreate);
                e.ChosenPhoto.CopyTo(stream);
                stream.Close();
                App.MainViewModel.ImageUri = new Uri("ms-appdata:///Local/" + imageFileName, UriKind.Absolute);
                await RequestPermissions();
            }
        }

        private async Task RequestPermissions()
        {
            bool wasLockscreen = false;
            if (App.MainViewModel.IsLockscreen) wasLockscreen = true;
            if (wasLockscreen) App.MainViewModel.IsLockscreen = false;
            try
            {
                var isProvider = LockScreenManager.IsProvidedByCurrentApplication;
                if (!isProvider)
                {
                    LockScreenRequestResult op = await LockScreenManager.RequestAccessAsync();
                    isProvider = op == LockScreenRequestResult.Granted;
                }
                if (isProvider)
                {
                    LockScreen.SetImageUri(App.MainViewModel.ImageUri);
                    MessageBox.Show(AppResources.SettingBackgroundSuccess);
                }
                else MessageBox.Show(AppResources.SettingBackgroundError);
            }
            catch { MessageBox.Show(AppResources.SettingBackgroundSuccess); }
            if (wasLockscreen) App.MainViewModel.IsLockscreen = true;
        }


    }

}