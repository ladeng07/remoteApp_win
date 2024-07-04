using System;
using System.IO;
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
using IconDisplayApp;
using Newtonsoft.Json;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using remoteApp_win.ViewModel;
using remoteApp_win;
using remoteApp_win.Views;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static remoteApp_win.UserControls.AppList;
using Image = System.Windows.Controls.Image;


namespace AddRemoteAppSP
{
    /// <summary>
    /// AddRemoteApp.xaml 的交互逻辑
    /// </summary>
    public partial class AddRemoteApp : MetroWindow
    {
        public Image shortcutImage;
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
            if (!CheckInputs()) return;

            AppStackPanel_ childdStackPanel = new AppStackPanel_ {
                AppPath = remoteAppPath.Text,
                AppName = remoteAppName.Text,
                AppIconPath = remoteAppIconPath.Text.EndsWith(".ico", StringComparison.OrdinalIgnoreCase)? remoteAppIconPath.Text : remoteAppIconPath.Text+","+remoteAppIconIndex.Text,
                AppIcon = "",
                Orientation = Orientation.Vertical,
            };

            TextBlock shortcutText = new TextBlock();
            shortcutText.Text = childdStackPanel.AppName;
            shortcutText.TextWrapping = TextWrapping.Wrap; // 设置自动换行
            shortcutText.TextAlignment = TextAlignment.Center; // 将文本水平对齐设置为居中
            shortcutText.Width = 60;
            shortcutText.MaxHeight = 4 * shortcutText.FontSize;
            shortcutText.TextTrimming = TextTrimming.CharacterEllipsis;
            shortcutText.Margin = new Thickness(10);

            Panel parentPanel = shortcutImage.Parent as Panel;
            parentPanel.Children.Remove(shortcutImage);

            childdStackPanel.Children.Add(shortcutImage);
            childdStackPanel.Children.Add(shortcutText);

            //给远程应用添加右下角角标
            BitmapSource iconSource = new BitmapImage(new Uri("pack://application:,,,/logo.png")); // Replace with your icon path
            BitmapSource combinedBitmap = AddIconToBitmap((BitmapSource)shortcutImage.Source, iconSource, 24); // 24x24 icon size

            childdStackPanel.AppIcon = ImageSourceToBase64(combinedBitmap);

            //获取MainWindow实例
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            //获取数据上下文
            MainViewModel viewModel = mainWindow.DataContext as MainViewModel;
            UserControl myUserControl = viewModel.Page1;
            WrapPanel AppWrapPanel = myUserControl.FindName("AppWrapPanel") as WrapPanel;

            AppWrapPanel.Children.Add(childdStackPanel);

            //刷新list.json
            WriteAppWrapPanelContentsToFile();

            this.Close();
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




        public void SetIconData(string path, string index,Image im)
        {
            // 在这里处理接收到的图标路径和索引数据
            // 例如，设置MainWindow中的相关控件的值
            // 示例代码：
            remoteAppIconPath.Text = path;
            remoteAppIconIndex.Text = index;
            shortcutImage = im;
        }

        private bool CheckInputs()
        {
            if (string.IsNullOrEmpty(remoteAppName.Text))
            {
                AduMessageBox.Show("应用名称不能为空！");
                return false;
            }

            if (string.IsNullOrEmpty(remoteAppPath.Text))
            {
                AduMessageBox.Show("应用路径不能为空！");
                return false;
            }

            if (string.IsNullOrEmpty(remoteAppIconPath.Text))
            {
                AduMessageBox.Show("图标路径不能为空！");
                return false;
            }

            if (string.IsNullOrEmpty(remoteAppIconIndex.Text))
            {
                AduMessageBox.Show("图标索引不能为空！");
                return false;
            }

            return true;
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
    }



}
