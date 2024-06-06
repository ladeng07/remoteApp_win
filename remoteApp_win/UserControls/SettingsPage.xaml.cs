using AduSkin.Controls.Metro;
using Microsoft.Win32;
using System;
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
            remoteDesktopSwitch.IsChecked = enable;

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
        }

        private void remoteDesktopTimeChanged(object sender, RoutedEventArgs e)
        {
            //TODO

        }

        private void remoteDesktopTimeLoaded(object sender, RoutedEventArgs e)
        {
            // 获取ComboBox
            AduComboBox comboBox = (AduComboBox)sender;

            // 读取注册表中的值
            int registryValue = ReadRegistryValue();

            // 将注册表的值添加到ComboBox的第一个位置
            comboBox.Items.Insert(0, registryValue);

            // 设置默认选中项为第一个项
            comboBox.SelectedIndex = 0;
        }

        // 从注册表中读取DWORD值的方法
        private int ReadRegistryValue()
        {
            int value = 0;

         
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services");
            if (key != null)
            {
                value = (int)key.GetValue("RemoteAppLogoffTimeLimit", 0);
                key.Close();
            }
            
            return value;
        }

        //查看lxz的server是否存活
        public bool IsProcessRunning(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }
    }
 }
