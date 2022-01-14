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
                throw new ArgumentNullException($"Argument '{nameof(configsModel)}' cannot be equals null.");
            }

            return Directory.GetFiles(configsModel.Folders.ObservedFolder, configsModel.Folders.ObservedFilesPattern, SearchOption.TopDirectoryOnly);
        }

        public IEnumerable<string> ReadFileData(FileNameModel fileNameModel)
        {
            if (fileNameModel == null)
            {
                throw new ArgumentNullException($"Argument '{nameof(fileNameModel)}' cannot be equals null.");
            }

            if (!File.Exists(fileNameModel.FullFilePath))
            {
                throw new FileNotFoundException($"File {nameof(fileNameModel.FullFilePath)} not found.");
            }

            return File.ReadLines(fileNameModel.FullFilePath);
        }

        public void FileTransfer(FileNameModel fileNameModel)
        {
            if (fileNameModel == null)
            {
                throw new ArgumentNullException($"Argument '{nameof(fileNameModel)}' cannot be equals null.");
            }

            if (!File.Exists(fileNameModel.FullFilePath))
            {
                throw new FileNotFoundException($"File {nameof(fileNameModel.FullFilePath)} not found.");
            }

            CreateFolder(fileNameModel.TargetFilePath);
            File.Move(fileNameModel.FullFilePath, fileNameModel.FullTargetFilePath, true);
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
