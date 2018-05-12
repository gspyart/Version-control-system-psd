using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using ImageMagick;
using System.IO;
using DropbBoxLogIn;
namespace dp
{
    class Data
    {
        
        public List<PSDFile> UserProjects = new List<PSDFile>();
        public void DownloadFiles()
        {

        }
        public void Unload()
        {

        }
        public Data()
        {
            Directory.CreateDirectory("data");
        }
        public void Update(Auth.UserData userData)
        {

        }

    }



    class Save
    {
        string name { get; set; }
        public Save()
        {
            name = "commit_test";

        }
    }

    class PSDFile
    {
        static int savenum = 0;
        public FileSystemWatcher looks = new FileSystemWatcher(); //отслеживание файла
        public string name { get; set; } //название проекта
        public string dir { get; set; } //путь проекта
        public string id { get; set; } //уникальный id проекта
        public List<Save> commits = new List<Save>(); //список коммитов проекта
        public PSDFile(string n, string d, string v)
        {
            name = n;
            dir = d;
            id = v;
            looks.Path = dir;
            looks.EnableRaisingEvents = true;
            Directory.CreateDirectory("data/" + name);
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
            commits.Add(b);


        }  //добавить коммит

        public void SortCommits() //сортировать коммиты
        {

        }
    }

}

