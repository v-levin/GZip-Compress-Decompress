using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
using VeeamGzip.Constants;
using VeeamGzip.Helpers;
using VeeamGzip.Interfaces;

namespace VeeamGzip.Services
{
    public class Decompress : IDecompressible
    {
        private readonly ILogger<Decompress> _logger;

        public Decompress(ILogger<Decompress> logger)
        {
            _logger = logger;
        }
        public int Execute(string existingFile, string fileName)
        {
            try
            {
                _logger.LogInformation($"Decompressing file {existingFile} started at {DateTime.Now}");

                DecompressFile(existingFile, fileName);

                _logger.LogInformation($"Decompressing file {existingFile} finished at {DateTime.Now}");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ShowErrorInConsole();
                return 1;
            }
        }

        private void DecompressFile(string existingFile, string fileName)
        {
            FileInfo fileToDecompress = Helper.GetFileInfo(existingFile);

            using FileStream inputStream = fileToDecompress.OpenRead();
            using FileStream outputStream = File.Create($@"{Constant.BasePath}\{fileName}");
            using GZipStream gzip = new GZipStream(inputStream, CompressionMode.Decompress);
            gzip.CopyTo(outputStream);
        }

        private void ShowErrorInConsole()
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("En error occurred while decompressing the file!");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
