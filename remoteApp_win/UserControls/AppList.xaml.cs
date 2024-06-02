using IconDisplayApp;
using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace remoteApp_win.UserControls
{
    /// <summary>
    /// AppList.xaml 的交互逻辑
    /// </summary>
    public partial class AppList : UserControl
    {
        public AppList()
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

        private void OpenWeChatButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 微信可执行文件路径
                string weChatPath = @"C:\Users\Public\Desktop\微信.lnk";

                // 启动微信应用程序
                Process.Start(new ProcessStartInfo(weChatPath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("无法启动微信应用程序: " + ex.Message);
            }
        }

        private void ScanAndDisplayShortcuts(object sender, RoutedEventArgs e)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            List<string> shortcutFiles = new List<string>();

            foreach (var file in Directory.GetFiles(desktopPath, "*.lnk"))
            {
                string targetPath = GetShortcutTarget(file);
                FileInfo fileInfo = new FileInfo(targetPath);


                // 检查目标路径是否为文件
                if (fileInfo.Exists && !fileInfo.Attributes.HasFlag(FileAttributes.Directory))
                {
                    shortcutFiles.Add(file);
                }

            }

            ShortcutWindow shortcutWindow = new ShortcutWindow(shortcutFiles);
            shortcutWindow.Show();
        }

        private string GetShortcutTarget(string shortcutPath)
        {
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            return shortcut.TargetPath;
        }
    }
}
