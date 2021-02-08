namespace VeeamGzip.Interfaces
{
    public interface ICompressible
    {
        int Execute(string existingFile, string fileName);
    }
}
