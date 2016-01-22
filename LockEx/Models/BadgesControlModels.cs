using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows;

namespace LockEx.Models.BadgesControl
{

    public class BadgesControlEntry : INotifyPropertyChanged
    {

        private BitmapImage _image;
        public BitmapImage Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                RaisePropertyChanged("Image");
            }
        }
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                RaisePropertyChanged("Text");
                RaisePropertyChanged("Visible");
            }
        }
        public Visibility Visible
        {
            get
            {
                return (Text != "") ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public BadgesControlEntry(BitmapImage image, string text)
        {
            _image = image;
            _text = text;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public BadgesControlEntry GetCopy()
        {
            BadgesControlEntry copy = (BadgesControlEntry)this.MemberwiseClone();
            return copy;
        }

    }

    public class BadgesControlView : INotifyPropertyChanged
    {

        private ObservableCollection<BadgesControlEntry> _entries;
        public ObservableCollection<BadgesControlEntry> Entries
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

        public BadgesControlView()
            : this(new ObservableCollection<BadgesControlEntry>()) { }
        public BadgesControlView(ObservableCollection<BadgesControlEntry> entries)
        {
            _entries = entries;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public BadgesControlView GetCopy()
        {
            BadgesControlView copy = (BadgesControlView)this.MemberwiseClone();
            return copy;
        }

    }

}
