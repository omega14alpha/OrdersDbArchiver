using OrdersDbArchiver.BusinessLogicLayer.Models;
using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace OrdersDbArchiver.BusinessLogicLayer.FilesWorkers
{
    public class FileInfoFactory : IFileInfoFactory
    {
        private readonly string dataFormat = "ddMMyyyy";

        public IEnumerable<FileNameModel> CreateFileInfoModels(IEnumerable<string> filePaths, string targetPath)
        {
            foreach (var path in filePaths)
            {
                yield return CreateModel(path, targetPath);
            }
        }

        private FileNameModel CreateModel(string path, string targetPath)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException();
            }

            FileInfo fileInfo = new FileInfo(path);
            var fileNameArr = fileInfo.Name.Split(new char[] { '_', '.' });
            var date = DateTime.ParseExact(fileNameArr[1], dataFormat, CultureInfo.InvariantCulture);

            return new FileNameModel()
            {
                Manager = fileNameArr[0],
                FullFileName = fileInfo.Name,
                FullFilePath = fileInfo.FullName,
                FileName = fileInfo.Name,
                TargetFilePath = Path.Combine(targetPath, fileNameArr[0], date.Year.ToString(), date.Month.ToString()),
                FullTargetFilePath = Path.Combine(targetPath, fileNameArr[0], date.Year.ToString(), date.Month.ToString(), fileInfo.Name),
            };
        }
    }
}
