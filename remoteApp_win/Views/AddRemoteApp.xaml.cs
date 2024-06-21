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


namespace AddRemoteAppSP
{
    /// <summary>
    /// AddRemoteApp.xaml 的交互逻辑
    /// </summary>
    public partial class AddRemoteApp : Window
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
                // appPathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void BrowseIconPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                // 你可以设置图标路径文本框的内容
                // iconPathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
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
            openFileDialog.Filter = "可执行文件 (*.exe;*.lnk)|*.exe;*.lnk|所有文件 (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                // 检查文件扩展名是否为 .exe 或 .lnk
                string extension = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
                if (extension.Equals(".exe") || extension.Equals(".lnk"))
                {
                    if (extension.Equals(".lnk"))
                    {
                        //如果是.lnk则寻找到对应的exe
                        WshShell shell_ = new WshShell();
                        IWshShortcut shortcut_ = (IWshShortcut)shell_.CreateShortcut(filePath);
                        filePath = shortcut_.TargetPath;

                        //判断快捷方式是exe还是目录
                        if (System.IO.File.Exists(filePath) && System.IO.Path.GetExtension(filePath).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                        {
                            //成功逻辑
                            remoteAppPath.Text= filePath;

                        }
                        else
                        {
                            AduMessageBox.Show($"请选择可执行文件的快捷方式！");
                        }
                    }
                }
                else
                {
                    AduMessageBox.Show($"请选择以 .exe 或 .lnk 结尾的文件！");
                }
            }
        }

        private void OpenAppIconPath_Click(object sender, EventArgs e)
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
                    if (extension.Equals(".lnk"))
                    {
                        //如果是.lnk则寻找到对应的exe
                        WshShell shell_ = new WshShell();
                        IWshShortcut shortcut_ = (IWshShortcut)shell_.CreateShortcut(filePath);
                        filePath = shortcut_.TargetPath;

                        //判断快捷方式是exe还是目录
                        if (System.IO.File.Exists(filePath) && System.IO.Path.GetExtension(filePath).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                        {
                            //成功逻辑
                            remoteAppPath.Text = filePath;

                        }
                        else
                        {
                            AduMessageBox.Show($"请选择可执行文件的快捷方式！");
                        }
                    }
                }
                else
                {
                    AduMessageBox.Show($"请选择以 .exe 或 .lnk 结尾的文件！");
                }
            }
        }



    }



}
