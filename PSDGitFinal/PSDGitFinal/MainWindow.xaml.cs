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
                App.Authorization.CheckToken();
                UsernameText.Text = App.Authorization.data.sender.username;
                App.Data.DatabaseLoad(App.Authorization.data.sender);
                App.Data.DatabaseDBLoad(App.Authorization.data.sender);
                changePhoto();
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

            void changePhoto()
            {
                
                var t = App.Authorization.data.client.Users.GetAccountAsync(App.Authorization.data.sender.id);
                t.Wait();
                WebRequest requestPic = WebRequest.Create(t.Result.ProfilePhotoUrl);
                WebResponse responsePic = requestPic.GetResponse();
                var ms = responsePic.GetResponseStream();
                var image = System.Drawing.Image.FromStream(ms);
                var mystream = new MemoryStream();
                image.Save(mystream, image.RawFormat);
                using (BinaryReader br = new BinaryReader(mystream))
                {
                    App.Authorization.data.Ava = br.ReadBytes((int)mystream.Length);
                }
                //========
                var fil = File.Create("data/img.jpg");
                List<byte> list = new List<byte>();
                foreach (var byt in mystream.ToArray())
                {
                    fil.WriteByte(byt);
                    list.Add(byt);
                }
                mystream.Close();
                fil.Close();
                App.Authorization.data.Ava = list.ToArray();
                avka.ImageSource = new BitmapImage(new Uri(@"D:\AAA PSDGit\Version-control-system-psd\PSDGitFinal\PSDGitFinal\bin\Debug\data\img.jpg"));
                //========

                ms.Close();
                responsePic.Close();
                requestPic.Abort();

            }
            if (!App.Authorization.isOnline())
            {
                logout_openAuth();
            }
            else
            {
                try { 
                UsernameText.Text = App.Authorization.data.sender.username;
                App.Data.DatabaseLoad(App.Authorization.data.sender);
                App.Data.DatabaseDBLoad(App.Authorization.data.sender);
                changePhoto();
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

        }

        private void Button_logout(object sender, RoutedEventArgs e)
        {
            App.Authorization.LogOut();
        }

        private void logout_openAuth()
        {
            this.IsEnabled = false;
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
        //private void Db_files(object sender, RoutedEventArgs e)
        //{
        //    dropbox_files dwin = new dropbox_files();
        //    dwin.Show();
        //}
    }
}