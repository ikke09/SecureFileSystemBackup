using System;
using System.IO;

namespace FileSystemBackupLib.FileSystem
{
    public class MyDirectory : FileSystemWatcher
    {
        private string _mirrorDir { get; set; }

        public MyDirectory(string mirrorDir)
        {
            _mirrorDir = mirrorDir;
        }

        public void Encrypt()
        {

        }

        public void Decrypt()
        {

        }

        public void MirrorFile(string file, WatcherChangeTypes changeType, string oldFileName = "")
        {
            if (Directory.Exists(_mirrorDir))
            {
                Directory.CreateDirectory(_mirrorDir);
            }

            string fileDir = Directory.GetParent(file).Name;
            string mirrorDir = Directory.GetParent(_mirrorDir).Name;

            bool fileInSubDir = false;
            if (!Directory.GetParent(file).Name.Equals(Directory.GetParent(_mirrorDir).Name))
            {
                CreateMissingMirrorDirSubDirectories(file);
                fileInSubDir = true;
            }

            switch (changeType)
            {
                case WatcherChangeTypes.Renamed:
                    if (File.Exists(oldFileName))
                    {
                        if (fileInSubDir)
                        {
                            File.Copy(oldFileName, String.Format(_mirrorDir + GetAllSubdirectories(file) + @"\{0}", System.IO.Path.GetFileName(oldFileName)), true);
                        }
                        else
                        {
                            File.Copy(oldFileName, String.Format(_mirrorDir + @"\{0}", System.IO.Path.GetFileName(oldFileName)), true);
                        }
                    }
                    break;
                case WatcherChangeTypes.Deleted:
                    if (fileInSubDir)
                    {
                        File.Delete(String.Format(_mirrorDir + GetAllSubdirectories(file) + @"\{0}", System.IO.Path.GetFileName(file)));
                    }
                    else
                    {
                        string abc = String.Format(_mirrorDir + @"\{0}", System.IO.Path.GetFileName(file));
                        File.Delete(String.Format(_mirrorDir + @"\{0}", System.IO.Path.GetFileName(file)));
                    }
                    break;
                default:
                    if (File.Exists(file))
                    {
                        if (fileInSubDir)
                        {
                            File.Copy(file, String.Format(_mirrorDir + GetAllSubdirectories(file) + @"\{0}", System.IO.Path.GetFileName(file)), true);
                        }
                        else
                        {
                            File.Copy(file, String.Format(_mirrorDir + @"\{0}", System.IO.Path.GetFileName(file)), true);
                        }
                    }
                    break;
            }

        }
        private string GetAllSubdirectories(string filePath)
        {
            string directoryPath = "";
            string directoryName = Directory.GetParent(filePath).Name;
            string fullDirPath = Directory.GetParent(filePath).FullName;

            while (!directoryName.Equals(Directory.GetParent(_mirrorDir + @"\").Name))
            {
                directoryPath += directoryName + @"\";
                directoryName = Directory.GetParent(fullDirPath).Name;
                fullDirPath = Directory.GetParent(fullDirPath).FullName;
            }
            string reversedFilePath = MyFile.ReverseFilePath(directoryPath);
            return reversedFilePath;
        }

        private void CreateMissingMirrorDirSubDirectories(string originalFilePath)
        {
            string subDirectories = GetAllSubdirectories(originalFilePath);
            string[] directory = subDirectories.Split('\\');

            string fullMirrorDir = _mirrorDir;
            
            foreach (string dir in directory)
            {
                fullMirrorDir += @"\" + dir;
                Directory.CreateDirectory(fullMirrorDir);
            }
        }
    }
}