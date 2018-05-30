using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.IO;
using Dianoga.ImageMagick;
using System.Threading;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using System.IO.Packaging;
using DropbBoxLogIn;
using System.Drawing;
using System.IO.Compression;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Team;
using Dropbox.Api.Users;
using Newtonsoft.Json;
using System.Data.Sql;
using System.ComponentModel;
using System.Data;
using ImageMagick;
using PSDGitFinal;
namespace dp
{
    

   class RemoteProject
    {
        public MemoryStream image { get; set; }
        public string name { get; set; }
        
    }

    class CommitsDescr
    {
        public int id { get; set; }
        public string descr { get; set; }
    }
    class Data
    {
        public delegate Task Tu(PSDProject a, Dropbox.Api.DropboxClient k);
        static int load = 0;
        public static void TryUpload(Tu cb, PSDProject a, Dropbox.Api.DropboxClient k) 
        {
            if (load < 1)
            {
                load++;
                Thread th = new Thread(new ThreadStart(() => { var t=Task.Run(()=>cb(a, k)); t.Wait(); load--; }));
                th.Start();

            }
        }
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
            foreach (PSDProject o in UserProjects)
            {
                o.off();
            }
                UserProjects.Clear();

            SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
            m_dbConn.Open();
            SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
            m_sqlCmd.CommandText = "Select * from Projects";
            SQLiteDataReader data = m_sqlCmd.ExecuteReader();
     
            SQLiteCommand m_sqlCmd2 = m_dbConn.CreateCommand();
            m_sqlCmd2.CommandText = "Select * from Commits";
            SQLiteDataReader data2 = m_sqlCmd2.ExecuteReader();

            while (data.Read())
            {
                data2.Close();
                data2 = m_sqlCmd2.ExecuteReader();
                if (data.GetString(3) == user.id)
                {
                    PSDProject m = new PSDProject(data.GetInt32(0), data.GetString(1), data.GetString(2), data.GetString(3));
                    AddProject(m);
                    while (data2.Read())
                    {
                        Save o = new Save(data2.GetString(3), data2.GetInt32(2), data2.GetInt32(1));
                        if (data.GetInt32(0) == data2.GetInt32(2)) m.AddCommit(o);
                    // var t = ZipFile.OpenRead("data/" + user.id.Replace(':', '-') + "/" + m.name.Remove(m.name.Length - 4) + "/commit" + UserProjects.Count);
                    //    t.GetEntry("file");
                       // o.preview = new Bitmap(t.GetEntry("preview.jpg").Open());
                    }
                }
            }
            data.Close();
            data2.Close();
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
        //public void DatabaseCommitInsert(PSDProject a, Save b)
        //{
        //    SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
        //    m_dbConn.Open();
        //    SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
        //    m_sqlCmd.CommandText = "INSERT INTO Commits (commit_number, Project_id, message) values ('" + a.name + "','" + a.dir + "','" + a.owner_id + "')";
        //    m_sqlCmd.ExecuteNonQuery();
        //    m_dbConn.Close();
        //}
        public void DatabaseDelete(int id)
        {
           
        }


        //dropbox api
        public async Task ProjectLoad(PSDProject t, DropboxClient user) //upload on dropbox
        {
            List<Save> infojson = t.Commits.ToList(); //сериализация метаданных
            var inf =  JsonConvert.SerializeObject(infojson);
            string dirpath = "/" + t.name.Remove(t.name.Length - 4);
            async Task insert()
            {
                foreach (Save o in t.Commits)
                {

                    var file = File.Open("data/" + t.owner_id.Replace(':', '-') + "/" + t.name.Remove(t.name.Length - 4) + "/commit" + o.que, FileMode.Open);
                    await user.Files.UploadAsync(dirpath + "/commit" + o.que, WriteMode.Overwrite.Instance, body: file);
                }
                string path = "data/" + t.owner_id.Replace(':', '-') + "/" + t.name.Remove(t.name.Length - 4) + "/preview.jpeg";
                var image = new MagickImage(t.dir + t.name);
                var fc = File.Create(path);
                image.ToBitmap().Save(fc, System.Drawing.Imaging.ImageFormat.Jpeg);
                fc.Close();
                var fo = File.Open(path, FileMode.Open);
                await user.Files.UploadAsync(dirpath +  "/preview.jpeg", WriteMode.Overwrite.Instance, body: fo);
                File.Delete(path);
                fo.Close();
                using (var infons = File.Create("data/" + t.owner_id.Replace(':', '-') + "/" + t.name.Remove(t.name.Length - 4) + "/metadata"))
                {
                    foreach (byte u in inf.ToArray())
                    {
                        infons.WriteByte(u);
                    }
                    infons.Close();
                }
                using (var infons = File.Open("data/" + t.owner_id.Replace(':', '-') + "/" + t.name.Remove(t.name.Length - 4) + "/metadata",FileMode.Open))
                {
                    await user.Files.UploadAsync(dirpath + "/metadata", WriteMode.Overwrite.Instance, body: infons);
                    infons.Close();
                }
                

               
               

            }
            try {
                await user.Files.CreateFolderV2Async(dirpath);
                var u = Task.Run(() => insert());
                u.Wait();
            }
            catch (DropboxException e)
            {
                var u = Task.Run(() => insert());
                u.Wait();
            }

        }

