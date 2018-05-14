using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
using Dropbox.Api;
using Dropbox.Api.Users;
using Newtonsoft.Json;
using System.Data.Sql;
using System.ComponentModel;
using System.Data;
using ImageMagick;
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
        public void DatabaseLoad(User user)
        {
            UserProjects.Clear();
            SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
            m_dbConn.Open();
            SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
            m_sqlCmd.CommandText = "Select * from Projects";
            SQLiteDataReader data = m_sqlCmd.ExecuteReader();
            while(data.Read())
            {
                if (data.GetString(3) == user.id) AddProject(new PSDProject(data.GetInt16(0),data.GetString(1), data.GetString(2), data.GetString(3)));
            }
            //==========
            //m_sqlCmd = m_dbConn.CreateCommand();
            //m_sqlCmd.CommandText = "Select * from Commits";
            //data = m_sqlCmd.ExecuteReader();
            //while (data.Read())
            //{
            //    foreach (PSDProject kek in UserProjects)
            //    {

            //        if (data.GetString(2) == kek.name) kek.AddCommit(new Save(data.GetString(1), data.GetInt16(0)));
            //    }
            //}

            m_dbConn.Close();

        }
        public void DatabaseInsert(PSDProject a) {
            SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
            m_dbConn.Open();
            SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
            m_sqlCmd.CommandText = "INSERT INTO Projects (project_name, dir, owner) values ('" + a.name + "','" +a.dir +"','" + a.owner_id +"')";
            m_sqlCmd.ExecuteNonQuery(); 
            m_dbConn.Close();
        }

        public void DatabaseDelete(int id)
        {
           
        }
    }



    class PSDProject //проект
    {
        
        public FileSystemWatcher looks = new FileSystemWatcher(); //отслеживание файла
        public int id { get; set; }
        public string name { get; set; } //полное название файла
        public string dir { get; set; } //путь проекта, на конце "/"
        public string owner_id { get; set; } //имя пользователя проекта
        public ObservableCollection<Save> Commits { get; set; } //список коммитов проекта
        public PSDProject(int i, string n, string d, string v)
        {
            id = i;
            Commits = new ObservableCollection<Save>();
            name = n;
            dir = d;
            owner_id = v;
            Directory.CreateDirectory("data/" + owner_id.Replace(':', '-') + "/" + name.Remove(name.Length-4)); //замена недопустимых символов в пути
            looks.Path = d; 
            looks.EnableRaisingEvents = true;
            looks.Created += (a, b) =>
            {

                looks.EnableRaisingEvents = false;
                looks.EnableRaisingEvents = true;
                Save ns = new Save("test commit", Commits.Count);
                AddCommit(ns);

                //SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
                //m_dbConn.Open();
                //SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
                //m_sqlCmd.CommandText = "INSERT INTO Commits (id, message, Project) values ('" + ns.number + "','" + ns.message + "'," + "'" + name + "')";
                //m_sqlCmd.ExecuteNonQuery();
                //m_dbConn.Close();

                File.Copy(dir + n, "data/" + owner_id.Replace(':', '-') + "/" + name.Remove(name.Length - 4) + "/" + "commit_" + Commits.Count + ".psd");

                var image = new MagickImage(dir+n);
                image.Write("data/" + owner_id.Replace(':', '-') + "/" + name.Remove(name.Length - 4) + "/" + "commit_" + Commits.Count + ".psd.jpg");
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
        public Save(string m, int n)
        {
            message = m;
            number = n;
        }
        public Save()
        {
            message = "commit_test";
            number = 0;

        }
    }


}

