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
    internal class ConfigV2Serialization : SerializerBase<ConfigV2>
    {
        private const string Head = "configv2";

        public override async Task<bool> CheckParsableAsync(byte[] buffer)
        {
            using var reader = new StreamReader(new MemoryStream(buffer));
            var line = await reader.ReadLineAsync();
            return line.Trim().ToLower() == Head;
        }

        public async override Task<ConfigV2> ParseAsync(byte[] buffer)
        {
            using var reader = new StreamReader(new MemoryStream(buffer));
            //skip header
            await reader.ReadLineAsync();

            var jsonContent = await reader.ReadToEndAsync();
            var obj = JsonSerializer.Deserialize<ConfigV2>(jsonContent);
            return obj;
        }

        public override async Task WriteAsync(Stream stream, ConfigV2 obj)
        {
            var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(Head);
            await writer.FlushAsync();
            await JsonSerializer.SerializeAsync(stream, obj);
        }
    }
}
