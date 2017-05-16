using FileSystemBackupLib.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Forms;

namespace FileSystemBackupInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*Directory.GetParent(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName).FullName + @"\Documents"*/
        private string _appDataPath = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName + @"\SecureFileSystemBackup";
        private List<MyDirectory> _watchDirectoryList;

        private static NotifyIcon _notificationIcon;
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (!Directory.Exists(_appDataPath))
            {
                Directory.CreateDirectory(_appDataPath);
            }

            _watchDirectoryList = new List<MyDirectory>();

            _notificationIcon = new NotifyIcon();
            _notificationIcon.Icon = new System.Drawing.Icon(@"..\..\filesystem_5De_icon.ico");
            _notificationIcon.Visible = true;
            _notificationIcon.Click +=
                delegate (object sender, EventArgs args)
                {
                    Show();
                    WindowState = WindowState.Normal;
                };

        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void AddNewWatchDirectory(string directory, string mirrorDir)
        {
            // Create a new MyDirectory and set its properties.
            MyDirectory watcher = new MyDirectory(mirrorDir);
            watcher.Path = directory;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.*";
            watcher.IncludeSubdirectories = true;

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnFileChanged);
            watcher.Created += new FileSystemEventHandler(OnFileChanged);
            watcher.Deleted += new FileSystemEventHandler(OnFileChanged);
            watcher.Renamed += new RenamedEventHandler(OnFileRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
            _watchDirectoryList.Add(watcher);
        }

        private static void ShowNotificationPopup(string fileName, WatcherChangeTypes change)
        {
            switch (change)
            {
                case WatcherChangeTypes.Changed:
                    _notificationIcon.BalloonTipTitle = "File Changed";
                    _notificationIcon.BalloonTipText = String.Format("File {0} has been changed", fileName);
                    _notificationIcon.ShowBalloonTip(1000);
                    break;
                case WatcherChangeTypes.Deleted:
                    _notificationIcon.BalloonTipTitle = "File Deleted";
                    _notificationIcon.BalloonTipText = String.Format("File {0} has been deleted", fileName);
                    _notificationIcon.ShowBalloonTip(1000);
                    break;
                case WatcherChangeTypes.Renamed:
                    _notificationIcon.BalloonTipTitle = "File Renamed";
                    _notificationIcon.BalloonTipText = String.Format("File {0} has been renamed", fileName);
                    _notificationIcon.ShowBalloonTip(1000);
                    break;
            }
        }

        // Define the event handlers.
        private static void OnFileChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
            ((MyDirectory)source).MirrorFile(e.FullPath, e.ChangeType);
            ShowNotificationPopup(e.FullPath, e.ChangeType);
        }

        private static void OnFileRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            ((MyDirectory)source).MirrorFile(e.OldFullPath, e.ChangeType, e.FullPath);
            ShowNotificationPopup(e.FullPath, e.ChangeType);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
            base.OnStateChanged(e);
        }

        private void AddWatchDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (DirPath.Text != "")
            {
                Directory.CreateDirectory(_appDataPath + @"\" + Directory.GetParent(DirPath.Text + @"\").Name);
                AddNewWatchDirectory(DirPath.Text, _appDataPath + @"\" + Directory.GetParent(DirPath.Text + @"\").Name);
                watchDirectoryList.Items.Add(new WatchDirectoriesListViewItem(Directory.GetParent(DirPath.Text + @"\").Name, DirPath.Text));
                DirPath.Text = "";
            }
        }

        private void ButtonClickChooseDirectory(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    DirPath.Text = fbd.SelectedPath;
                }
            }
        }
    }
}
