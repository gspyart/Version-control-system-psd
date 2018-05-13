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
        internal PSDProject selected;

        delegate void kek();
        
        public MainWindow()
        {
            InitializeComponent();
            ArtistText.DataContext = this;
            Tagging.DataContext = App.Data;
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
            openFileDialog.Filter = "PSD files(*.PSD;)|*.PSD; ";
            openFileDialog.CheckFileExists = true;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                App.Data.AddProject(new PSDProject(openFileDialog.SafeFileName, openFileDialog.FileName, "uncorrect"));
                // когда будет готово App.Data.AddProject(new PSDProject(openFileDialog.SafeFileName,openFileDialog.FileName, App.Authorization.data.sender.username));  
            }
            
        }

        private void Tagging_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            commits.DataContext = (PSDProject)Tagging.SelectedItem;
        }
    }
}   