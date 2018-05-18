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
    /// Логика взаимодействия для commit.xaml
    /// </summary>
    public partial class commit : Window
    {
        public event EventHandler happend;

        public commit()
        {
            InitializeComponent();
        }
        public void ok (object sender, EventArgs e)
        {
            happend(commit_name.Text, EventArgs.Empty);
            this.Close();
        }
    }
}
