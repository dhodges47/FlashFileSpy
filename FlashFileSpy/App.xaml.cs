using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FlashFileSpy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public List<FlashFolder> listFlashFiles { get; set; }
        public App()
        {
            listFlashFiles = new List<FlashFolder>();
        }

    }
    public class FlashFolder
    {
        public FlashFolder ()
        {
            lstFlashFilesFound = new List<string>();
        }
        public string pathName { get; set; }
        public List<string> lstFlashFilesFound { get; set; }
    }
}
