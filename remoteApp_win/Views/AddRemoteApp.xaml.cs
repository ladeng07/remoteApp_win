using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Shapes;
using AduSkin.Controls.Metro;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using remoteApp_win.Views;
using static remoteApp_win.UserControls.AppList;


namespace AddRemoteAppSP
{
    /// <summary>
    /// AddRemoteApp.xaml 的交互逻辑
    /// </summary>
    public partial class AddRemoteApp : MetroWindow
    {
        public AddRemoteApp()
        {
            InitializeComponent();
        }



        private void BrowseAppPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            

            if (openFileDialog.ShowDialog() == true)
            {
                // 你可以设置应用路径文本框的内容
                remoteAppPath.Text = openFileDialog.FileName;
            }
        }

        private void BrowseIconPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            AppStackPanel_ childdStackPanel = new AppStackPanel_();
            childdStackPanel.AppPath = remoteAppPath.Text;
            childdStackPanel.AppName = remoteAppName.Text;
            childdStackPanel.AppIconPath = remoteAppIconPath.Text;
            //childdStackPanel.AppIcon = clickedStackPanel.AppIcon;


            // 保存按钮的点击事件处理逻辑
            MessageBox.Show("保存成功");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // 取消按钮的点击事件处理逻辑
            this.Close();
        }

        private void OpenAppPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "可执行文件 (*.exe;*.bat;*.cmd)|*.exe;*.bat;*.cmd|所有文件 (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                // 检查文件扩展名是否为 .exe 或 .lnk
                string extension = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
                if (extension.Equals(".exe") || extension.Equals(".bat") || extension.Equals(".cmd"))
                {
                    // 你可以设置图标路径文本框的内容
                    remoteAppPath.Text = openFileDialog.FileName;
                }
                else
                {
                    AduMessageBox.Show($"请选择可执行文件！");
                }
            }
        }

        private void OpenAppIconPath_Click(object sender, EventArgs e)
        {
            BrowseIcon BrowseIconWindow = new BrowseIcon();
            BrowseIconWindow.Owner = this;
            BrowseIconWindow.ShowDialog();
        }




        public void SetIconData(string path, string index)
        {
            // 在这里处理接收到的图标路径和索引数据
            // 例如，设置MainWindow中的相关控件的值
            // 示例代码：
            remoteAppIconPath.Text = path;
            remoteAppIconIndex.Text = index;
        }
    }

        

}
