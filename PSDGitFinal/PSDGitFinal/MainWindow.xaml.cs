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
using System.Data.SQLite;
using System.IO;
using Microsoft.Win32;
using dp;
namespace PSDGitFinal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        delegate void kek();
        
        public MainWindow()
        {
            InitializeComponent();
            App.Data.PropertyChanged += (a, b) => { MessageBox.Show("kek"); };

            //this.IsEnabled = false;
            //AuthorizationWindow AuthWindow = new AuthorizationWindow();
            //AuthWindow.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Open_dialog(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Картинки(*.PSD;*.JPEG)|*.PSD;*.JPEG" + "|Все файлы (*.*)|*.* ";
            openFileDialog.CheckFileExists = true;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                MessageBox.Show("choosen");
                App.Data.AddProject(new PSDProject(openFileDialog.SafeFileName,openFileDialog.FileName, "kek"));

               
            }
            
        }
    }
}