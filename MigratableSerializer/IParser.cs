using System.IO;
using System.Threading.Tasks;

namespace MigratableSerializer
{
    public interface IParser
    {
        Task<bool> CheckParsableAsync(byte[] buffer);
        Task<object> ParseAsync(byte[] buffer);
    }

    public interface IParser<T> : IParser
    {
        new Task<T> ParseAsync(byte[] buffer);
    }
}
