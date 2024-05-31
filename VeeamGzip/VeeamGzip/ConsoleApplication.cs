using Microsoft.Extensions.Logging;
using System;
using System.IO;
using VeeamGzip.Constants;
using VeeamGzip.Interfaces;

namespace VeeamGzip
{
    public class ConsoleApplication : IConsoleApplication
    {
        private readonly ICompressible _compress;
        private readonly IDecompressible _decompress;
        private readonly ILogger<ConsoleApplication> _logger;

        public ConsoleApplication(ICompressible compress, IDecompressible decompress, ILogger<ConsoleApplication> logger)
        {
            _compress = compress;
            _decompress = decompress;
            _logger = logger;
        }

        public void Run()
        {
            _logger.LogInformation($"Application started at {DateTime.Now}");

            ShowInstructions();

            var input = Console.ReadLine();
            var existingFile = string.Empty;
            while (true)
            {
                var commandArgs = input.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (!ValidateInput(ref input, commandArgs, ref existingFile)) continue;

                if (commandArgs.Length > 2)
                {
                    var fileName = commandArgs[2];
                    ExecuteCommand(existingFile, fileName, commandArgs[0]);
                }

                ShowInstructions();
                input = Console.ReadLine();
            }
        }

        private void ExecuteCommand(string existingFile, string fileName, string command)
        {
            int result;
            if (command == Constant.Compress)
            {
                result = _compress.Execute(existingFile, fileName);
                Environment.Exit(result);
            }
            else
            {
                result = _decompress.Execute(existingFile, fileName);
                Environment.Exit(result);
            }
        }

        private bool ValidateInput(ref string input, string[] commandArgs, ref string existingFile)
        {
            if (!commandArgs[0].Equals(Constant.Compress) && !commandArgs[0].Equals(Constant.Decompress))
            {
                Console.WriteLine("The command should be 'compress' or 'decompress'");
                input = Console.ReadLine();
                return false;
            }

            if (commandArgs.Length > 1)
            {
                existingFile = commandArgs[1];
                if (!CheckIfFileExists(existingFile, ref input)) 
                    return false;
            }

            return true;
        }

        private bool CheckIfFileExists(string existingFile, ref string input)
        {
            if (!File.Exists($@"{Constant.BasePath}\{existingFile}"))
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine($"The file name provided does not exist in '{Constant.BasePath}' folder or extension is missing");
                Console.WriteLine();
                Console.BackgroundColor = ConsoleColor.Black;
                ShowInstructions();
                input = Console.ReadLine();
                return false;
            }

            return true;
        }

        private static void ShowInstructions()
        {
            Console.WriteLine("Please enter command for compressing or decompresing the file in the following format:");
            Console.WriteLine("compress [original file name] [archive file name] or");
            Console.WriteLine("decompress [archive file name] [decompressing file name]");
        }
    }
}
