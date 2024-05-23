using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using IWshRuntimeLibrary;
using remoteApp_win;

namespace IconDisplayApp
{
    public partial class ShortcutWindow : Window
    {
        public ShortcutWindow(List<string> shortcutFiles)
        {
            InitializeComponent();
            DisplayShortcuts(shortcutFiles);
        }

        private void DisplayShortcuts(List<string> shortcutFiles)
        {
            foreach (var shortcutFile in shortcutFiles)
            {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutFile);

                string iconLocation = shortcut.IconLocation;
                string shortcutName = System.IO.Path.GetFileNameWithoutExtension(shortcutFile);

                Image shortcutImage = new Image();
                shortcutImage.Source = GetBitmapSourceFromIcon(shortcut.TargetPath + ",0");
                shortcutImage.Width = 32;
                shortcutImage.Height = 32;

                shortcutImage.Margin = new Thickness(10);

                TextBlock shortcutText = new TextBlock();
                shortcutText.Text = shortcutName;
                shortcutText.TextWrapping = TextWrapping.Wrap; // 设置自动换行

                shortcutText.Width= 50;
                shortcutText.MaxHeight = 4 * shortcutText.FontSize;
                shortcutText.TextTrimming = TextTrimming.CharacterEllipsis;
                shortcutText.Margin = new Thickness(10);

                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

                // 添加事件处理程序，将图标添加到 AppWrapPanel 中
                shortcutImage.MouseDown += (sender, e) =>
                {
                    Image clickedImage = sender as Image;
                    if (clickedImage != null)
                    {
                        StackPanel parentStackPanel = clickedImage.Parent as StackPanel;
                        if (parentStackPanel != null)
                        {
                            ShortcutWrapPanel.Children.Remove(parentStackPanel);
                            mainWindow.AppWrapPanel.Children.Add(parentStackPanel);
                        }
                    }
                };


                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Vertical;
                stackPanel.Children.Add(shortcutImage);
                stackPanel.Children.Add(shortcutText);

                ShortcutWrapPanel.Children.Add(stackPanel);
            }
        }

        private BitmapSource GetBitmapSourceFromIcon(string iconPath)
        {
            string[] splitPath = iconPath.Split(',');
            string filePath = splitPath[0];
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

        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        private IntPtr ExtractIconFromFile(string filePath, int iconIndex)
        {
            return ExtractIcon(IntPtr.Zero, filePath, iconIndex);
        }
    }
}
