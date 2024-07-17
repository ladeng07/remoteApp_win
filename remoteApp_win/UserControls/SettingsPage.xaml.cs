using AduSkin.Controls.Metro;
using Microsoft.Win32;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Web.UI.WebControls;
using IconDisplayApp;
using Newtonsoft.Json;
using System.Drawing;
using remoteApp_win.ViewModel;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace remoteApp_win.UserControls
{
    /// <summary>
    /// SettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        private readonly string configFilePath = "config.ini";
        bool isRemoteTagEnabled;
        public SettingsPage()
        {
            InitializeComponent();
            bool enable = IsRemoteDesktopEnabled();
            remoteDesktopSwitch.Content = enable ? "启用" : "关闭";
            remoteDesktopSwitch.Foreground = enable ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
            remoteDesktopSwitch.IsChecked = enable;
            LoadConfigSettings();

            //enable = IsUACEnabled();
            //UACSwitch.Content = enable ? "启用" : "关闭";
            //UACSwitch.Foreground = enable ? Brushes.Green : Brushes.Red;
            //UACSwitch.IsChecked = enable;

            remoteDesktopTimeLoaded();
        }


        //打开设置
        public void OpenRemoteDesktopSettings(object sender, RoutedEventArgs e)
        {
            Process.Start("ms-settings:remotedesktop");
        }

        private void remoteDesktopSwitch_Checked(object sender, RoutedEventArgs e)
        {
            // 启用远程桌面
            SetRemoteDesktopEnabled(true);
        }

        private void remoteDesktopSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            // 禁用远程桌面
            SetRemoteDesktopEnabled(false);
        }

        // 检查远程桌面是否已启用
        private bool IsRemoteDesktopEnabled()
        {
            // 远程桌面的设置保存在注册表中的以下位置
            string keyPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Terminal Server";

            // 读取 "fDenyTSConnections" 键的值
            int value = (int)Registry.GetValue(keyPath, "fDenyTSConnections", null);

            // 如果值为 0，则远程桌面已启用；如果值为 1，则远程桌面已禁用
            return value == 0;
        }

        // 设置远程桌面状态
        private void SetRemoteDesktopEnabled(bool enabled)
        {
            // 远程桌面的设置保存在注册表中的以下位置
            string keyPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Terminal Server";

            // 设置 "fDenyTSConnections" 键的值为 0 或 1
            Registry.SetValue(keyPath, "fDenyTSConnections", enabled ? 0 : 1);

            remoteDesktopSwitch.Content = enabled ? "启用" : "关闭";
            remoteDesktopSwitch.Foreground = enabled ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
        }




        //会话时长
        private void remoteDesktopTimeChanged(object sender, RoutedEventArgs e)
        {
            //TODO 修改最长RDP断连时间

        }

        private void remoteDesktopTimeLoaded()
        {

            // 读取注册表中的值
            int registryValue = ReadRegistryValue();

            // 将注册表的值添加到ComboBox的第一个位置
            remoteDesktopTime.Items.Insert(0, registryValue);

            // 设置默认选中项为第一个项
            remoteDesktopTime.SelectedIndex = 0;
        }

        // 从注册表中读取DWORD值的方法
        private int ReadRegistryValue()
        {
            int value = 0;

         
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services");
            if (key != null)
            {
                value = (int)key.GetValue("MaxDisconnectionTime", 0);
                key.Close();
            }
            
            return value;
        }




        private void LoadConfigSettings()
        {
            if (File.Exists(configFilePath))
            {
                var lines = File.ReadAllLines(configFilePath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("RemoteTagEnabled"))
                    {
                        var value = line.Split('=')[1].Trim();
                        AppIconSwitch.IsChecked = value == "1";
                        isRemoteTagEnabled = value == "1";
                        break;
                    }
                }
            }
        }

        private void UpdateConfigFile(bool isEnabled)
        {
            if (File.Exists(configFilePath))
            {
                var lines = File.ReadAllLines(configFilePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("RemoteTagEnabled"))
                    {
                        lines[i] = $"RemoteTagEnabled = {(isEnabled ? 1 : 0)}";
                        isRemoteTagEnabled = isEnabled;
                        break;
                    }
                }
                File.WriteAllLines(configFilePath, lines);
            }
        }

        private void AppIcon_Checked(object sender, RoutedEventArgs e)
        {
            UpdateConfigFile(true);
            FlushAppWrapPanelContentsToFile();
        }

        private void AppIcon_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateConfigFile(false);
            FlushAppWrapPanelContentsToFile();
        }

        private void FlushAppWrapPanelContentsToFile()
        {
            StringBuilder contents = new StringBuilder();

            //获取MainWindow实例
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            //获取数据上下文
            MainViewModel viewModel = mainWindow.DataContext as MainViewModel;
            if (viewModel == null) return;
            UserControl myUserControl = viewModel.Page1;
            WrapPanel AppWrapPanel = myUserControl.FindName("AppWrapPanel") as WrapPanel;

            List<AppInfo> appInfoList = new List<AppInfo>();
            // 遍历 AppWrapPanel 中的所有元素，将其内容添加到 StringBuilder 中
            foreach (UIElement element in AppWrapPanel.Children)
            {
                AppStackPanel stackPanel = element as AppStackPanel;
                if (stackPanel != null)
                {
                    //选中文件中的图标总数
                    var iconTotalCount = PrivateExtractIcons(stackPanel.AppPath, 0, 0, 0, null, null, 0, 0);

                    //用于接收获取到的图标指针
                    IntPtr[] hIcons = new IntPtr[iconTotalCount];
                    ////对应的图标id
                    int[] ids = new int[iconTotalCount];
                    ////成功获取到的图标个数
                    var successCount = PrivateExtractIcons(stackPanel.AppPath, 0, 64, 64, hIcons, null, iconTotalCount, 0);


                    Icon icon; //判断exe还是ico
                    BitmapSource bitmapSource;
                    if (hIcons.Length > 0)
                    { //添加exe的
                        icon = System.Drawing.Icon.FromHandle(hIcons[0]);
                        bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            icon.Handle,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromWidthAndHeight(64, 64));
                    }
                    else
                    { //添加ico
                        bitmapSource = GetBitmapSourceFromIcon(stackPanel.AppIconPath);
                    }

                    AppInfo appInfo;
                    if (isRemoteTagEnabled)
                    {
                        //给远程应用添加右下角角标
                        BitmapSource iconSource = new BitmapImage(new Uri("pack://application:,,,/logo.png")); // Replace with your icon path
                        BitmapSource combinedBitmap = AddIconToBitmap(bitmapSource, iconSource, 24); // 24 icon size

                        stackPanel.AppIcon = ImageSourceToBase64(combinedBitmap);
                        appInfo = new AppInfo
                        {
                            Name = stackPanel.AppName,
                            Path = stackPanel.AppPath,
                            IconPath = stackPanel.AppIconPath,
                            Icon = ImageSourceToBase64(combinedBitmap)
                        };

                    }
                    else
                    {
                        appInfo = new AppInfo
                        {
                            Name = stackPanel.AppName,
                            Path = stackPanel.AppPath,
                            IconPath = stackPanel.AppIconPath,
                            Icon = ImageSourceToBase64(bitmapSource)
                        };
                    }


                    appInfoList.Add(appInfo);
                }
            }
            string json = JsonConvert.SerializeObject(appInfoList, Formatting.Indented);
            // 将 StringBuilder 中的内容写入文件

            string fileName = "list.json";
            System.IO.File.WriteAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName), json);
        }

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

        private BitmapSource GetBitmapSourceFromIcon(string iconPath)
        {
            string[] splitPath = iconPath.Split(',');
            string filePath = splitPath[0];
            try
            {//防止传入非法的地址
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
            catch (Exception) { return null; }

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

        public static BitmapSource AddIconToBitmap(BitmapSource bitmapSource, BitmapSource iconSource, int iconSize)
        {
            if (bitmapSource == null || iconSource == null)
            {   // TODO
                // 当bitmapSource为空时，返回一个空或者其他默认处理
                return null;
            }
            int width = bitmapSource.PixelWidth;
            int height = bitmapSource.PixelHeight;

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                // Draw the original bitmap
                context.DrawImage(bitmapSource, new Rect(0, 0, width, height));

                // Draw the icon in the right bottom corner
                context.DrawImage(iconSource, new Rect(width - iconSize, height - iconSize, iconSize, iconSize));
            }

            RenderTargetBitmap result = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            result.Render(visual);

            return result;
        }

        [DllImport("User32.dll")]
        public static extern int PrivateExtractIcons(
            string lpszFile, //file name
            int nIconIndex,  //The zero-based index of the first icon to extract.
            int cxIcon,      //The horizontal icon size wanted.
            int cyIcon,      //The vertical icon size wanted.
            IntPtr[] phicon, //(out) A pointer to the returned array of icon handles.
            int[] piconid,   //(out) A pointer to a returned resource identifier.
            int nIcons,      //The number of icons to extract from the file. Only valid when *.exe and *.dll
            int flags        //Specifies flags that control this function.
        );

        //private void ModifyUAC(bool enable)
        //{
        //    try
        //    {
        //        string keyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
        //        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath, true))
        //        {
        //            if (key != null)
        //            {
        //                key.SetValue("EnableLUA", enable ? 1 : 0, RegistryValueKind.DWord);

        //                //UACSwitch.Content = enable ? "启用" : "关闭";
        //                //UACSwitch.Foreground = enable ? Brushes.Green : Brushes.Red;
        //            }
        //            else
        //            {
        //                throw new InvalidOperationException("无法打开注册表项。");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AduMessageBox.Show($"修改注册表时出错: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private bool IsUACEnabled()
        //{
        //    // UAC的设置保存在注册表中的以下位置
        //    string keyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";

        //    // 读取 "EnableLUA" 键的值
        //    object value = Registry.GetValue(keyPath, "EnableLUA", null);

        //    // 将值转换为int类型
        //    int enableLUAValue = (int)value;

        //    // 返回是否UAC已禁用
        //    return enableLUAValue == 1;
        //}


        //重启
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = AduMessageBox.Show("确定要重启吗？", "确认重启", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                // 执行重启操作
                Process.Start("shutdown", "/r /t 0");
            }
        }

        //关机
        private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = AduMessageBox.Show("确定要关机吗？", "确认关机", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                // 执行关机操作
                Process.Start("shutdown", "/s /t 0");
            }
        }



        //查看lxz的server是否存活
        public bool IsProcessRunning(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }
    }
 }
