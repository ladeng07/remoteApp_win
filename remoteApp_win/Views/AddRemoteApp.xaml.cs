using System;
using System.Collections.Generic;
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
    }
}
