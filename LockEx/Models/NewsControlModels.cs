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
using System.Net.Http;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;

namespace LockEx.Models.NewsControl
{

    public class NewsControlEntry : INotifyPropertyChanged
    {

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set 
            {
                _title = value;
                RaisePropertyChanged("Title");
            }
        }
        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                RaisePropertyChanged("Description");
            }
        }
        private Uri _link;
        public Uri Link
        {
            get
            {
                return _link;
            }
            set
            {
                _link = value;
                RaisePropertyChanged("Link");
            }
        }
        private DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                RaisePropertyChanged("Date");
            }
        }

        public NewsControlEntry(string title, string description, Uri link, DateTime date)
        {
            _title = title;
            _description = description;
            _link = link;
            _date = date;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public NewsControlEntry GetCopy()
        {
            NewsControlEntry copy = (NewsControlEntry)this.MemberwiseClone();
            return copy;
        }

    }

    public class NewsControlView : INotifyPropertyChanged
    {

        private ObservableCollection<NewsControlEntry> _entries;
        public ObservableCollection<NewsControlEntry> Entries
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

        private Uri _source;
        public Uri Source
        {
            get
            {
                if (DesignerProperties.IsInDesignTool) return _source;
                return (IsolatedStorageSettings.ApplicationSettings.Contains("Source")) ?
                    new Uri((string)IsolatedStorageSettings.ApplicationSettings["Source"]) :
                    new Uri(AppResources.DefaultNewsSource);
            }
            set
            {
                _source = value;
                if (!DesignerProperties.IsInDesignTool)
                {
                    IsolatedStorageSettings.ApplicationSettings["Source"] = value.AbsoluteUri;
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
            }
        }
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                RaisePropertyChanged("Title");
            }
        }
        private Visibility _loadingVisible;
        public Visibility LoadingVisible
        {
            get
            {
                return _loadingVisible;
            }
            set
            {
                _loadingVisible = value;
                RaisePropertyChanged("LoadingVisible");
                RaisePropertyChanged("UIVisible");
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
                return (_errorVisible == Visibility.Visible || _loadingVisible == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private const string RFC822 = "ddd, dd MMM yyyy HH:mm:ss zzz";

        public NewsControlView()
            : this(new ObservableCollection<NewsControlEntry>(), null) { }
        public NewsControlView( ObservableCollection<NewsControlEntry> entries, Uri source)
        {
            _entries = entries;
            _source = source;
        }

        public async Task PopulateData()
        {
            ErrorVisible = Visibility.Collapsed;
            LoadingVisible = Visibility.Visible;
            try
            {
                HttpClient client = new HttpClient();
                String resString = await client.GetStringAsync(Source.AbsoluteUri);
                LoadingVisible = Visibility.Collapsed;
                XElement document = XElement.Parse(resString).Element("channel");
                Title = document.Element("title").Value;
                IEnumerable<XElement> items = document.Descendants("item");
                Entries.Clear();
                foreach(XElement item in items)
                {
                    DateTime dt = DateTime.ParseExact(item.Element("pubDate").Value, RFC822, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
                    string title = Regex.Replace(item.Element("title").Value, "<.*?>", String.Empty);
                    string description = Regex.Replace(item.Element("description").Value, "<.*?>", String.Empty);
                    Entries.Add(new NewsControlEntry(title, description, new Uri(item.Element("link").Value), dt));
                }
            }
            catch
            {
                LoadingVisible = Visibility.Collapsed;
                ErrorVisible = Visibility.Visible;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public NewsControlView GetCopy()
        {
            NewsControlView copy = (NewsControlView)this.MemberwiseClone();
            return copy;
        }

    }

}
