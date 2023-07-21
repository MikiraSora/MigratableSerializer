using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MigratableSerializer.Wrapper
{
    public abstract class SerializerBase<T> : IFormatter<T>, IParser<T>
    {
        public Type WritableType { get; } = typeof(T);

        async Task<object> IParser.ParseAsync(byte[] buffer) => await ParseAsync(buffer);
        async Task<T> IParser<T>.ParseAsync(byte[] buffer) => await ParseAsync(buffer);
        public async Task WriteAsync(Stream stream, object obj) => await WriteAsync(stream, (T)obj);


        public abstract Task<bool> CheckParsableAsync(byte[] buffer);
        public abstract Task<T> ParseAsync(byte[] buffer);
        public abstract Task WriteAsync(Stream stream, T obj);
    }
}
