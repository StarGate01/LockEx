using System;
using System.Globalization;
using System.ComponentModel;
using LockEx.Resources;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System.Windows.Threading;
using Windows.Phone.System;

namespace LockEx.Models.MusicControl
{

    public class MusicControlView : INotifyPropertyChanged
    {

        #region Maps

        private static Dictionary<MediaState, Uri> PlayIconsMap = new Dictionary<MediaState, Uri>()
        {
            { MediaState.Paused, new Uri("/Assets/Icons/appbar.control.play.png", UriKind.Relative) },
            { MediaState.Playing, new Uri("/Assets/Icons/appbar.control.pause.png", UriKind.Relative) }
        };

        #endregion

        private DispatcherTimer _XNAFrameworkDispatcher;
        public DispatcherTimer XNAFrameworkDispatcher
        {
            get
            {
                return _XNAFrameworkDispatcher;
            }
        }

        private MediaState _playState;
        public MediaState PlayState
        {
            get
            {
                if (DesignerProperties.IsInDesignTool)
                {
                    return _playState;
                }
                else
                {
                    return MediaPlayer.State;
                }
            }
            set
            {
                _playState = value;
                if (!DesignerProperties.IsInDesignTool)
                {
                    switch(value)
                    {
                        case MediaState.Paused:
                            MediaPlayer.Pause();
                            break;
                        case MediaState.Playing:
                            MediaPlayer.Resume();
                            break;
                    }
                }
            }
        }
        public bool HasMusic
        {
            get
            {
                return (PlayState != MediaState.Stopped);
            }
        }
        public Uri MainButtonImageUri
        {
            get
            {
                return PlayIconsMap[PlayState];
            }
        }
        private string _song;
        public string Song
        {
            get
            {
                if (DesignerProperties.IsInDesignTool)
                {
                    return _song;
                }
                else
                {
                    return MediaPlayer.Queue.ActiveSong.Name;
                }
            }
            set
            {
                _song = value;
            }
        }
        private string _artist;
        public string Artist
        {
            get
            {
                if (DesignerProperties.IsInDesignTool)
                {
                    return _artist;
                }
                else
                {
                    return MediaPlayer.Queue.ActiveSong.Artist.Name;
                }
            }
            set
            {
                _artist = value;
            }
        }
        private double _position;
        public double Position
        {
            get
            {
                if (DesignerProperties.IsInDesignTool)
                {
                    return _position;
                }
                else if (MediaPlayer.State != MediaState.Stopped)
                {
                    return MediaPlayer.PlayPosition.TotalSeconds / MediaPlayer.Queue.ActiveSong.Duration.TotalSeconds * 100;
                }
                else return 0;
            }
            set
            {
                _position = value;
            }
        }

        public MusicControlView()
            : this(MediaState.Playing, "", "", 0) { }
        public MusicControlView(MediaState playState, string song, string artist, double position)
        {
            _playState = playState;
            _song = song;
            _artist = artist;
            _position = position;
            _XNAFrameworkDispatcher = new DispatcherTimer();
            _XNAFrameworkDispatcher.Interval = TimeSpan.FromSeconds(0.1);
            _XNAFrameworkDispatcher.Tick += XNAFrameworkDispatcher_Tick;
            MediaPlayer.ActiveSongChanged += MediaPlayer_ActiveSongChanged;
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
        }

        private void XNAFrameworkDispatcher_Tick(object sender, EventArgs e)
        {
            if (!SystemProtection.ScreenLocked) return;
            FrameworkDispatcher.Update();
        }

        public void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("PlayState");
            RaisePropertyChanged("MainButtonImageUri");
            RaisePropertyChanged("HasMusic");
        }

        public void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Song");
            RaisePropertyChanged("Artist");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public MusicControlView GetCopy()
        {
            MusicControlView copy = (MusicControlView)this.MemberwiseClone();
            return copy;
        }

    }

}
