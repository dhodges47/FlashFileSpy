using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace FlashFileSpy
{
    public partial class PageStart : Page
    {
        string[] extensions = new[] { ".swf", ".fla", ".flv" };
        public PageStart()
        {
            InitializeComponent();
        }
        private void onDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                
            }
        }

        private void onDropRectangle(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var listFlashFilesx = (App.Current as App).listFlashFiles;
                listFlashFilesx.Clear();
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
                    Debug.WriteLine($"  File={file}");
                }

                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.Attributes == FileAttributes.Directory)
                    {
                        // it's a folder
                        // traverse the folder structure and scan the contents for flash files
                        Debug.WriteLine("It's a folder");
                        DirectoryInfo di = new DirectoryInfo(fi.FullName);
                        if (folderScan(di, false))
                        {
                            MessageBox.Show($"{(App.Current as App).listFlashFiles.Count} Flash file(s) found");
                            // list of files is in listFlashFiles
                        }

                    }
                    else if (fi.Attributes == FileAttributes.Archive || fi.Extension.ToLower() == ".zip")
                    {
                        // it's a zip file
                        Debug.WriteLine("It's a zip file");
                        using (ZipArchive archive = ZipFile.OpenRead(fi.FullName))
                        {
                            foreach (ZipArchiveEntry sZippedFile in archive.Entries)
                            {
                                if (bIsFlashFile(sZippedFile.FullName))
                                {
                                    (App.Current as App).listFlashFiles.Add(sZippedFile.FullName);

                                }
                            }
                        }
                        if ((App.Current as App).listFlashFiles.Count > 0)
                        {
                           // MessageBox.Show($"{(App.Current as App).listFlashFiles.Count} Flash file(s) found");

                        }
                    }
                    else
                    {
                        //it's a regular file
                        Debug.WriteLine("It's a regular file");
                        if (bIsFlashFile(fi))
                        {
                            MessageBox.Show("Flash file found");
                        }
                        else
                        {
                            MessageBox.Show("Not a flash file");
                        }
                        return;

                    }
                }
                NavigationService ns = this.NavigationService;
                Uri uri = new Uri("PageResults.xaml", UriKind.Relative);
                this.NavigationService.Navigate(uri);

            }
        }

        private void onDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
        private bool bIsFlashFile(FileInfo fi)
        {
            string s = fi.Extension.ToLower();
            if (extensions.Contains(s))
            {
                return true;
            }
            return false;
        }
        private bool bIsFlashFile(string sPath)
        {
            foreach (string sExt in extensions)
            {
                if (sPath.EndsWith(sExt, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        private bool folderScan(DirectoryInfo di, bool bContainsFlash)
        {
            bool b = bContainsFlash;
            FileInfo[] filesFiltered =
                           di.EnumerateFiles()
                                .Where(f => extensions.Contains(f.Extension.ToLower()))
                                .ToArray();
            foreach (FileInfo f in filesFiltered)
            {
                (App.Current as App).listFlashFiles.Add(f.FullName);
                b = true;
            }
            var diSub = di.EnumerateDirectories();
            foreach (DirectoryInfo dii in diSub)
            {
                b = folderScan(dii, b);
            }
            return b;
        }

        private void btnFileExplorer_onClick(object sender, RoutedEventArgs e)
        {
            // for opening a zip file
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = "zip";
            openFileDialog.Filter = "zip files(*.zip)|*.zip|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string sPath = openFileDialog.FileName;
                FileInfo fi = new FileInfo(sPath);
                using (ZipArchive archive = ZipFile.OpenRead(fi.FullName))
                {
                    foreach (ZipArchiveEntry sZippedFile in archive.Entries)
                    {
                        if (bIsFlashFile(sZippedFile.FullName))
                        {
                            (App.Current as App).listFlashFiles.Add(sZippedFile.FullName);

                        }
                    }
                }
                if ((App.Current as App).listFlashFiles.Count > 0)
                {
                    MessageBox.Show($"{(App.Current as App).listFlashFiles.Count} Flash file(s) found");
                    NavigationService ns = this.NavigationService;
                    Uri uri = new Uri("PageResults.xaml", UriKind.Relative);
                    this.NavigationService.Navigate(uri);

                }

            }
        }

        private void btnFolderExplorer_onClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string sPath = folderBrowserDialog.SelectedPath;
                DirectoryInfo di = new DirectoryInfo(sPath);
                if (folderScan(di, false))
                {
                    MessageBox.Show($"{(App.Current as App).listFlashFiles.Count} Flash file(s) found");
                    NavigationService ns = this.NavigationService;
                    Uri uri = new Uri("PageResults.xaml", UriKind.Relative);
                    this.NavigationService.Navigate(uri);
                    // list of files is in listFlashFiles
                }
            }

        }
    }
}
