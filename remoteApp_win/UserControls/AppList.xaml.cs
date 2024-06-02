using IconDisplayApp;
using IWshRuntimeLibrary;
using Newtonsoft.Json;
using remoteApp_win.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            LoadAppInfoList();

        }

       

        private void LoadAppInfoList()
        {
            string filePath = @"D:\list.json";
            if (System.IO.File.Exists(filePath))
            {
                string json = System.IO.File.ReadAllText(filePath);
                List<AppInfo> appInfoList = JsonConvert.DeserializeObject<List<AppInfo>>(json);

                foreach (AppInfo appInfo in appInfoList)
                {
                    AppStackPanel stackPanel = new AppStackPanel
                    {
                        AppName = appInfo.Name,
                        AppPath = appInfo.Path,
                        Orientation = Orientation.Vertical
                    };

                    Image shortcutImage = new Image(); // 您需要根据路径获取实际图标
                    shortcutImage.Source = GetBitmapSourceFromIcon(appInfo.Path + ",0");

                    shortcutImage.Width = 32;
                    shortcutImage.Height = 32;

                    shortcutImage.Margin = new Thickness(10);


                    TextBlock shortcutText = new TextBlock();
                    shortcutText.Text = appInfo.Name;
                    shortcutText.TextWrapping = TextWrapping.Wrap; // 设置自动换行
                    shortcutText.TextAlignment = TextAlignment.Center; // 将文本水平对齐设置为居中
                    shortcutText.Width = 60;
                    shortcutText.MaxHeight = 4 * shortcutText.FontSize;
                    shortcutText.TextTrimming = TextTrimming.CharacterEllipsis;
                    shortcutText.Margin = new Thickness(10);

                    stackPanel.Children.Add(shortcutImage);
                    stackPanel.Children.Add(shortcutText);

                    

                    // 添加点击事件
                    //stackPanel.PreviewMouseLeftButtonUp += (sender, e) =>
                    //{
                    //    AppStackPanel clickedStackPanel = sender as AppStackPanel;
                    //    if (clickedStackPanel != null && clickedStackPanel.Parent != AppWrapPanel)
                    //    {
                    //        ShortcutWrapPanel.Children.Remove(clickedStackPanel);
                    //        AppWrapPanel.Children.Add(clickedStackPanel);

                    //        WriteAppWrapPanelContentsToFile();
                    //    }
                    //};

                    AppWrapPanel.Children.Add(stackPanel);
                }
            }
        }

        private BitmapSource GetBitmapSourceFromIcon(string iconPath)
        {
            string[] splitPath = iconPath.Split(',');
            string filePath = splitPath[0];
            int iconIndex = splitPath.Length > 1 ? int.Parse(splitPath[1]) : 0;

            IntPtr hIcon = ExtractIconFromFile(filePath, iconIndex);
            if (hIcon == IntPtr.Zero) return null;

            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                hIcon,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            DestroyIcon(hIcon);
            return bitmapSource;
        }

        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        private IntPtr ExtractIconFromFile(string filePath, int iconIndex)
        {
            return ExtractIcon(IntPtr.Zero, filePath, iconIndex);
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
