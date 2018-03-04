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

        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           
           
            if (id.data.sender != null)
            {
                var form2 = new MainWin();
                
                Application.Run(form2);
                
            }
            else {
                var form1 = new Form1();
                Application.Run(form1);
    
            }
          


        }
    }
}