        public async Task Download(DropboxClient user, RemoteProject rp, User myuser)
        {
            var lst = await user.Files.ListFolderAsync("/"+rp.name);
            int j = lst.Entries.Count; //количество коммитов на дропбоксе

            SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
            m_dbConn.Open();
            SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
            m_sqlCmd.CommandText = "select id from Projects where project_name = " + "'" +rp.name + ".psd'";
            SQLiteDataReader data = m_sqlCmd.ExecuteReader();
            data.Read();
            int id = data.GetInt32(0);  //айди проекта в бд
            data.Close();
            m_sqlCmd.CommandText = "select * from Commits where Project_id = " + id ;
            SQLiteDataReader data2 = m_sqlCmd.ExecuteReader();
            data2.Read();
            data2.Close();
            m_dbConn.Close();
            

            //var lst = await user.Files.ListFolderAsync("/"+rp.name);
            //int commits = 0;
            //SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
            //m_dbConn.Open();
            //SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
            //m_sqlCmd.CommandText = "Select * from Projects";
            //SQLiteDataReader data = m_sqlCmd.ExecuteReader();

            //SQLiteCommand m_sqlCmd2 = m_dbConn.CreateCommand();
            //m_sqlCmd.CommandText = "Select * from Commits";
            //SQLiteDataReader data2 = m_sqlCmd.ExecuteReader();

            //while (data.Read())
            //{
            //    if (data.GetString(1) == rp.name && data.GetString(3) == myuser.id)
            //    {
            //        while(data2.Read())
            //        {
            //            if (data2.GetInt32(2) == data.GetInt32(0)) commits++;
            //        }
            //    }
            //}
            //if (commits < lst.Entries.Count-1) {
            //    List<Save> saves = new List<Save>();
            //    foreach (var file in lst.Entries)
            //    {
            //        if (file.Name != "preview.jpeg")
            //        {
            //            var k = await user.Files.DownloadAsync(path: "/" + rp.name + "/" + file.Name);
            //            var stream = await k.GetContentAsStreamAsync();
            //            MemoryStream t = new MemoryStream();
            //            stream.CopyTo(t);


            //        }
            //    }
            //}



        }
        public async Task<List<RemoteProject>> GetProjects(DropboxClient user)
        {
            List<RemoteProject> list = new List<RemoteProject>();
            var lst = await user.Files.ListFolderAsync(string.Empty);
            foreach(var item in lst.Entries)
            {
               var k =  await user.Files.DownloadAsync(path: "/"+ item.Name+"/preview.jpeg");
                var stream = await k.GetContentAsStreamAsync();
                MemoryStream t = new MemoryStream();
                stream.CopyTo(t);
                list.Add(new RemoteProject { name = item.Name, image = t });
            }
            return list;
        }
    }

