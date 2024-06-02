using remoteApp_win.UserControls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace remoteApp_win.ViewModel
{
    public class MainViewModel : ViewModelBase 
    {
        public MainViewModel()
        {
        }

        private int _SelectedModularIndex;

        public int SelectedModularIndex
        {
            get { return _SelectedModularIndex; }
            set
            {
                Set(ref _SelectedModularIndex, value);
                if (value == 2)
                    MainBackground = new SolidColorBrush(Color.FromRgb(28, 64, 139));
                else if (value == 3)
                    MainBackground = new SolidColorBrush(Color.FromRgb(250, 251, 252));
                else
                    MainBackground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }

        private SolidColorBrush _MainBackground = new SolidColorBrush(Color.FromRgb(255, 255, 255));

        public SolidColorBrush MainBackground
        {
            get { return _MainBackground; }
            set { Set(ref _MainBackground, value); }
        }

        private UserControl _Page1 = new AppList();
        public UserControl Page1
        {
            get { return _Page1; }
            set { Set(ref _Page1, value); }
        }

        private UserControl _Settings = new SettingsPage();
        public UserControl Settings
        {
            get { return _Settings; }
            set { Set(ref _Settings, value); }
        }

    }

    public class TabItemViewModel
    {
        public string Header { get; set; }
        public string Content { get; set; }
    }
}
