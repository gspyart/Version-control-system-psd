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
using System.IO.Compression;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.Sql;
using System.IO;
using System.Data;
using Microsoft.Win32;
using dp;
namespace PSDGitFinal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal DataTable ProjectsTable;
        PSDProject selected;
        
        public MainWindow()
        {
            InitializeComponent();

            DropbBoxLogIn.Auth.SenderChanged += () => // событие при авторизации нового пользователя
            //надо исправить login событие
            {
                this.IsEnabled = true;
                UsernameText.Text = App.Authorization.data.sender.username;
                App.Data.DatabaseLoad(App.Authorization.data.sender);
            };

            DropbBoxLogIn.Auth.logout += () => // событие если пользователь вышел из аккаунта
            {

            };

            Tagging.DataContext = App.Data;
            commits.DataContext = selected;
          
            if (App.Authorization.data.sender == null)
            {
                logout_openAuth();
            }
            else
            {
                UsernameText.Text = App.Authorization.data.sender.username;
                App.Data.DatabaseLoad(App.Authorization.data.sender);
            }
            //else
            //{
            //    ProjectsTable =  App.Data.DatabaseLoad();
            //    MessageBox.Show(ProjectsTable.Select().ToString());
            //}
          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        //
        // TOP BAR 
        //
        private void Open_dialog(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PSD files(*.PSD;)|*.PSD; ";
            openFileDialog.CheckFileExists = true;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                //добавление проекта
                PSDProject b = new PSDProject(-1, openFileDialog.SafeFileName, openFileDialog.FileName.Remove(openFileDialog.FileName.Length - openFileDialog.SafeFileName.Length), App.Authorization.data.sender.id);
                App.Data.AddProject(b);
                App.Data.DatabaseInsert(b);
            }
            
        }
        private void DB_Upload(object sender, RoutedEventArgs e)
        {
       //     App.Authorization.data.client.Files

        }
        private void DB_UploadAll(object sender, RoutedEventArgs e)
        {

        }
        //
        //
        //
        private void Tagging_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected = (PSDProject)Tagging.SelectedItem;
            commits.DataContext = selected;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
        private void Delete_project(object sender, RoutedEventArgs e)
        {
           
        }

        private void Button_logout(object sender, RoutedEventArgs e)
        {
            App.Authorization.LogOut();
            logout_openAuth();
        }

        private void logout_openAuth()
        {
            this.IsEnabled = false;
            AuthorizationWindow o = new AuthorizationWindow();
            o.Show();
        }

    }
}   