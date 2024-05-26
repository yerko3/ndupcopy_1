using System.Collections.Generic;

namespace ndupcopy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string sourceDirectory1 = args[0];
            string sourceDirectory2 = args[1];
            string destinationDirectory = args[2];

            FileDataBase fileReader = new FileDataBase();
            fileReader.AddSourceDirectory(sourceDirectory1);
            fileReader.AddSourceDirectory(sourceDirectory2);

            fileReader.ReadFilesFromSourceDirectories();

            List<string> files = fileReader.GetFiles();

            using (Utils fileCopier = new Utils())
            {
                fileCopier.CopyUniqueFiles(files, destinationDirectory);
            }
        }
    }
    
}
