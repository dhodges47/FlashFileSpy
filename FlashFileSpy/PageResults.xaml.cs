using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlashFileSpy
{
    /// <summary>
    /// Interaction logic for PageResults.xaml
    /// </summary>
    public partial class PageResults : Page
    {
        public PageResults()
        {
            InitializeComponent();
            displayresults();
        }

        private void displayresults()
        {
            var listFlashFiles = (App.Current as App).listFlashFiles;
            foreach (var sFlashFilePath in listFlashFiles)
            {
                txtBlock.Inlines.Add($"{sFlashFilePath}\n");

            }
        }

        private void btnReturn_onClick(object sender, RoutedEventArgs e)
        {
            NavigationService ns = this.NavigationService;
            Uri uri = new Uri("PageStart.xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
    }
}
