using OrdersDbArchiver.BusinessLogicLayer.Models;
using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace OrdersDbArchiver.BusinessLogicLayer.FilesWorkers
{
    internal class FileWorker : IFileWorker
    {
        public IEnumerable<string> GetFilesPath(AppConfigsModel configsModel)
        {
            if (configsModel == null)
            {
                throw new ArgumentNullException();
            }

            return Directory.GetFiles(configsModel.Folders.ObservedFolder, configsModel.Folders.ObservedFilesPattern, SearchOption.TopDirectoryOnly);
        }

        public IEnumerable<string> ReadFileData(FileNameModel fileNameModel)
        {
            if (fileNameModel == null)
            {
                throw new ArgumentNullException();
            }

            if (!File.Exists(fileNameModel.FullFilePath))
            {
                throw new FileNotFoundException();
            }

            return File.ReadLines(fileNameModel.FullFilePath);
        }

        public void FileTransfer(FileNameModel fileNameModel)
        {
            if (fileNameModel == null)
            {
                throw new ArgumentNullException();
            }

            if (!File.Exists(fileNameModel.FullFilePath))
            {
                throw new FileNotFoundException();
            }

            try
            {
                CreateFolder(fileNameModel.TargetFilePath);
                File.Move(fileNameModel.FullFilePath, fileNameModel.FullTargetFilePath);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
