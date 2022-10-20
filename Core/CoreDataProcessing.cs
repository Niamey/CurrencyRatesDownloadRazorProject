using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public class CoreDataProcessing
    {
        HttpClientWrapper dataFromHttpClient = new HttpClientWrapper();

        public async Task DownloadCurrencyRates()
        {
            CreateNewDirectory(Shared.path);
            ReplaceIllegalFilenameCharacters(Shared.name);

            try
            {
                var data = await dataFromHttpClient.ProcessData();
                WriteTextToFile(Shared.path, data);

            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }
            DeleteDirectory(Shared.path);
        }

        public static string[] GetFilesFromDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
                return null;

            var files = Directory.GetFiles(dirPath);
            return files;
        }

        public static void DeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
                return;

            var files = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(path, false);
        }

        public static string ReplaceIllegalFilenameCharacters(string filename)
        {
            return Path.GetInvalidFileNameChars().Aggregate(filename, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        public static void CreateNewDirectory(string path)
        {

            var ExistingDirectory = Directory.Exists(path);
            if (ExistingDirectory == false)
            {
                Directory.CreateDirectory(path);
            }

        }

        public static void WriteTextToFile(string path, string text)
        {
            if (Directory.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path + "CurrencyRates.txt"))
                {
                    sw.WriteLine(text);
                }
            }
        }

        public static bool CheckIfFileExists(string path)
        {
            var documents = Directory.GetFiles(path);
            bool isFileEmpty = true;

            foreach (var document in documents)
            {
                string[] lines = File.ReadAllLines(Path.Combine(path, document));

                if (lines.Length == 0)
                {
                    return true;
                }
                else
                {
                    isFileEmpty = false;

                }
            }

            return isFileEmpty;
        }

        public static bool CheckIfFileExistsInDownloadFolder(string fileName)
        {
            var downloadFolderPath = GetPathFromAppSettings("DownloadFolder");
            var filePath = Path.Combine(downloadFolderPath, fileName);
            var res = File.Exists(filePath);

            return res;
        }

        public static bool WaitAndCheckIfFileExistsInDownloadFolder(string fileName, int iterations = 10)
        {
            for (int i = 0; i < iterations; i++)
            {
                if (CheckIfFileExistsInDownloadFolder(fileName))
                    return true;
                Thread.Sleep(500);
            }
            return false;
        }

        public static string GetPathFromAppSettings(string key)
        {
            var path = ConfigurationManager.AppSettings[key];
            if (!Path.IsPathRooted(path))
            {
                string theDirectory = AppDomain.CurrentDomain.BaseDirectory;
                path = Path.Combine(theDirectory, path);
            }

            return path;
        }
    }
}

