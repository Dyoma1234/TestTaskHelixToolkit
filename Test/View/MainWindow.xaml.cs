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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Threading;

using HelixToolkit.Wpf;
using Microsoft.Win32;

namespace Test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window ,IView
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(this);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
        string IView.GetFilePath()
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "Object file(*.obj) | *.obj"
            };
            if (dlg.ShowDialog() == true)
            {
              
                return dlg.FileName;
            }
            return string.Empty;
        }
        void IView.ShowError(string message)
        {    
          MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
       

        private void Title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
