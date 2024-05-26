using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ndupcopy
{
    public class Utils  : IDisposable
    {

        public static long GetFileSize(string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                return fileInfo.Length;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el tamaño del archivo {ex.Message}");
                return -1;
            }
        }
        public static bool CompareFileContents(string FilePath1, string FilePath2)
        {
            using (Stream s1 = File.OpenRead(FilePath1))
            using (Stream s2 = File.OpenRead(FilePath2))
            {
                byte[] buffer1 = new byte[2048];
                byte[] buffer2 = new byte[2048];

                while (true)
                {
                    int br1 = s1.Read(buffer1, 0, buffer1.Length);
                    int br2 = s2.Read(buffer2, 0, buffer2.Length);

                    if (br1 == 0 && br2 == 0)
                        break;

                    for (int i = 0; i < br1; i++)
                    {
                        if (buffer1[i] != buffer2[i])
                            return false;
                    }
                }
            }
            return true;
        }
        public static bool CompareFileSizes(string FilePath1, string FilePath2)
        {
            return GetFileSize(FilePath1) == GetFileSize(FilePath2);
        }
        public static bool CompareFilesHashes(FileHash fileHash1, FileHash fileHash2)
        {
            if (fileHash1 == null || fileHash2 == null)
                return false;

            return fileHash1 == fileHash2;
        }

        public static bool VerifyFiles(string filePath1, string filePath2)
        {

            if (!CompareFileSizes(filePath1, filePath2))
                return false;

            if (!CompareFileContents(filePath1, filePath2))
                return false;


            FileHash fileHash1 = new FileHash(filePath1);
            FileHash fileHash2 = new FileHash(filePath2);
            if (!CompareFilesHashes(fileHash1, fileHash2))
                return false;

            // Si todas las comparaciones son consistentes, los archivos son iguales
            return true;
        }
        public void CopyUniqueFiles(List<string> files, string destinationFolder)
        {
            HashSet<string> existingFileNames = GetExistingFileNames(destinationFolder);

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destinationFilePath = Path.Combine(destinationFolder, fileName);

                if (FileExistsInDestination(fileName, existingFileNames))
                    continue;

                if (IsIdenticalFile(file, existingFileNames, destinationFolder))
                    continue;

                CopyFile(file, destinationFilePath);
                existingFileNames.Add(fileName);
            }
        }

        private  static HashSet<string> GetExistingFileNames(string destinationFolder)
        {
            return new HashSet<string>(Directory.GetFiles(destinationFolder).Select(Path.GetFileName));
        }

        private static bool FileExistsInDestination(string fileName, HashSet<string> existingFileNames)
        {
            if (existingFileNames.Contains(fileName))
            {
                Console.WriteLine($"El archivo '{fileName}' ya existe en la carpeta de destino.");
                return true;
            }
            return false;
        }
        private  static bool IsIdenticalFile(string file, HashSet<string> existingFileNames, string destinationFolder)
        {
            string fileName = Path.GetFileName(file);
            string fileHash = FileHash.GetFileHash(file);

            foreach (string existingFile in existingFileNames)
            {
                string existingFilePath = Path.Combine(destinationFolder, existingFile);
                string existingFileHash = FileHash.GetFileHash(existingFilePath);

                if (fileHash == existingFileHash && !CompareFiles(file, existingFilePath))
                {
                    Console.WriteLine($"El archivo '{fileName}' es idéntico a '{existingFile}' en la carpeta de destino.");
                    return true;
                }
            }
            return false;
        }

        private static void CopyFile(string sourceFilePath, string destinationFilePath)
        {
            try
            {
                File.Copy(sourceFilePath, destinationFilePath);
                Console.WriteLine($"Archivo copiado: {Path.GetFileName(sourceFilePath)} -> {destinationFilePath}");
            }
            catch (IOException)
            {
                Console.WriteLine($"Error: El archivo '{Path.GetFileName(sourceFilePath)}' ya existe en la carpeta de destino.");
            }
        }

        private static bool CompareFiles(string filePath1, string filePath2)
        {
            return VerifyFiles(filePath1, filePath2);
        }

        public void Dispose()
        {
        }
    }
}
