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
    // Created by Outermost Software, LLC 2019
    public partial class PageStart : Page
    {
        string[] extensions = new[] { ".swf", ".fla", ".flv" }; // these file extensions are for flash files
        public PageStart()
        {
            InitializeComponent();
            labelVersion.Content = $"Version {Properties.Settings.Default.Version}"; ;
            string author = Properties.Settings.Default.CreatedBy;
        }
        private void onDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // we handle this in onDropRectangle
            }
        }
        /// <summary>
        /// Drag/Drop event for when the user has dragged one or more folders and/or zip files to the drop rectangle, to be scanned
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onDropRectangle(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
               
                var listFlashFiles = (App.Current as App).listFlashFiles;
                foreach (string file in (string[])e.Data.GetData(DataFormats.FileDrop))
                {
                    var flashFolder = new FlashFolder();
                    flashFolder.pathName = file;
                    listFlashFiles.Add(flashFolder);
                    Debug.WriteLine($"  File={file}");
                    FileInfo fi = new FileInfo(file);
                    if (fi.Attributes == FileAttributes.Directory)
                    {
                        // it's a folder
                        // traverse the folder structure and scan the contents for flash files
                        Debug.WriteLine("It's a folder");
                        DirectoryInfo di = new DirectoryInfo(fi.FullName);
                        if (folderScan(di, false, flashFolder))
                        {
                            //MessageBox.Show($"{(App.Current as App).listFlashFiles.Count} Flash file(s) found");
                            // list of files is in listFlashFiles, and PageResults will display them
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
                                    flashFolder.lstFlashFilesFound.Add(sZippedFile.FullName);

                                }
                            }
                        }
                        if ((App.Current as App).listFlashFiles.Count > 0)
                        {
                            // MessageBox.Show($"{(App.Current as App).listFlashFiles.Count} Flash file(s) found");
                            // list of files is in listFlashFiles, and PageResults will display them

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
    /// <summary>
    /// Button event for when the user has manually selected a zip file to be scanned
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
        private void btnFileExplorer_onClick(object sender, RoutedEventArgs e)
        {
            var listFlashFiles = (App.Current as App).listFlashFiles;
            listFlashFiles.Clear();
            // for opening a zip file
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = "zip";
            openFileDialog.Filter = "zip files(*.zip)|*.zip|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string sPath = openFileDialog.FileName;
                FileInfo fi = new FileInfo(sPath);
                var flashFolder = new FlashFolder();
                flashFolder.pathName = sPath;
                listFlashFiles.Add(flashFolder);
                using (ZipArchive archive = ZipFile.OpenRead(fi.FullName))
                {
                    foreach (ZipArchiveEntry sZippedFile in archive.Entries)
                    {
                        if (bIsFlashFile(sZippedFile.FullName))
                        {
                            flashFolder.lstFlashFilesFound.Add(sZippedFile.FullName);
                        }
                    }
                }
                if ((App.Current as App).listFlashFiles.Count > 0)
                {
                    NavigationService ns = this.NavigationService;
                    Uri uri = new Uri("PageResults.xaml", UriKind.Relative);
                    this.NavigationService.Navigate(uri);
                }
            }
        }
        /// <summary>
        /// Button event for when the user has manually selected a folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFolderExplorer_onClick(object sender, RoutedEventArgs e)
        {
            var listFlashFiles = (App.Current as App).listFlashFiles;
            listFlashFiles.Clear();
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string sPath = folderBrowserDialog.SelectedPath;
                var flashFolder = new FlashFolder();
                flashFolder.pathName = sPath;
                listFlashFiles.Add(flashFolder);
                DirectoryInfo di = new DirectoryInfo(sPath);
                if (folderScan(di, false, flashFolder))
                {
                    //MessageBox.Show($"{(App.Current as App).listFlashFiles.Count} Flash file(s) found");
                    NavigationService ns = this.NavigationService;
                    Uri uri = new Uri("PageResults.xaml", UriKind.Relative);
                    this.NavigationService.Navigate(uri);
                    // list of files is in listFlashFiles, and PageResults will display them
                }
            }
        }
        /// <summary>
        /// Scan a folder, looking for flashfiles.
        /// </summary>
        /// <param name="di"></param>
        /// <param name="bContainsFlash"></param>
        /// <param name="flashFolder"></param>
        /// <returns></returns>
        private bool folderScan(DirectoryInfo di, bool bContainsFlash, FlashFolder flashFolder)
        {
            bool b = bContainsFlash;
            var listFlashFiles = (App.Current as App).listFlashFiles;
            FileInfo[] filesFiltered =
                           di.EnumerateFiles()
                                .Where(f => extensions.Contains(f.Extension.ToLower()))
                                .ToArray();
            foreach (FileInfo f in filesFiltered)
            {
                flashFolder.lstFlashFilesFound.Add(f.FullName);
                b = true;
            }
            var diSub = di.EnumerateDirectories();
            foreach (DirectoryInfo dii in diSub)
            {
                b = folderScan(dii, b, flashFolder);
            }
            return b;
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
    }
}
