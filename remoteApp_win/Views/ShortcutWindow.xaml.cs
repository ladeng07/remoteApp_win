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
using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;
using AduSkin.Controls.Metro;
using static remoteApp_win.UserControls.AppList;

namespace IconDisplayApp
{
    public class AppInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string IconPath { get; set; }

        public string Icon { get; set; }
    }

    public class AppStackPanel : StackPanel
    {

        public string AppPath;
        private string appName;
        public string AppIconPath;
        public string AppIcon;

        public string AppName
        {
            get => appName;
            set
            {
                appName = value;
                UpdateToolTip();
            }
        }

        // 构造函数
        public AppStackPanel() : base()
        {
            // 可以在构造函数中进行一些初始化操作
            AppPath = "";
            AppName = "";
            AppIconPath = "";
            AppIcon = "";
            Background = Brushes.Transparent;
            ToolTipService.SetInitialShowDelay(this, 1000); // 鼠标悬停3秒后显示
            ToolTipService.SetBetweenShowDelay(this, 0);   // 确保每次都重新计算延迟时间
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

        private void UpdateToolTip()
        {
            ToolTip toolTip = new ToolTip
            {
                Content = new TextBlock { Text = AppName }
            };
            ToolTipService.SetToolTip(this, toolTip);
        }


    }
    public partial class ShortcutWindow : MetroWindow
    {
        public ShortcutWindow(List<List<string>> shortcutFiles)
        {
            InitializeComponent();
            DisplayShortcuts(shortcutFiles);

            // 将传入的参数保存到窗口的实例中
            DataContext = shortcutFiles;

            // 在窗口关闭时释放实例
            Closing += (s, e) => instance = null;
        }
 

        private void DisplayShortcuts(List<List<string>> desktopShortcuts)
        {
            // 存储 WrapPanel 的列表
            List<WrapPanel> wrapPanels = new List<WrapPanel> { ShortcutWrapPanel, ShortcutWrapPanel2 };
            int currentPanelIndex = 0; // 当前使用的 WrapPanel 索引

            foreach (var shortcutFiles in desktopShortcuts)
            {
                foreach (var shortcutFile in shortcutFiles)
                {
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutFile);

                    string iconLocation = shortcut.IconLocation;
                    string shortcutName = System.IO.Path.GetFileNameWithoutExtension(shortcutFile);
                    string TargetPath = shortcut.TargetPath;

                    Image shortcutImage = new Image();

                    // 修正路径
                    if (!System.IO.File.Exists(TargetPath))
                    {
                        TargetPath = FixPath(TargetPath);

                    }

                    string finalLocation = iconLocation.Length > 2 ? iconLocation : TargetPath + ",0";

                   

                    //shortcutImage.Source = GetBitmapSourceFromIcon(shortcut.TargetPath + ",0");
                    shortcutImage.Source = GetBitmapSourceFromIcon(finalLocation);

                    shortcutImage.Width = 32;
                    shortcutImage.Height = 32;

                    shortcutImage.Margin = new Thickness(10);

                    TextBlock shortcutText = new TextBlock();
                    shortcutText.Text = shortcutName;
                    shortcutText.TextWrapping = TextWrapping.Wrap; // 设置自动换行
                    shortcutText.TextAlignment = TextAlignment.Center; // 将文本水平对齐设置为居中
                    shortcutText.Width = 60;
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
                        AppStackPanel_ existingPanel = element as AppStackPanel_;
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
                    stackPanel.AppPath = TargetPath;
                    stackPanel.AppName = shortcutName;
                    stackPanel.AppIconPath = finalLocation;
                    stackPanel.AppIcon = ImageSourceToBase64(shortcutImage.Source);

                    //获取当前桌面下标
                    int temp = currentPanelIndex;
                    //点击事件
                    stackPanel.PreviewMouseLeftButtonUp += (sender, e) =>
                    {
                        AppStackPanel clickedStackPanel = sender as AppStackPanel;
                        if (clickedStackPanel != null)
                        {
                            var currentParent = VisualTreeHelper.GetParent(clickedStackPanel) as Panel;

                            // 如果当前父级是 AppWrapPanel，则不执行后续操作
                            if (currentParent == AppWrapPanel)
                            {   //TODO:再添加完之后的双击打开事件？好像没必要
                                //TODO:之后给图标添加其他点击事件
                                return;
                            }

                            //删除指定桌面的的APP
                            wrapPanels[temp].Children.Remove(clickedStackPanel);

                            AppStackPanel_ childdStackPanel = new AppStackPanel_();
                            childdStackPanel.AppPath = clickedStackPanel.AppPath;
                            childdStackPanel.AppName = clickedStackPanel.AppName;
                            childdStackPanel.AppIconPath = clickedStackPanel.AppIconPath;
                            childdStackPanel.AppIcon = clickedStackPanel.AppIcon;
                            for (int i = 0; i < 2; i++) {
                                FrameworkElement originalChild = clickedStackPanel.Children[0] as FrameworkElement;
                                clickedStackPanel.Children.Remove(originalChild);
                                childdStackPanel.Children.Add(originalChild);

                            }

                            // 获取 clickedStackPanel 的父元素
                            Panel parentPanel = clickedStackPanel.Parent as Panel;
                            if (parentPanel != null)
                            {
                                // 从父元素中移除 clickedStackPanel
                                parentPanel.Children.Remove(clickedStackPanel);
                            }

                            AppWrapPanel.Children.Add(childdStackPanel);

                            //刷新list.json
                            WriteAppWrapPanelContentsToFile();
                        }
                    };

                    wrapPanels[temp].Children.Add(stackPanel);
                    
                }
                currentPanelIndex++;
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
                            Path = stackPanel.AppPath,
                            IconPath = stackPanel.AppIconPath,
                            Icon = stackPanel.AppIcon
                    };

                    appInfoList.Add(appInfo);
                }
            }
            string json = JsonConvert.SerializeObject(appInfoList, Formatting.Indented);
            // 将 StringBuilder 中的内容写入文件

            string fileName = "list.json";
            System.IO.File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName), json);
        }

        //将图片转换为base64
        private string ImageSourceToBase64(ImageSource imageSource)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                if (imageSource == null)
                {   // TODO
                    // 当ImageSource为空时，返回一个空字符串或者其他默认处理
                    return string.Empty; // 或者返回一个默认图片的Base64编码
                }
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSource));
                encoder.Save(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        private static ShortcutWindow instance;

        public static ShortcutWindow Instance(List<List<string>> shortcutFilesList)
        {
            if (instance == null)
            {
                instance = new ShortcutWindow(shortcutFilesList);
            }
            return instance;
        }

        public static void ShowWindow(List<List<string>> shortcutFilesList)
        {
            var window = Instance(shortcutFilesList);
            if (!window.IsVisible)
            {
                window.Show();
            }
            else
            {
                window.Activate(); // 如果窗口已经显示，则激活窗口
            }
        }


        private string FixPath(string targetPath)
        {
            if (targetPath.Contains("Program Files (x86)"))
            {
                string newPath = targetPath.Replace("Program Files (x86)", "Program Files");
                if (System.IO.File.Exists(newPath))
                {
                    return newPath;
                }
            }
            return targetPath;
        }
    }
}
