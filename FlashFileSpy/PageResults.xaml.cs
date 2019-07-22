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
            var iCountSubmittedFoldersWithFlash = listFlashFiles.Where(ix => ix.lstFlashFilesFound.Count > 0).Count();
            var iCountFlashFilesFound = 0;
            foreach (var ix in listFlashFiles)
            {
                iCountFlashFilesFound += ix.lstFlashFilesFound.Count;
            }
            if (iCountFlashFilesFound == 0)
            {
                ImageRedX.Visibility = Visibility.Collapsed;
                ImageGreenCheckmark.Visibility = Visibility.Visible;
                labelNoFlash.Visibility = Visibility.Visible;
                return;
            }
            List<FlashFolder> listSubmittedFoldersWithFlash = listFlashFiles.Where(ix => ix.lstFlashFilesFound.Count > 0).ToList();
            foreach (FlashFolder ix in listFlashFiles)
            {
                if (ix.lstFlashFilesFound.Count != 1)
                {
                    txtBlock.Text += $"{ix.lstFlashFilesFound.Count} Flash files found in {ix.pathName}{Environment.NewLine}";
                }
                else
                {
                    txtBlock.Text += $"{ix.lstFlashFilesFound.Count} Flash file found in {ix.pathName}{Environment.NewLine}";
                }
            }
            txtBlock.Text += Environment.NewLine;

            ImageRedX.Visibility = Visibility.Visible;
            ImageGreenCheckmark.Visibility = Visibility.Collapsed;
            labelNoFlash.Visibility = Visibility.Collapsed;

            foreach (FlashFolder ff in listSubmittedFoldersWithFlash)
            {
                if (iCountSubmittedFoldersWithFlash > 1)
                {
                    txtBlock.Text += $"{ff.pathName}:{Environment.NewLine}{Environment.NewLine}";
                }
                foreach (string s in ff.lstFlashFilesFound)
                {
                    txtBlock.Text += $"    {s}{Environment.NewLine}";
                }
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
