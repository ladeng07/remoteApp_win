using IconDisplayApp;
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
using AduSkin.Controls.Metro;


using NotifyIconWPF = System.Windows.Forms.NotifyIcon;
using IWshRuntimeLibrary;

namespace remoteApp_win
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private NotifyIconWPF _notifyIcon;
        public MainWindow()
        {
            InitializeComponent();

            // 创建托盘图标
            _notifyIcon = new NotifyIconWPF();
            _notifyIcon.Icon = new System.Drawing.Icon("Resources/logo.ico"); // 替换为您的图标路径
            _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            _notifyIcon.Visible = true;

            // 创建托盘图标的右键菜单
            System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MenuItem restoreMenuItem = new System.Windows.Forms.MenuItem("恢复");
            restoreMenuItem.Click += RestoreMenuItem_Click;
            contextMenu.MenuItems.Add(restoreMenuItem);
            System.Windows.Forms.MenuItem exitMenuItem = new System.Windows.Forms.MenuItem("退出");
            exitMenuItem.Click += ExitMenuItem_Click;
            contextMenu.MenuItems.Add(exitMenuItem);

            _notifyIcon.ContextMenu = contextMenu;
        }

        //托盘图标
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 将窗口最小化到托盘
            e.Cancel = true;
            Hide();
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            // 双击托盘图标时恢复窗口
            Show();
            WindowState = WindowState.Normal;
        }

        private void RestoreMenuItem_Click(object sender, EventArgs e)
        {
            // 右键菜单中的“恢复”选项
            Show();
            WindowState = WindowState.Normal;
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            // 右键菜单中的“退出”选项
            _notifyIcon.Dispose();
            Close();
        }

        

        //首页

        //private void OpenDesktopButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // 获取桌面路径
        //    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        //    // 使用Process启动资源管理器并打开桌面路径
        //    Process.Start(new ProcessStartInfo
        //    {
        //        FileName = desktopPath,
        //        UseShellExecute = true
        //    });
        //}

        //private void WrapPanel_DragEnter(object sender, DragEventArgs e)
        //{
        //    //拖拽事件
            
        //    if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //    {
        //        e.Effects = DragDropEffects.Copy;
        //    }
        //    else
        //    {
        //        e.Effects = DragDropEffects.None;
        //    }
        //}

        //private void WrapPanel_Drop(object sender, DragEventArgs e)
        //{
        //    //拖拽事件
            
        //    if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //    {
        //        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        //        foreach (string file in files)
        //        {
        //            // 这里处理拖入的文件
        //            MessageBox.Show("文件拖入: " + file);
        //        }
        //    }
        //}

        //private void OpenWeChatButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        // 微信可执行文件路径
        //        string weChatPath = @"C:\Users\Public\Desktop\微信.lnk";

        //        // 启动微信应用程序
        //        Process.Start(new ProcessStartInfo(weChatPath) { UseShellExecute = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("无法启动微信应用程序: " + ex.Message);
        //    }
        //}

        //private void ScanAndDisplayShortcuts(object sender, RoutedEventArgs e)
        //{
        //    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //    List<string> shortcutFiles = new List<string>();

        //    foreach (var file in Directory.GetFiles(desktopPath, "*.lnk"))
        //    {
        //        string targetPath = GetShortcutTarget(file);
        //        FileInfo fileInfo = new FileInfo(targetPath);
               

        //        // 检查目标路径是否为文件
        //        if (fileInfo.Exists && !fileInfo.Attributes.HasFlag(FileAttributes.Directory))
        //        {
        //            shortcutFiles.Add(file);
        //        }

        //    }
            
        //    ShortcutWindow shortcutWindow = new ShortcutWindow(shortcutFiles);
        //    shortcutWindow.Show();
        //}

        //private string GetShortcutTarget(string shortcutPath)
        //{
        //    WshShell shell = new WshShell();
        //    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
        //    return shortcut.TargetPath;
        //}


        
    }
}
