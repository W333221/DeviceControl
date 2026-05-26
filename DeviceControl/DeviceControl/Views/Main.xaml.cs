using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeviceControl.Views
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : UserControl
    {
        private readonly IRegionManager _regionManager;

        public Main(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _regionManager.RequestNavigate("ContentRegion", "DeviceLog");
            //if (sideMenu.Width == 0)
            //{
            //    sideMenu.BeginAnimation(WidthProperty, new DoubleAnimation(90, TimeSpan.FromSeconds(0.2)));
            //}
        }

        private void ButtonShowMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sideMenu.Width == 0)
            {
                sideMenu.BeginAnimation(WidthProperty, new DoubleAnimation(90, TimeSpan.FromSeconds(0.2)));
            }
            else
            {
                sideMenu.BeginAnimation(WidthProperty, new DoubleAnimation(0, TimeSpan.FromSeconds(0.2)));

            }

        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            sideMenu.BeginAnimation(WidthProperty, new DoubleAnimation(0, TimeSpan.FromSeconds(0.2)));
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            sideMenu.BeginAnimation(WidthProperty, new DoubleAnimation(0, TimeSpan.FromSeconds(0.2)));
        }

    }
}
