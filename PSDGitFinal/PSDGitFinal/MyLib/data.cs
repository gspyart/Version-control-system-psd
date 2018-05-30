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
    class RemoteProject //проекты на dropbox
    {
        public bool fresh = false;
        public MemoryStream image { get; set; }
        public string name { get; set; }
    }

    class Data // главный класс с данными
    {
        static int load = 0;
        public Data()
        {
            UserProjects = new ObservableCollection<PSDProject>();
        }
        public delegate Task Tu(PSDProject a, Dropbox.Api.DropboxClient k);
        public ObservableCollection<PSDProject> UserProjects { get; set; }

        public static MemoryStream Decompress(FileStream e)
        {
            var k = new MemoryStream();
            var t = new GZipStream(e, CompressionMode.Decompress);
            t.CopyTo(k);
            t.Close();
            return k;
        }

        public void AddProject(PSDProject b) //добавить проект
        {
            UserProjects.Add(b);
        }
        public static void metadata(PSDProject a, FileStream k) // формирование методанных о проекте
        {
            List<Save> infojson = a.Commits.ToList(); //сериализация метаданных
            var inf = JsonConvert.SerializeObject(infojson);
            foreach (byte u in inf.ToArray())
            {
                k.WriteByte(u);
            }
            k.Close();
        }
        public static List<Save> getmetadata(PSDProject psd)
        {
            try
            {
                var file = File.Open("data/" + psd.owner_id.Replace(':', '-') + "/" + psd.name.Remove(psd.name.Length - 4) + "/metadata", FileMode.Open);
                StreamReader sr = new StreamReader(file);
                string meta = sr.ReadToEnd();
                sr.Close();
                List<Save> eq = JsonConvert.DeserializeObject<List<Save>>(meta); //данные о коммитах в бд
                return eq;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка чтения metadata файла с диска: " + ex.Message);
            }
            return new List<Save>();
        }
        //БД 
        public void DatabaseLoad(User user) //данные из БД о проектах
        {
            try
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
                            if (data.GetInt32(0) == data2.GetInt32(2))
                            {
                                var ms = Decompress(File.Open("data/" + m.owner_id.Replace(':', '-') + "/" + m.name.Remove(m.name.Length - 4) + "/commit" + o.que, FileMode.Open));
                                //o.preview = new Bitmap(ms);        ломается
                                m.AddCommit(o);
                                ms.Close();
                            }

                        }
                    }
                }
                data.Close();
                data2.Close();
                m_dbConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("DatabaseLoad error: " + ex.Message);
            }
        }
        public void DatabaseInsert(PSDProject a)  //Добавить проект в БД
        {
            SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
            m_dbConn.Open();
            SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
            m_sqlCmd.CommandText = "INSERT INTO Projects (project_name, dir, owner) values ('" + a.name + "','" + a.dir + "','" + a.owner_id + "')";
            m_sqlCmd.ExecuteNonQuery();
            m_dbConn.Close();
        }
        public void DatabaseDelete(int id)
        {

        } //удалить проект с БД
        //dropbox api
        public static void TryUpload(Tu cb, PSDProject a, Dropbox.Api.DropboxClient k) //попытка залить проект на дропбокс (не больше 1 за раз)
        {
            if (load < 1)
            {
                load++;
                Thread th = new Thread(new ThreadStart(() => { var t = Task.Run(() => cb(a, k)); t.Wait(); load--; }));
                th.Start();
            }
        }
        public async Task ProjectLoad(PSDProject t, DropboxClient user) //upload on dropbox
        {

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
                await user.Files.UploadAsync(dirpath + "/preview.jpeg", WriteMode.Overwrite.Instance, body: fo);
                File.Delete(path);
                fo.Close();
                using (var infons = File.Create("data/" + t.owner_id.Replace(':', '-') + "/" + t.name.Remove(t.name.Length - 4) + "/metadata"))
                {
                    metadata(t, infons);

                }
                using (var infons = File.Open("data/" + t.owner_id.Replace(':', '-') + "/" + t.name.Remove(t.name.Length - 4) + "/metadata", FileMode.Open))
                {
                    await user.Files.UploadAsync(dirpath + "/metadata", WriteMode.Overwrite.Instance, body: infons);
                    infons.Close();
                }
            }
            try
            {
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
            var lst = await user.Files.ListFolderAsync("/" + rp.name);
            int j = lst.Entries.Count; //количество коммитов на дропбоксе
            SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
            m_dbConn.Open();
            SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
            m_sqlCmd.CommandText = "select id from Projects where project_name = " + "'" + rp.name + ".psd'";
            SQLiteDataReader data = m_sqlCmd.ExecuteReader();
            data.Read();
            int id = data.GetInt32(0);  //айди проекта в бд
            data.Close();
            m_sqlCmd.CommandText = "select * from Commits where Project_id = " + id;
            SQLiteDataReader data2 = m_sqlCmd.ExecuteReader();
            data2.Read();
            data2.Close();
            m_dbConn.Close();
        } //Скачать проекты с Dropbox
        public async Task<List<RemoteProject>> GetProjects(DropboxClient user)
        {
            RemoteProject y = new RemoteProject();
            List<RemoteProject> list = new List<RemoteProject>();
            var lst = await user.Files.ListFolderAsync(string.Empty);
            foreach (var item in lst.Entries)
            {
                try
                {
                    var md = await user.Files.DownloadAsync(path: "/" + item.Name + "/metadata"); //проверка на наличие metadata файла на дропбоксе
                    string file = await md.GetContentAsStringAsync();
                    List<Save> eq = JsonConvert.DeserializeObject<List<Save>>(file); //данные о коммитах в Dropbox
                    foreach (PSDProject u in UserProjects)
                    {
                        List<Save> mydate = getmetadata(u); //данные о коммитах на компе
                        if (eq.Count < mydate.Count) y.fresh = false;
                        else y.fresh = true;
                    }
                }
                catch (DropboxException e)
                {
                    MessageBox.Show("Ошибка получения списков проектов с Dropbox: " + e.Message);
                    continue;
                }
                try
                {
                    var k = await user.Files.DownloadAsync(path: "/" + item.Name + "/preview.jpeg");
                    var stream = await k.GetContentAsStreamAsync();
                    MemoryStream t = new MemoryStream();
                    stream.CopyTo(t);
                    y.name = item.Name;
                    y.image = t;
                    list.Add(y);
                }
                catch (DropboxException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return list;
        } //Получить список проектов с Dropbox
    }

    class PSDProject //проект
    {
        public bool local = true;
        public static event EventHandler okno;
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
            try
            {
                Save ns;
                ns = txt.Length == 0 ? new Save("autocommit", id, Commits.Count) : new Save(txt, id, Commits.Count);
                SQLiteConnection m_dbConn = new SQLiteConnection("Data Source=projects_database.db; Version=3;");
                m_dbConn.Open();
                SQLiteCommand m_sqlCmd = m_dbConn.CreateCommand();
                m_sqlCmd.CommandText = "INSERT INTO Commits (commit_number, Project_id, message) values (" + ns.que + "," + this.id + ",'" + ns.message + "')";
                m_sqlCmd.ExecuteNonQuery();
                m_dbConn.Close();
                AddCommit(ns);
                var t = File.Create("data/" + owner_id.Replace(':', '-') + "/" + name.Remove(name.Length - 4) + "/commit" + (Commits.Count - 1));
                var tr = File.Open(dir + name, FileMode.Open);
                GZipStream o = new GZipStream(t, CompressionMode.Compress);
                tr.CopyTo(o);
                o.Close();
                tr.Close();
                t.Close();
               // ломается
               // var ms = Data.Decompress(File.Open("data/" + owner_id.Replace(':', '-') + "/" + name.Remove(name.Length - 4) + "/commit" + ns.que, FileMode.Open));
               // ns.preview = new Bitmap(ms);

                using (var infons = File.Create("data/" + this.owner_id.Replace(':', '-') + "/" + this.name.Remove(this.name.Length - 4) + "/metadata"))
                {
                    Data.metadata(this, infons);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления нового коммита: " + ex.Message);
            }
        } //добавление нового коммита
        public PSDProject(int i, string n, string d, string v)
        {
            id = i;
            Commits = new ObservableCollection<Save>();
            name = n;
            dir = d;
            owner_id = v;
            // d + n = full directioay
            try
            {
                looks.Path = d;
                looks.EnableRaisingEvents = false;
                looks.EnableRaisingEvents = true;
                looks.Deleted += (a, b) =>
                {
                    if (b.FullPath == dir + name)
                    {
                        looks.EnableRaisingEvents = false;
                        looks.EnableRaisingEvents = true;
                        okno(this, EventArgs.Empty);
                    }
                };
                Directory.CreateDirectory("data/" + owner_id.Replace(':', '-') + "/" + name.Remove(name.Length - 4)); //замена недопустимых символов в пути
            }
            catch (Exception message)
            {
                MessageBox.Show("Ошибка в конструкторе PSDProject коммита: " + message.Message);
            }

            using (var infons = File.Create("data/" + this.owner_id.Replace(':', '-') + "/" + this.name.Remove(this.name.Length - 4) + "/metadata"))
            {
                Data.metadata(this, infons);
            }
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
            string path = a.dir + this.message + ".psd";   //сохранить файл в дериктории исходного проекта f
            if (!File.Exists(path))
            {
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

