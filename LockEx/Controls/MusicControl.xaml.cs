using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace LockEx.Controls
{

    public partial class MusicControl : UserControl
    {

        public MusicControl()
        {
            InitializeComponent();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            FrameworkDispatcher.Update();
            MediaPlayer.MovePrevious();
        }

        private void ButtonMain_Click(object sender, RoutedEventArgs e)
        {
            FrameworkDispatcher.Update();
            if (App.MainViewModel.MusicView.PlayState == MediaState.Paused) App.MainViewModel.MusicView.PlayState = MediaState.Playing;
            else if (App.MainViewModel.MusicView.PlayState == MediaState.Playing) App.MainViewModel.MusicView.PlayState = MediaState.Paused;
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            FrameworkDispatcher.Update();
            MediaPlayer.MoveNext();
        }

    }

}