    class PSDProject //проект
    {
        public bool local = true;
        public static event EventHandler okno;
        private void CompressFile(string dir, string dircreate) // доделать сжатие файлов!!!!!
        {
            FileStream buff = File.Open(dir, FileMode.Open);
            FileStream buff2 = File.Create(dircreate + "kek.gz");
            GZipStream compress = new GZipStream(buff, CompressionMode.Compress);
            buff2.CopyTo(compress);
        }
        public FileSystemWatcher looks = new FileSystemWatcher(); //отслеживание файла
        public int id { get; set; }
        public string name { get; set; } //полное название файла
        public string dir { get; set; } //путь проекта, на конце "/"
        public string owner_id { get; set; } //имя пользователя проекта
        public void off()
        {
            this.looks.Dispose();
        }
        public ObservableCollection<Save> Commits { get; set; } //список коммитов проекта
        public void txt(string txt)
        {
            Save ns;
           // ns = new Save("autocommit", Commits.Count);
            ns = txt.Length == 0 ? new Save("autocommit", id, Commits.Count) : new Save(txt, id, Commits.Count);
            //  CompressFile(d + n, "data/" + owner_id.Replace(':', '-') + "/" + name.Remove(name.Length - 4) + "/");
            // File.Copy(dir + n, "data/" + owner_id.Replace(':', '-') + "/" + name.Remove(name.Length - 4) + "/" + "commit_" + Commits.Count + ".psd");
            SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
            m_dbConn.Open();
            SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
            m_sqlCmd.CommandText = "INSERT INTO Commits (commit_number, Project_id, message) values (" + ns.que + "," + this.id + ",'" + ns.message + "')";
            m_sqlCmd.ExecuteNonQuery();
            m_dbConn.Close();
            AddCommit(ns);

           // var image = new MagickImage(dir + name);
            var t = File.Create("data/" + owner_id.Replace(':', '-') + "/" + name.Remove(name.Length - 4) + "/commit" + (Commits.Count-1));
            var tr = File.Open(dir+name,FileMode.Open);
            GZipStream o = new GZipStream(t, CompressionMode.Compress);
            tr.CopyTo(o);
            o.Close();
            //var zipimage = zip.CreateEntry("preview.jpg");
            //BinaryWriter sw = new BinaryWriter(zipimage.Open());
            //var kek = new MemoryStream();
            //image.ToBitmap().Save(kek, System.Drawing.Imaging.ImageFormat.Jpeg);
            //byte[] bytes = kek.GetBuffer();
            //foreach (byte o in bytes)
            //{
            //    sw.Write(o);
            //}
            //sw.Close();
            //kek.Close();
            tr.Close();
            t.Close();

        }
        public PSDProject(int i, string n, string d, string v)
        {

            id = i;
            Commits = new ObservableCollection<Save>();
            name = n;
            dir = d;
            owner_id = v;
            // d + n = full directioay
            Directory.CreateDirectory("data/" + owner_id.Replace(':', '-') + "/" + name.Remove(name.Length-4)); //замена недопустимых символов в пути
            looks.Path = d;
            looks.EnableRaisingEvents = false;        
            looks.EnableRaisingEvents = true;

            looks.Deleted += (a, b) =>
            {
                if (b.FullPath == dir+name) {
                looks.EnableRaisingEvents = false;
                looks.EnableRaisingEvents = true;
                okno(this, EventArgs.Empty);
                }
                //commit commit_window = new commit();
                //commit_window.Show();
                ////==================================================
                //commit_window.happend += (f,g) =>
                //{

                //};
                //==================================
            };
          //  if (!File.Exists(dir + name)) looks.Dispose(); 

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
        public string message { get; set; } //описание
        public int number { get; set; } //айди проекта
        public int que { get; set; } //айди коммита
        public Bitmap preview { get; set; } //превью изображениия
        public Save(string m, int n, int q)
        {
            que = q;
            message = m;
            number = n;
        }
        public Save()
        {
            message = "commit_test";
            number = 0;

        }
        public void Open(PSDProject a)
        {
            //сохранить файл в дериктории исходного проекта f
            string path = a.dir + this.message + ".psd";
            if (!File.Exists(path)) {

                var file = File.Open("data/" + a.owner_id.Replace(':', '-') + "/" + a.name.Remove(a.name.Length - 4) + "/commit" + que, FileMode.Open);
                var k = File.Create(path);
                var t = new GZipStream(file, CompressionMode.Decompress);
                t.CopyTo(k);
                t.Close();
                k.Close();
                file.Close();
            }
       
            System.Diagnostics.Process.Start(path);

        }
    }


}

