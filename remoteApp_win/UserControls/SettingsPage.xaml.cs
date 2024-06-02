using Microsoft.Win32;
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
        }
    }
}
