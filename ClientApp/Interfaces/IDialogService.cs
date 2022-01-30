using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public interface IDialogService
    {
        void ShowMessage(string message);
        string[] FilePaths { get; set; }
        bool OpenFileDialog();
    }
}
