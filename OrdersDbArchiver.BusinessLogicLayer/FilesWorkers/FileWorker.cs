using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers.Models;
using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace OrdersDbArchiver.BusinessLogicLayer.FilesWorkers
{
    internal class FileWorker : IFileWorker
    {
        public IEnumerable<OrderFileName> GetFilesPath(string folderPath, string fileExtension, IFileInfoFactory fileInfoFactory)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new ArgumentNullException();
            }

            var files = Directory.GetFiles(folderPath, fileExtension, SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                yield return fileInfoFactory.CreateModel(file);
            }
        }

        public IEnumerable<string> ReadFileData(OrderFileName fileNameModel)
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

        public void FileTransfer(OrderFileName fileNameModel)
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
