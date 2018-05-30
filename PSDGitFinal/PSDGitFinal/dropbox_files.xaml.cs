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
using dp;
using Dropbox.Api;
using System.Collections.ObjectModel;
using Dropbox.Api.Files;
using Dropbox.Api.Team;
namespace PSDGitFinal
{
    /// <summary>
    /// Логика взаимодействия для dropbox_files.xaml
    /// </summary>
    public partial class dropbox_files : Window
    {
        List<RemoteProject> list;
        public async void load()
        {
            list = await App.Data.GetProjects(App.Authorization.data.client);
            foreach (RemoteProject u in list)
            {
                Projects.Items.Add(u);
            }
        }
        public dropbox_files()
        {

            this.DataContext = this;
            load();
            InitializeComponent();
            Projects.SelectionChanged += (a, b) =>
            {
                App.Data.Download(App.Authorization.data.client, (RemoteProject)Projects.SelectedItem, App.Authorization.data.sender);
            };

        }
    }
}
