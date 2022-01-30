using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class TxtFileService : IFileService
    {
        public string Open(string fileName)
        {
            string text = string.Empty;
            using (StreamReader streamReader = File.OpenText(fileName))
            {
                text = streamReader.ReadToEnd();
                Console.WriteLine(text);
            }
            return text;
        }
    }
}
