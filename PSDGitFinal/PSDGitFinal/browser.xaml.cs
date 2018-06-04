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
using System.Threading.Tasks;

namespace PSDGitFinal
{
    /// <summary>
    /// Логика взаимодействия для browser.xaml
    /// </summary>
    public partial class browser : Window
    {
        public browser()
        {
            InitializeComponent();
            this.DataContext = App.Authorization.data;
            /*
            users.SelectionChanged += (a, b) =>
            {
                App.Authorization.Choose((DropbBoxLogIn.User)(users.SelectedItem));
                this.Close();
            };
            */
            Browser.Navigated += (a,b) =>
            {
                if (Browser.Source.AbsoluteUri.Contains("access_token"))
                {

                   App.Authorization.Logined(Browser);
                   this.Close();
                }
            };
            App.Authorization.Login(Browser);
        }
    }
}
