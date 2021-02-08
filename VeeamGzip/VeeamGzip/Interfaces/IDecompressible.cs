namespace VeeamGzip.Interfaces
{
    public interface IDecompressible
    {
        int Execute(string existingFile, string fileName);
    }
}
