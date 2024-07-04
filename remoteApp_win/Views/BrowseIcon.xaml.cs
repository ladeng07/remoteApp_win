using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Shapes;
using AddRemoteAppSP;
using AduSkin.Controls.Metro;
using IconDisplayApp;
using Microsoft.Win32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Image = System.Windows.Controls.Image;

namespace remoteApp_win.Views
{
    /// <summary>
    /// BrowseIcon.xaml 的交互逻辑
    /// </summary>
    public partial class BrowseIcon : MetroWindow
    {
        public Image shortcutImage { get; set; }
        public class IconStackPanel : StackPanel
        {
            public string iconName { get; set; }
            public string iconPath { get; set; }
            

            public static readonly DependencyProperty IsSelectedProperty =
                DependencyProperty.Register("IsSelected", typeof(bool), typeof(IconStackPanel), new PropertyMetadata(false, OnIsSelectedChanged));

            public bool IsSelected
            {
                get { return (bool)GetValue(IsSelectedProperty); }
                set { SetValue(IsSelectedProperty, value); }
            }

            private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var panel = d as IconStackPanel;
                if (panel != null)
                {
                    panel.UpdateVisualState();
                }
            }

            private void UpdateVisualState()
            {
                if (IsSelected)
                {
                    Background = new SolidColorBrush(Colors.LightBlue);
                }
                else
                {
                    Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }



        public BrowseIcon()
        {
            InitializeComponent();
            
        }

        private void BrowseIcon_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图标 (*.exe;*.dll;*.ico)|*.exe;*.dll;*.ico|所有文件 (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                if (string.IsNullOrEmpty(filePath)) return;
                

                // 检查文件扩展名是否合法
                string extension = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
                if ((extension.Equals(".exe") || extension.Equals(".dll") || extension.Equals(".ico")) && System.IO.File.Exists(filePath))
                {
                    // 你可以设置图标路径文本框的内容
                    iconPath.Text = openFileDialog.FileName;
                    IconWrapPanel.Children.Clear();

                    List<int> iconIndexes = GetIconIndexes(filePath);
                    Image tempImage = null;

                    foreach (int index in iconIndexes)
                    {
                        IconStackPanel iconItem = new IconStackPanel {
                            iconName = index.ToString(),
                            iconPath = iconPath.Text,
                        };

                         // 您需要根据路径获取实际图标
                                                           //选中文件中的图标总数
                        var iconTotalCount = PrivateExtractIcons(filePath, 0, 0, 0, null, null, 0, 0);

                        //用于接收获取到的图标指针
                        IntPtr[] hIcons = new IntPtr[iconTotalCount];
                        ////对应的图标id
                        int[] ids = new int[iconTotalCount];
                        ////成功获取到的图标个数
                        var successCount = PrivateExtractIcons(filePath, 0, 64, 64, hIcons, null, iconTotalCount, 0);

                        Icon icon; //判断exe还是ico
                        BitmapSource bitmapSource;
                        if (hIcons.Length > 0)
                        { //针对exe
                            try
                            {
                                icon = System.Drawing.Icon.FromHandle(hIcons[index]);
                                bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                          icon.Handle,
                                          Int32Rect.Empty,
                                          BitmapSizeOptions.FromWidthAndHeight(64, 64));
                            }
                            catch (Exception)
                            {
                                //处理某些应用传递icon空句柄（向日葵）
                                bitmapSource = GetBitmapSourceFromIcon(iconItem.iconPath);
                            }

                        }
                        else
                        {  //针对Icon
                            bitmapSource = GetBitmapSourceFromIcon(iconItem.iconPath);

                        }

                        shortcutImage = new Image();
                        shortcutImage.Source = bitmapSource;
                        if(index == 0) tempImage= shortcutImage;

                        shortcutImage.Width = 32;
                        shortcutImage.Height = 32;

                        shortcutImage.Margin = new Thickness(10);


                        TextBlock shortcutText = new TextBlock();
                        shortcutText.Text = iconItem.iconName;
                        shortcutText.TextWrapping = TextWrapping.Wrap; // 设置自动换行
                        shortcutText.TextAlignment = TextAlignment.Center; // 将文本水平对齐设置为居中
                        shortcutText.Width = 60;
                        shortcutText.MaxHeight = 4 * shortcutText.FontSize;
                        shortcutText.TextTrimming = TextTrimming.CharacterEllipsis;
                        shortcutText.Margin = new Thickness(10);

                        iconItem.Children.Add(shortcutImage);
                        iconItem.Children.Add(shortcutText);


                        iconItem.PreviewMouseLeftButtonUp += (senderr, ee) =>
                        {
                            IconStackPanel clickedStackPanel = senderr as IconStackPanel;
                            if (clickedStackPanel != null)
                            {
                                foreach (IconStackPanel child in IconWrapPanel.Children)
                                {
                                    child.IsSelected = false;
                                }
                                clickedStackPanel.IsSelected = true;
                                iconIndex.Text = index.ToString();
                                shortcutImage = (Image) iconItem.Children[0];
                            }
                        };



                        IconWrapPanel.Children.Add(iconItem);
                    };
                    //默认值
                    shortcutImage = tempImage;
                    iconIndex.Text = "0";

                }
                else
                {
                    AduMessageBox.Show($"请选择正确的图标！");
                }
            }
                
            }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // 关闭当前窗口
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // 获取需要传递的数据
            string iconPathValue = iconPath.Text;
            string iconIndexValue = iconIndex.Text;
            

            if (Owner is AddRemoteApp AddRemoteAppWindow)
            {
                // 假设 MainWindow 有一个方法来接收图标路径和索引
                AddRemoteAppWindow.SetIconData(iconPath.Text, iconIndex.Text, shortcutImage);
            }

            // 关闭当前窗口
            this.Close();
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
            catch (Exception ex) { return null; }

        }

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        private IntPtr ExtractIconFromFile(string filePath, int iconIndex)
        {
            return ExtractIcon(IntPtr.Zero, filePath, iconIndex);
        }


        // 定义提取图标的函数
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        // 获取图标的数量
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern uint ExtractIconEx(string lpszFile, int nIconIndex, IntPtr[] phIconLarge, IntPtr[] phIconSmall, uint nIcons);

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

        public static List<int> GetIconIndexes(string filePath)
        {
            List<int> iconIndexes = new List<int>();

            // 获取图标的数量
            uint iconCount = ExtractIconEx(filePath, -1, null, null, 0);

            // 如果有图标，则添加它们的索引到列表中
            if (iconCount > 0)
            {
                for (int i = 0; i < iconCount; i++)
                {
                    iconIndexes.Add(i);
                }
            }

            return iconIndexes;
        }

    }


    


}
