using DropbBoxLogIn;
using System.Windows;
using System.Threading;
using System;
namespace PSDGit
{
    /// <summary>
    /// Логика взаимодействия для Auth.xaml
    /// </summary>
    public partial class Auth : Window
    {
 
        public Auth()
        {
           
            InitializeComponent();
            PGuserlist.DisplayMemberPath = "username";

            PGuserlist.ItemsSource = MainWindow.id.data.GetList();

            PGuserlist.SelectionChanged += (s, e) => {MainWindow.id.Choose((User)PGuserlist.SelectedItem); this.Close();};
            
            PGLoginBtn.Click += (t, tt) => { PGExplorer.Source = MainWindow.id.Login();  PGExplorer.Visibility = Visibility.Visible; PGExplorer.Navigate(MainWindow.id.Login()); };
            PGExplorer.Navigated += (ab, ba) =>
            {

                if (ba.Uri.AbsoluteUri.Contains("https://localhost/authorize") && !ba.Uri.AbsoluteUri.Contains("dropbox.com"))
                {                
                    MainWindow.id.Logined(ba.Uri);
                  //  this.PGExplorer.Dispose();
                 //   PGExplorer.Visibility = Visibility.Collapsed;
                    this.Close();
                }
            };
            this.Closing += (o, e) => { Application.Current.Shutdown(); };
        }
        void PGExitFun(object sunder, EventArgs e)
        {  
            Application.Current.Shutdown();      
        }

    }
}
