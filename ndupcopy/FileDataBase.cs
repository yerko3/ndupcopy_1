using System;
using System.Collections.Generic;
using System.IO;


namespace ndupcopy
{
    public class FileDataBase
    {
        private List<string> _files = new List<string>();
        private List<string> _sourceDirectories = new List<string>();


        public void AddSourceDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"El directorio '{directoryPath}' no existe.");

            _sourceDirectories.Add(directoryPath);
        }
        public void ReadFilesFromSourceDirectories()
        {
            foreach (string sourceDir in _sourceDirectories)
            {
                try
                {
                    string[] files = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
                    _files.AddRange(files);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al leer archivos desde '{sourceDir}': {ex.Message}");
                }
            }
        }

        public List<string> GetFiles()
        {
            return new List<string>(_files);
        }

    }
}
