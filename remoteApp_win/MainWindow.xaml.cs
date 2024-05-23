using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace remoteApp_win
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        

        private void OpenDesktopButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取桌面路径
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // 使用Process启动资源管理器并打开桌面路径
            Process.Start(new ProcessStartInfo
            {
                FileName = desktopPath,
                UseShellExecute = true
            });
        }

        private void WrapPanel_DragEnter(object sender, DragEventArgs e)
        {
            //拖拽事件
            
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void WrapPanel_Drop(object sender, DragEventArgs e)
        {
            //拖拽事件
            
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    // 这里处理拖入的文件
                    MessageBox.Show("文件拖入: " + file);
                }
            }
        }


    }
}
