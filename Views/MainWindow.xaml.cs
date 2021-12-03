using System;
using System.Linq;
using System.Windows;
using Microsoft.Toolkit.Mvvm.Messaging;
using ModernWpf.Controls;
using OpenCCNET;

namespace OpenCC.NET.GUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TextPage textPage = new TextPage();
        FilePage filePage = new FilePage();

        public MainWindow()
        {
            InitializeComponent();
            NavigationView.SelectedItem = NavigationView.MenuItems.OfType<NavigationViewItem>().FirstOrDefault();
        }

        private void NavigationView_SelectionChanged(NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem selectedItem)
            {
                var mode = selectedItem.Tag as string;
                switch (mode)
                {
                    case "Text":
                        ContentFrame.Navigate(textPage);
                        break;
                    case "File":
                        ContentFrame.Navigate(filePage);
                        break;
                }

                WeakReferenceMessenger.Default.Send(mode, "ActiveMode");
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ZhUtil.Initialize();
            }
            catch (Exception)
            {
                MessageBox.Show("加载失败，请检查资源文件是否缺失！");
                Application.Current.Shutdown();
            }
        }
    }
}