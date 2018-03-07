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
using System.Windows.Shapes;
using DropbBoxLogIn;
using dp;
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

            PGuserlist.SelectionChanged += (s, e) => {MainWindow.id.Choose((User)PGuserlist.SelectedItem); this.Close(); };
            
            PGLoginBtn.Click += (t, tt) => { PGExplorer.Visibility = Visibility.Visible; PGExplorer.Navigate(MainWindow.id.Login()); };
            PGExplorer.Navigated += (ab, ba) =>
            {

                if (ba.Uri.AbsoluteUri.Contains("https://localhost/authorize") && !ba.Uri.AbsoluteUri.Contains("dropbox.com"))
                {
                    MainWindow.id.Logined(ba.Uri);
                    PGExplorer.Navigate("http://0.0.0.0");
                    PGExplorer.Visibility = Visibility.Collapsed;
                    this.Close();

                }
            };


        }


    }
}
