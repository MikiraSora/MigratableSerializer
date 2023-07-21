using System;
using System.IO;
using System.Threading.Tasks;

namespace MigratableSerializer
{
    public interface IFormatter
    {
        Type WritableType { get; }
        Task WriteAsync(Stream stream, object obj);
    }

    public interface IFormatter<T> : IFormatter
    {
        Task WriteAsync(Stream stream, T obj);
    }
}
