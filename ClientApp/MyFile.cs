using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class MyFile
    {
        public string FileContent { get; set; } //содержимое файла
        public string FilePath { get; set; }    //путь к файлу

        public MyFile(string content, string path)
        {
            FileContent = content;
            FilePath = path;
        }
    }
}
