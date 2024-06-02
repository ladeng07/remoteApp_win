using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using IWshRuntimeLibrary;
using remoteApp_win;
using Newtonsoft.Json;
using System.Windows.Media;
using System.Windows.Input;
using remoteApp_win.UserControls;

namespace IconDisplayApp
{

    public class AppInfo
    {
        public string name { get; set; }
        public string path { get; set; }
    }
    public class AppStackPanel : StackPanel
    {

        public string AppPath;
        public string AppName;


        // 构造函数
        public AppStackPanel() : base()
        {
            // 可以在构造函数中进行一些初始化操作
            AppPath = "";
            AppName = "";
            Background = Brushes.Transparent;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            // 当鼠标进入时改变背景色
            Background = Brushes.LightGray;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            // 当鼠标离开时恢复背景色
            Background = Brushes.Transparent;
        }
    }
    public partial class ShortcutWindow : Window
    {
        public ShortcutWindow(List<string> shortcutFiles)
        {
            InitializeComponent();
            DisplayShortcuts(shortcutFiles);
        }

        private void DisplayShortcuts(List<string> shortcutFiles)
        {
            foreach (var shortcutFile in shortcutFiles)
            {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutFile);

                string iconLocation = shortcut.IconLocation;
                string shortcutName = System.IO.Path.GetFileNameWithoutExtension(shortcutFile);

                Image shortcutImage = new Image();
                shortcutImage.Source = GetBitmapSourceFromIcon(shortcut.TargetPath + ",0");
                shortcutImage.Width = 32;
                shortcutImage.Height = 32;

                shortcutImage.Margin = new Thickness(10);

                TextBlock shortcutText = new TextBlock();
                shortcutText.Text = shortcutName;
                shortcutText.TextWrapping = TextWrapping.Wrap; // 设置自动换行
                shortcutText.TextAlignment = TextAlignment.Center; // 将文本水平对齐设置为居中
                shortcutText.Width= 60;
                shortcutText.MaxHeight = 4 * shortcutText.FontSize;
                shortcutText.TextTrimming = TextTrimming.CharacterEllipsis;
                shortcutText.Margin = new Thickness(10);

                //MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

                AppList myUserControl = new AppList();
                WrapPanel AppWrapPanel = myUserControl.FindName("AppWrapPanel") as WrapPanel;

                AppStackPanel stackPanel = new AppStackPanel();
                stackPanel.Orientation = Orientation.Vertical;
                stackPanel.Children.Add(shortcutImage);
                stackPanel.Children.Add(shortcutText);
                stackPanel.AppPath = shortcut.TargetPath;
                stackPanel.AppName = shortcutName;

                //点击事件
                stackPanel.PreviewMouseLeftButtonUp += (sender, e) =>
                {
                    AppStackPanel clickedStackPanel = sender as AppStackPanel;
                    if (clickedStackPanel != null)
                    {
                        ShortcutWrapPanel.Children.Remove(clickedStackPanel);
                        AppWrapPanel.Children.Add(clickedStackPanel);
                        
                        WriteAppWrapPanelContentsToFile();
                    }
                };

                ShortcutWrapPanel.Children.Add(stackPanel);
            }
        }

        //从图标获取位图
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

        private void WriteAppWrapPanelContentsToFile()
        {
            StringBuilder contents = new StringBuilder();
            //MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            AppList myUserControl = new AppList();
            WrapPanel AppWrapPanel = myUserControl.FindName("AppWrapPanel") as WrapPanel;

            List<AppInfo> appInfoList = new List<AppInfo>();
            // 遍历 AppWrapPanel 中的所有元素，将其内容添加到 StringBuilder 中
            foreach (UIElement element in AppWrapPanel.Children)
            {
                AppStackPanel stackPanel = element as AppStackPanel;
                if (stackPanel != null)
                {
                    AppInfo appInfo = new AppInfo
                        {
                            name = stackPanel.AppName,
                            path = stackPanel.AppPath
                        };

                    appInfoList.Add(appInfo);
                }
            }
            string json = JsonConvert.SerializeObject(appInfoList, Formatting.Indented);
            // 将 StringBuilder 中的内容写入文件
            System.IO.File.WriteAllText(@"D:\list.json", json);
        }
    }
}
