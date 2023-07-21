using MigratableSerializer.TestConsole.Impl.Models;
using MigratableSerializer.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MigratableSerializer.TestConsole.Impl.Migrations
{
    internal class MigrationConfigV1ToConfigV2 : MigrationBase<ConfigV1, ConfigV2>
    {
        public override bool CanDowngradable => true;
       
        public override async Task<ConfigV2> UpgradeAsync(ConfigV1 fromObj)
        {
            //simple copy
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, fromObj);
            stream.Position = 0;
            var v2Obj = await JsonSerializer.DeserializeAsync<ConfigV2>(stream);

            v2Obj.NewDescription = v2Obj.Description + " NEW!!!";

            return v2Obj;
        }

        public override Task<ConfigV1> DowngradeAsync(ConfigV2 toObj)
        {
            var v1Obj = new ConfigV1();
            v1Obj.Description = toObj.Description;
            v1Obj.Name = toObj.Name;

            return Task.FromResult(v1Obj);
        }
    }
}
