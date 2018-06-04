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

namespace PSDGitFinal
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        public AuthorizationWindow()
        {
            this.DataContext = App.Authorization.data;
            DropbBoxLogIn.Auth.SenderChanged += () => {
                this.Close();
            };

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (select.SelectedItem != null)
            {
                App.Authorization.Choose((DropbBoxLogIn.User)(select.SelectedItem));
            }
            else
            {
                browser web = new browser();
                web.Show();
            }
            
        }

    }
}
