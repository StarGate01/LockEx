using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Windows.System;
using Windows.Phone.System;

namespace LockEx.Controls
{
    public partial class NewsControl : UserControl
    {
        public NewsControl()
        {
            InitializeComponent();
        }

        private void stackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
           //SystemProtection.RequestScreenUnlock();
           Launcher.LaunchUriAsync((Uri)((StackPanel)sender).Tag);
        }
    }
}
