using AduSkin.Controls.Metro;
using IconDisplayApp;
using IWshRuntimeLibrary;
using Newtonsoft.Json;
using remoteApp_win.ViewModel;
using Microsoft.Win32;
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
using Icon = System.Drawing.Icon;


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

        public class AppStackPanel_ : AppStackPanel
        {

            // 构造函数
            public AppStackPanel_() : base()
            {
                // 订阅 MouseLeftButtonUp 事件
                MouseLeftButtonUp += DoubleClickStackPanel_MouseLeftButtonUp;

                // 创建右键菜单
                MetroContextMenu rightClickMenu = new MetroContextMenu();
                
                // 添加菜单项

                //设置
                MetroMenuItem menuSetting = new MetroMenuItem();
                menuSetting.Header = "设置";
                menuSetting.Foreground = Brushes.Black;
                menuSetting.Click += MenuItem_Setting_Click;
                rightClickMenu.Items.Add(menuSetting);

                //分割线
                MetroMenuSeparator menuSeparator = new MetroMenuSeparator();
                rightClickMenu.Items.Add(menuSeparator);

                //删除
                MetroMenuItem menuDel = new MetroMenuItem();
                menuDel.Header = "删除";
                menuDel.Click += MenuItem_Del_Click;
                rightClickMenu.Items.Add(menuDel);


                // 将右键菜单
                ContextMenu = rightClickMenu;
            }


            //添加并绑定双击事件
            private const int DoubleClickTimeThreshold = 500; // 定义双击时间阈值（毫秒）
            private DateTime lastClickTime = DateTime.MinValue;

            public event EventHandler DoubleClick;

            private void DoubleClickStackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
                DateTime now = DateTime.Now;
                TimeSpan timeSinceLastClick = now - lastClickTime;
                lastClickTime = now;

                // 如果两次点击的时间间隔小于等于阈值，则认为是双击事件
                if (timeSinceLastClick.TotalMilliseconds <= DoubleClickTimeThreshold)
                {
                    OnDoubleClick();
                }
            }

            protected virtual void OnDoubleClick()
            {
                // 处理右键菜单中的 "Open" 菜单项点击事件
                AppStackPanel_ clickedStackPanel = this as AppStackPanel_;
                if (clickedStackPanel != null)
                {
                    Process.Start(new ProcessStartInfo(clickedStackPanel.AppPath) { UseShellExecute = true });
                    // 如果需要在打开时执行其他操作，可以在这里添加代码
                }
            }


            private void MenuItem_Setting_Click(object sender, RoutedEventArgs e)
            {
                //TODO
            }

            private void MenuItem_Del_Click(object sender, RoutedEventArgs e)
            {
                AppStackPanel_ clickedStackPanel = this as AppStackPanel_;
                Panel parentPanel = clickedStackPanel.Parent as Panel;
                if (clickedStackPanel != null)
                {
                    parentPanel.Children.Remove(clickedStackPanel);
                }
            }
        }

        //从文件打开
        private void LoadAppInfoList()
        {
            string fileName = "list.json";
            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            if (System.IO.File.Exists(filePath))
            {
                string json = System.IO.File.ReadAllText(filePath);
                List<AppInfo> appInfoList = JsonConvert.DeserializeObject<List<AppInfo>>(json);

                foreach (AppInfo appInfo in appInfoList)
                {
                    AppStackPanel_ stackPanel = new AppStackPanel_
                    {
                        AppName = appInfo.Name,
                        AppPath = appInfo.Path,
                        AppIconPath = appInfo.IconPath,
                        Orientation = Orientation.Vertical
                    };

                    Image shortcutImage = new Image(); // 您需要根据路径获取实际图标
                    shortcutImage.Source = GetBitmapSourceFromIcon(appInfo.IconPath);

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
                    stackPanel.DoubleClick += (sender, e) =>
                    { 
                            AppStackPanel_ clickedStackPanel = sender as AppStackPanel_;
                            Process.Start(new ProcessStartInfo(clickedStackPanel.AppPath) { UseShellExecute = true });
                    };


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

        //从安装目录添加
        private void OpenWeChatButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "可执行文件 (*.exe;*.lnk)|*.exe;*.lnk|所有文件 (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                // 检查文件扩展名是否为 .exe 或 .lnk
                string extension = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
                if (extension.Equals(".exe") || extension.Equals(".lnk"))
                {
                    if (extension.Equals(".lnk")) {
                        WshShell shell_ = new WshShell();
                        IWshShortcut shortcut_ = (IWshShortcut)shell_.CreateShortcut(filePath);
                        filePath = shortcut_.TargetPath;

                    }
                    // 检查 AppWrapPanel 是否已包含具有相同名称和路径的元素
                    bool exists = false;
                    foreach (UIElement element in AppWrapPanel.Children)
                    {
                        AppStackPanel_ existingPanel = element as AppStackPanel_;
                        if (existingPanel != null && existingPanel.AppPath == filePath)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (exists) MessageBox.Show($"该应用已存在");
                    else
                    {

                        Image shortcutImage = new Image();

                        string shortcutName = System.IO.Path.GetFileNameWithoutExtension(filePath);

                        // Extract icon from EXE file
                        Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
                        //TODO:打开Docker会找不到
                        BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            icon.Handle,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromWidthAndHeight(32, 32));

                        shortcutImage.Source = bitmapSource;

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

                        AppStackPanel_ stackPanel = new AppStackPanel_();
                        stackPanel.Orientation = Orientation.Vertical;
                        stackPanel.Children.Add(shortcutImage);
                        stackPanel.Children.Add(shortcutText);
                        stackPanel.AppPath = filePath;
                        stackPanel.AppName = shortcutName;
                        stackPanel.AppIconPath = filePath;//TODO
                        stackPanel.AppIcon = ImageSourceToBase64(shortcutImage.Source);

                        // 添加点击事件
                        stackPanel.DoubleClick += (sender_, e_) =>
                        {
                            AppStackPanel_ clickedStackPanel = sender as AppStackPanel_;
                            Process.Start(new ProcessStartInfo(clickedStackPanel.AppPath) { UseShellExecute = true });
                        };


                        AppWrapPanel.Children.Add(stackPanel);

                        //刷新文件
                        WriteAppWrapPanelContentsToFile();
                    }
                }
                else
                {
                    MessageBox.Show($"请选择以 .exe 或 .lnk 结尾的文件！");
                }
            }
        }

        private void ScanAndDisplayShortcuts(object sender, RoutedEventArgs e)
        {
            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string desktopPath = "C:\\Users\\Public\\Desktop";
            //TODO：很多桌面路径

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

        private string ImageSourceToBase64(ImageSource imageSource)
        {
            if (imageSource != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSource));
                    encoder.Save(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();
                    return Convert.ToBase64String(imageBytes);
                }
            }
            else return null;
        }

        private void WriteAppWrapPanelContentsToFile()
        {
            StringBuilder contents = new StringBuilder();

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
            System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName), json);
        }
    }
}
