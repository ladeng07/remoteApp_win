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

namespace remoteApp_win.UserControls
{
    /// <summary>
    /// SettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            InitializeComponent();
            bool enable = IsRemoteDesktopEnabled();
            remoteDesktopSwitch.Content = enable ? "启用" : "关闭";
            remoteDesktopSwitch.Foreground = enable ? Brushes.Green : Brushes.Red;
            remoteDesktopSwitch.IsChecked = enable;

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
            remoteDesktopSwitch.Foreground = enabled ? Brushes.Green : Brushes.Red;
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




        //设置远程桌面
        private void AppIcon_Checked(object sender, RoutedEventArgs e)
        {
           
        }

        private void AppIcon_Unchecked(object sender, RoutedEventArgs e)
        {
            
        }

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
