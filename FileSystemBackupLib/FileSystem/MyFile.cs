namespace FileSystemBackupLib.FileSystem
{
    static class MyFile
    {
        public static void Encrypt()
        {

        }

        public static void Decrypt()
        {

        }

        public static string ReverseFilePath(string filePath)
        {
            string[] arrFilePath = filePath.Split('\\');
            string reversedFilePath = "";
            for (int i = arrFilePath.Length - 1; i >= 0; i--)
            {
                if (!arrFilePath[i].Equals(""))
                {
                    reversedFilePath += arrFilePath[i] + @"\";
                }
            }
            return reversedFilePath;
        }
    }
}
