using System.IO;
using VeeamGzip.Constants;

namespace VeeamGzip.Helpers
{
    public static class Helper
    {
        public static FileInfo GetFileInfo(string existingFile)
        {
            return new FileInfo($@"{Constant.BasePath}\{existingFile}");
        }
    }
}
