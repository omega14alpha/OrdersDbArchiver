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
            if (filePaths == null)
            {
                throw new ArgumentNullException($"Argument '{nameof(filePaths)}' cannot be equals null.");
            }

            if (string.IsNullOrWhiteSpace(targetPath))
            {
                throw new ArgumentNullException($"Argument '{nameof(targetPath)}' cannot be empty or equals null.");
            }

            foreach (var path in filePaths)
            {
                yield return CreateModel(path, targetPath);
            }
        }

        private FileNameModel CreateModel(string path, string targetPath)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException($"Argument '{nameof(path)}' cannot be empty or equals null.");
            }

            if (string.IsNullOrWhiteSpace(targetPath))
            {
                throw new ArgumentNullException($"Argument '{nameof(targetPath)}' cannot be empty or equals null.");
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
