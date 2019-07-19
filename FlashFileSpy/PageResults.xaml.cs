using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

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
            var iCount = listFlashFiles.Where(ix => ix.lstFlashFilesFound.Count > 0).Count();
            var iCount2 = 0;
            foreach (var ix in listFlashFiles)
            {
                iCount2 += ix.lstFlashFilesFound.Count;
            }
            List<FlashFolder> listX = listFlashFiles.Where(ix => ix.lstFlashFilesFound.Count > 0).ToList();

            if (iCount2 > 0)
            {
                ImageRedX.Visibility = Visibility.Visible;
                ImageGreenCheckmark.Visibility = Visibility.Collapsed;
                labelNoFlash.Visibility = Visibility.Collapsed;
                if (iCount > 1)
                {
                    txtBlock.Text = $"{iCount2} Flash files found in {iCount} files/folders:{Environment.NewLine}{Environment.NewLine}";
                }
                else if (iCount == 1)
                {
                    txtBlock.Text = $"{iCount2} Flash files found in {listX.FirstOrDefault().pathName}: {Environment.NewLine}";
                }
                foreach (FlashFolder ff in listX)
                {
                    if (iCount > 1)
                    {
                        txtBlock.Text += $"{ff.pathName}:{Environment.NewLine}{Environment.NewLine}";
                    }
                    foreach  (string s in ff.lstFlashFilesFound)
                    {
                        txtBlock.Text += $"    {s}{Environment.NewLine}";
                    }
                }
            }
            else
            {
                ImageRedX.Visibility = Visibility.Collapsed;
                ImageGreenCheckmark.Visibility = Visibility.Visible;
                labelNoFlash.Visibility = Visibility.Visible;
            }
        }

        private void btnReturn_onClick(object sender, RoutedEventArgs e)
        {
            ((App.Current as App).listFlashFiles).Clear();
            NavigationService ns = this.NavigationService;
            Uri uri = new Uri("PageStart.xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
    }
}
