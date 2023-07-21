using MigratableSerializer.TestConsole.Impl.Models;
using MigratableSerializer.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MigratableSerializer.TestConsole.Impl.Parser
{
    internal class ConfigV1Serialization : SerializerBase<ConfigV1>
    {
        private const string Head = "configv1";

        public override async Task<bool> CheckParsableAsync(byte[] buffer)
        {
            using var reader = new StreamReader(new MemoryStream(buffer));
            var line = await reader.ReadLineAsync();
            return line.Trim().ToLower() == Head;
        }

        public async override Task<ConfigV1> ParseAsync(byte[] buffer)
        {
            using var reader = new StreamReader(new MemoryStream(buffer));
            //skip header
            await reader.ReadLineAsync();

            var jsonContent = await reader.ReadToEndAsync();
            var obj = JsonSerializer.Deserialize<ConfigV1>(jsonContent);
            return obj;
        }

        public override async Task WriteAsync(Stream stream, ConfigV1 obj)
        {
            var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(Head);
            await writer.FlushAsync();
            await JsonSerializer.SerializeAsync(stream, obj);
        }
    }
}
