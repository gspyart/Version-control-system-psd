using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Net;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.IO.Compression;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.Sql;
using System.IO;
using System.Data;
using Microsoft.Win32;
using dp;
using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Team;
namespace PSDGitFinal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PSDProject selected;
        public MainWindow()
        {
            InitializeComponent();
            commit ct = new commit();
            Tagging.DataContext = App.Data;
            commits.DataContext = selected;

            DropbBoxLogIn.Auth.SenderChanged += () => // событие при авторизации нового пользователя
            //надо исправить login событие
            {
                try { 
                this.IsEnabled = true;
                    this.Visibility = Visibility.Visible;
                    UsernameText.Text = App.Authorization.data.sender.username;
                App.Data.DatabaseLoad(App.Authorization.data.sender);
                changePhoto();
                App.Authorization.CheckToken();
                App.Data.DatabaseDBLoad(App.Authorization.data.sender);
               
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            };

            DropbBoxLogIn.Auth.logout += () => // событие если пользователь вышел из аккаунта
            {
                logout_openAuth();
            };

            dp.PSDProject.okno += (t, y) =>
            {

                ct.Dispatcher.Invoke(() =>
                {
                    ct = new commit(); ct.Show(); ct.Focus();

                    ct.KeyDown += (a, b) => { if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.S) && Keyboard.IsKeyDown(Key.D)) ct.ok(this, EventArgs.Empty); };
                });

                ct.Closed += (j, o) =>
                {

                };

                ct.happend += (j, o) =>
                {

                    ((PSDProject)(t)).txt((string)j);
                };
            };

            commits.SelectionChanged += (a, b) =>
            {
                try { 
                Save t = (Save)commits.SelectedItem;
                // MessageBox.Show("номер коммита " + t.que.ToString());
                t.Open((PSDProject)Tagging.SelectedItem);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Uncorrect selected item: " + ex.Message);
                }
            };

            async void changePhoto()
            {

                var t = await App.Authorization.data.client.Users.GetAccountAsync(App.Authorization.data.sender.id);
                //t.Wait();
                //BitmapImage asd = new BitmapImage();
                //if (t.Result.ProfilePhotoUrl == null)
                //{
                //    MessageBox.Show("net");
                //    asd.BeginInit();
                //    asd.UriSource = new Uri(@"no-profile.jpg");
                //    asd.EndInit();
                //    App.Authorization.data.Ava = asd;
                //    avka.ImageSource = asd;
                //    return;
                //}
                WebRequest requestPic = WebRequest.Create(t.ProfilePhotoUrl);
                WebResponse responsePic = requestPic.GetResponse();
                
                var ms = responsePic.GetResponseStream();
                BitmapImage asd = new BitmapImage();
                asd.BeginInit();
                asd.StreamSource = ms;
                asd.EndInit();
                App.Authorization.data.Ava = asd;
                avka.ImageSource = asd;
                //========
                ms.Close();
                responsePic.Close();
                requestPic.Abort();
                

            }
            if (!App.Authorization.isOnline())
            {
                this.Visibility = Visibility.Hidden;
                logout_openAuth();
            }
            else
            {
                try {
                
                UsernameText.Text = App.Authorization.data.sender.username;
                App.Data.DatabaseLoad(App.Authorization.data.sender);
                changePhoto();
                App.Authorization.CheckToken();
                App.Data.DatabaseDBLoad(App.Authorization.data.sender);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
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
            try
            {
              
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PSD files(*.PSD;)|*.PSD; ";
                openFileDialog.CheckFileExists = true;
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == true) //Добавление проекта в бд и загрузка всех проектов из бд в программу с "нормальным" id
                {
                    if (App.Data.ProjectExist(openFileDialog.SafeFileName, App.Authorization.data.sender.id))
                    {
                        MessageBox.Show("Проект существует");
                        return;
                    }
                    PSDProject b = new PSDProject(-1, openFileDialog.SafeFileName, openFileDialog.FileName.Remove(openFileDialog.FileName.Length - openFileDialog.SafeFileName.Length), App.Authorization.data.sender.id);
                    //App.Data.AddProject(b);
                    App.Data.DatabaseInsert(b);
                    b.off();
                    App.Data.DatabaseLoad(App.Authorization.data.sender);
                }
            }
            catch (Exception message)
            {
                MessageBox.Show("Ошибка при добавлении проекта: " + message.Message);
            }


        }
        private async void DB_Upload(object sender, RoutedEventArgs e)
        {
            try { 
            PSDProject o = (PSDProject)Tagging.SelectedItem;
            Data.TryUpload(App.Data.ProjectLoad, o, App.Authorization.data.client, App.Authorization.data.sender);
                //     App.Authorization.data.client.Files
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не выбран проект " + ex.Message);
            }

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
            try
            {
                PSDProject o = (PSDProject)Tagging.SelectedItem;
                App.Data.DeleteProject(o);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_logout(object sender, RoutedEventArgs e)
        {
            App.Authorization.LogOut();
        }

        private void logout_openAuth()
        {
            this.IsEnabled = false;
            this.Visibility = Visibility.Hidden;
            AuthorizationWindow o = new AuthorizationWindow();
            o.Show();
        }


   
            private void SearchString_TextChanged(object sender, TextChangedEventArgs e)
        {
            ObservableCollection<PSDProject> SearchResult = new ObservableCollection<PSDProject>();
            Regex SearchRequest = new Regex(SearchString.Text);

            foreach (var Project in App.Data.UserProjects)
            {
                if (SearchRequest.IsMatch(Project.name) == true)
                {
                    SearchResult.Add(Project);
                }
            }
            Tagging.ItemsSource = SearchString.Text.Length == 0 ? App.Data.UserProjects : SearchResult;
        }

        //private void commits_SelectionChanged(object sender, MouseButtonEventArgs e)
        //{
        //    try
        //    {
        //        ((Save)commits.SelectedItem).Open((PSDProject)Tagging.SelectedItem);
        //    }
        //    catch(Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}
    
    }
}