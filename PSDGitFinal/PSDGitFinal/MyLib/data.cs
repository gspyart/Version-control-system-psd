using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Dianoga.ImageMagick;
using System.Collections.ObjectModel;
using System.IO;
using DropbBoxLogIn;
using System.IO.Compression;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.Sql;
using System.ComponentModel;
using System.Data;
using PSDGitFinal;
namespace dp
{
    class Data
    {

        public ObservableCollection<PSDProject> UserProjects { get; set; }

        public void AddProject(PSDProject b)
        {
            UserProjects.Add(b);
     
        }
        public Data()
        {
            UserProjects = new ObservableCollection<PSDProject>();
        }
        public DataTable DatabaseLoad()
        {
            DataTable dTable = new DataTable();

            SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=database.db; Version=3;");
            m_dbConn.Open();

            SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
            m_sqlCmd.CommandText = "Select * from Projects";
           SQLiteDataReader data = m_sqlCmd.ExecuteReader();
            dTable = data.GetSchemaTable();
            m_dbConn.Close();
            return dTable;

        }
    }



    class PSDProject //проект
    {
        public FileSystemWatcher looks = new FileSystemWatcher(); //отслеживание файла
        public string name { get; set; } //название проекта
        public string dir { get; set; } //путь проекта
        public string owner { get; set; } //уникальный id проекта
        public ObservableCollection<Save> Commits { get; set; } //список коммитов проекта
        public PSDProject(string n, string d, string v)
        {
            Commits = new ObservableCollection<Save>();
            name = n;
            dir = d;
            owner = v;
            Directory.CreateDirectory("data/" + owner + "/" + name.Remove(name.Length-4));
            looks.Path = d; 
            looks.EnableRaisingEvents = true;
            looks.Changed += (a, b) =>
            {
                this.AddCommit(new Save());
                looks.EnableRaisingEvents = false;
                looks.EnableRaisingEvents = true;
            };
        
        }
        

        public void AddCommit(Save b)
        {
            /*
            MagickImage image = new MagickImage();
            MagickReadSettings settings = new MagickReadSettings();
            settings.Density = new Density(144);
            image.Read(dir + name + ".psd", settings);
            Directory.CreateDirectory("data/" + name + "/" + savenum.ToString());
            image.Write("data/" + name + "/" + savenum.ToString() + "/" +"preview.jpg" );
            PSDFile.savenum++;
            */
           
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                Commits.Add(b);
            });

        }  //добавить коммит

        public void SortCommits() //сортировать коммиты
        {

        }
    }

    class Save //коммит
    {
        public string message { get; set; }
        public int number { get; set; }
        public Save(string m)
        {
            message = m;
            number = 0;
        }
        public Save()
        {
            message = "commit_test";

        }
    }


}

