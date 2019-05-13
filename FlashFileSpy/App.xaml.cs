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
        public List<string> listFlashFiles { get; set; }
        public App()
        {
            listFlashFiles = new List<string>();
        }

    }
}
