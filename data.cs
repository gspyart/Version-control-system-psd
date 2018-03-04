using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
namespace dp
{
    public class info //информация о коммите
    {
        string name { get; set; }
        DateTime t { get { return this.t; } set { this.t = value; } }
        string descr { get; set; }
        public info(string n, string o)
        {
            name = n;
            t = new DateTime();
            descr = o;
        }
    }

    class Saves
    {
        public List<info> commits = new List<info>(); //массив коммитов
        // загрузка коммитов
        // удаление коммитов
        // 
    }

    class PSDFile
    {
        public FileSystemWatcher looks = new FileSystemWatcher(); //отслеживание файла
        public string name { get; set; } //название проекта
        public string dir { get; set; } //путь проекта
        public string id { get; set; } //уникальный id проекта
        public Saves commit = new Saves(); //список коммитов проекта
        public PSDFile(string n, string d, string v)
        {
            name = n;
            dir = d;
            id = v;
            looks.Path = dir;
            looks.EnableRaisingEvents = true;
        }
        public void AddCommit(info b)
        {
            commit.commits.Add(b);
        }  //добавить коммит
        public void SortCommits() //сортировать коммиты
        {

        }
    }

}

