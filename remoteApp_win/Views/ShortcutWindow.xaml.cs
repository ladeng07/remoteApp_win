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
using remoteApp_win.ViewModel;

namespace IconDisplayApp
{
    public class AppInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
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
            LoadAppInfoList();
            DisplayShortcuts(shortcutFiles);
        }

        //加载已有App列表
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

                    //获取MainWindow实例
                    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

                    //获取数据上下文
                    MainViewModel viewModel = mainWindow.DataContext as MainViewModel;
                    UserControl myUserControl = viewModel.Page1;
                    WrapPanel AppWrapPanel = myUserControl.FindName("AppWrapPanel") as WrapPanel;

                    // 添加点击事件
                    stackPanel.PreviewMouseLeftButtonUp += (sender, e) =>
                    {
                        AppStackPanel clickedStackPanel = sender as AppStackPanel;
                        if (clickedStackPanel != null && clickedStackPanel.Parent != AppWrapPanel)
                        {
                            ShortcutWrapPanel.Children.Remove(clickedStackPanel);
                            AppWrapPanel.Children.Add(clickedStackPanel);

                            WriteAppWrapPanelContentsToFile();
                        }
                    };

                    AppWrapPanel.Children.Add(stackPanel);
                }
            }
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

                string finalLocation = iconLocation.Length > 2 ? iconLocation : shortcut.TargetPath + ",0";
                //shortcutImage.Source = GetBitmapSourceFromIcon(shortcut.TargetPath + ",0");
                shortcutImage.Source = GetBitmapSourceFromIcon(finalLocation);

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

                //获取MainWindow实例
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                
                //获取数据上下文
                MainViewModel viewModel = mainWindow.DataContext as MainViewModel;
                UserControl myUserControl = viewModel.Page1;
                WrapPanel AppWrapPanel = myUserControl.FindName("AppWrapPanel") as WrapPanel;

                // 检查 AppWrapPanel 是否已包含具有相同名称和路径的元素
                bool exists = false;
                foreach (UIElement element in AppWrapPanel.Children)
                {
                    AppStackPanel existingPanel = element as AppStackPanel;
                    if (existingPanel != null && existingPanel.AppName == shortcutName && existingPanel.AppPath == shortcut.TargetPath)
                    {
                        exists = true;
                        break;
                    }
                }

                if (exists)
                {
                    // 如果已存在具有相同名称和路径的元素，则跳过
                    continue;
                }

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
                        var currentParent = VisualTreeHelper.GetParent(clickedStackPanel) as Panel;

                        // 如果当前父级是 AppWrapPanel，则不执行后续操作
                        if (currentParent == AppWrapPanel)
                        {   
                            //TODO:之后给图标添加其他点击事件
                            return;
                        }

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

            //获取MainWindow实例
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            //获取数据上下文
            MainViewModel viewModel = mainWindow.DataContext as MainViewModel;
            UserControl myUserControl = viewModel.Page1;
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
                            Name = stackPanel.AppName,
                            Path = stackPanel.AppPath
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
