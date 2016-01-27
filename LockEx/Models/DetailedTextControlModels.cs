using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace LockEx.Models.DetailedTextControl
{

    public class DetailedTextControlEntry : INotifyPropertyChanged
    {

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
            }
        }
        private bool _bold;
        public bool Bold
        {
            get
            {
                return _bold;
            }
            set
            {
                _bold = value;
                RaisePropertyChanged("Bold");
                RaisePropertyChanged("FontWeight");
            }
        }
        public FontWeight FontWeight
        {
            get
            {
                return Bold ? FontWeights.SemiBold : FontWeights.Normal;
            }
        }
        private bool _wrap;
        public bool Wrap
        {
            get
            {
                return _wrap;
            }
            set
            {
                _wrap = value;
                RaisePropertyChanged("Wrap");
                RaisePropertyChanged("TextWrapping");
            }
        }
        public TextWrapping TextWrapping
        {
            get
            {
                return Wrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
            }
        }

        public DetailedTextControlEntry(string text, bool bold, bool wrap)
        {
            _text = text;
            _bold = bold;
            _wrap = wrap;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public DetailedTextControlEntry GetCopy()
        {
            DetailedTextControlEntry copy = (DetailedTextControlEntry)this.MemberwiseClone();
            return copy;
        }

    }

    public class DetailedTextControlView : INotifyPropertyChanged
    {

        private ObservableCollection<DetailedTextControlEntry> _entries;
        public ObservableCollection<DetailedTextControlEntry> Entries
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

        public DetailedTextControlView()
            : this(new ObservableCollection<DetailedTextControlEntry>()) { }
        public DetailedTextControlView(ObservableCollection<DetailedTextControlEntry> entries)
        {
            _entries = entries;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public DetailedTextControlView GetCopy()
        {
            DetailedTextControlView copy = (DetailedTextControlView)this.MemberwiseClone();
            return copy;
        }

    }

}
