using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using VeeamGzip.Constants;
using VeeamGzip.Helpers;
using VeeamGzip.Interfaces;

namespace VeeamGzip.Services
{
    public class Compress : ICompressible
    {
        private static readonly int BlockSize = 1024;

        private readonly ILogger<Compress> _logger;

        public Compress(ILogger<Compress> logger)
        {
            _logger = logger;
        }

        public int Execute(string existingFile, string fileName)
        {
            try
            {
                _logger.LogInformation($"Compressing file {existingFile} started at {DateTime.Now}");

                if (!CheckIfFileNameContainsExtension(fileName)) fileName = $"{fileName}.zip";

                CompressFile(existingFile, fileName);

                _logger.LogInformation($"Compressing file {existingFile} finished at {DateTime.Now}");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ShowErrorInConsole();
                return 1;
            }
        }

        private void CompressFile(string existingFile, string fileName)
        {
            FileInfo fileToCompress = Helper.GetFileInfo(existingFile);

            using FileStream originalFileStream = fileToCompress.OpenRead();
            using FileStream compressedFileStream = File.Create($@"{Constant.BasePath}\{fileName}");
            using GZipStream gzip = new GZipStream(compressedFileStream, CompressionMode.Compress);

            int count;
            var buffer = new byte[BlockSize];
            while ((count = originalFileStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                gzip.Write(buffer, 0, buffer.Length);
            }
        }

        private void ShowErrorInConsole()
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("En error occurred while compressing the file!");
            Console.BackgroundColor = ConsoleColor.Black;
        }

        private bool CheckIfFileNameContainsExtension(string fileName)
        {
            Regex regex = new Regex("zip|gz|tar|rar");
            return regex.IsMatch(fileName);
        }
    }
}
