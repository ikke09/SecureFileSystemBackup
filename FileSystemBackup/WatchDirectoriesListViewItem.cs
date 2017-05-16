namespace FileSystemBackupInterface
{
    public class WatchDirectoriesListViewItem
    {
        public WatchDirectoriesListViewItem(string dirName, string dirPath)
        {
            DirName = dirName;
            DirPath = dirPath;
        }

        public string DirName { get; private set; }
        public string DirPath { get; private set; }
    }
}
