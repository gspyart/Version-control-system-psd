using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


namespace PSDGitFinal
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static DropbBoxLogIn.Auth Authorization;
        internal static dp.Data Data;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DropbBoxLogIn.Auth.SenderChanged += () => {}; //событие происходящие при авторизации нового пользователя (надо объъявить до вызова конструктора)
            Authorization = new DropbBoxLogIn.Auth();
            Data = new dp.Data();
     
        }
    }
}
