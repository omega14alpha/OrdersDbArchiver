using OrdersDbArchiver.BusinessLogicLayer.FilesWorkers.Models;
using OrdersDbArchiver.BusinessLogicLayer.Interfaces;
using System;
using System.Globalization;
using System.IO;

namespace OrdersDbArchiver.BusinessLogicLayer.FilesWorkers.Factories
{
    public class FileInfoFactory : IFileInfoFactory
    {
        private readonly string _targetFileFolder;

        public FileInfoFactory(string targetFileFolder)
        {
            _targetFileFolder = targetFileFolder;
        }

        public OrderFileName CreateModel(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException();
            }

            FileInfo fileInfo = new FileInfo(fileName);
            var fileNameArr = fileInfo.Name.Split(new char[] { '_', '.' });
            var date = DateTime.ParseExact(fileNameArr[1], "ddMMyyyy", CultureInfo.InvariantCulture);

            return new OrderFileName()
            {
                Manager = fileNameArr[0],
                FullFileName = fileInfo.Name,
                FullFilePath = fileInfo.FullName,
                FileName = fileInfo.Name,
                TargetFilePath = Path.Combine(_targetFileFolder, fileNameArr[0], date.Year.ToString(), date.Month.ToString()),
                FullTargetFilePath = Path.Combine(_targetFileFolder, fileNameArr[0], date.Year.ToString(), date.Month.ToString(), fileInfo.Name),
            };
        }
    }
}
