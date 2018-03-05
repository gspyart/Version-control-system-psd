using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DropbBoxLogIn;

namespace dp
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        /// 

        public static Auth id = new Auth();
        public static Data data = new Data();
        
        [STAThread]
        static void Main()
        {
            id.SenderChanged += () => { data.Update(id.data);  };
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWin());

        }
    }
}
