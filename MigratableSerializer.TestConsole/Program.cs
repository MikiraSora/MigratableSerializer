using MigratableSerializer.TestConsole.Impl.Migrations;
using MigratableSerializer.TestConsole.Impl.Models;
using MigratableSerializer.TestConsole.Impl.Parser;
using System.Text;
using System.Text.Json;

namespace MigratableSerializer.TestConsole
{
    internal class Program
    {
        private static MigratableSerializerManager manager;

        private static void Setup()
        {
            manager = new MigratableSerializerManager();

            manager.AddParser(new ConfigV1Serialization());
            manager.AddParser(new ConfigV2Serialization());
            manager.AddFormatter(new ConfigV1Serialization());
            manager.AddFormatter(new ConfigV2Serialization());
            manager.AddMigration(new MigrationConfigV1ToConfigV2());
        }

        private static async Task<byte[]> GetFileBuffer()
        {
            using var ms = new MemoryStream();
            await manager.Save(ms, new ConfigV1() { Description = "123", Name = "SBPPY" });
            ms.Position = 0;

            return ms.ToArray();
        }

        static async Task Main(string[] args)
        {
            Setup();

            var buffer = await GetFileBuffer();

            var config = await manager.Load<ConfigV2>(buffer);

            config.NewDescription = "456";

            var ms = new MemoryStream();
            await manager.Save<ConfigV2, ConfigV1>(ms, config);
            var str = Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}