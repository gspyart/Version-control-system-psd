using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using DropbBoxLogIn;
using dp;
namespace PSDGit
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static public Authorization id = new Authorization();
        static Data data = new Data();
        delegate void check();
        OpenFileDialog dlg = new OpenFileDialog();

        public MainWindow()
        {

            dlg.Filter = "Psd files (*.psd)|*.psd";
            check p1 = () =>
            {
                if (!id.IsLogged())
                {
                    Auth form = new Auth();
                    form.Show();
                }
            };
            try
            {
                InitializeComponent();
                MWWindow.IsEnabled = false;
                PGAddProject.Click += (t, o) =>
                {
                    dlg.ShowDialog();
                };
                dlg.FileOk += (l, d) =>
                {
                    PSDFile psdbuff = new PSDFile(dlg.SafeFileName, dlg.FileName, "0");

                    data.AddProject(psdbuff);
                };
                id.SenderChanged += () => { PGUsername.Text = id.data.sender.username; MWWindow.IsEnabled = true; };
                PGLogOutBtn.Click += (t, o) => { MWWindow.IsEnabled = false; id.LogOut(); p1(); };
                id.Start();
                p1();


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

    }
}
