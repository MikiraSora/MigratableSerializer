using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MigratableSerializer.Wrapper
{
    public abstract class MigrationBase<From, To> : IMigration<From, To>
    {
        public Type FromType { get; } = typeof(From);
        public Type ToType { get; } = typeof(To);

        public abstract bool CanDowngradable { get; }

        public async Task<object> UpgradeAsync(object fromObj) => await UpgradeAsync((From)fromObj);
        public async Task<object> DowngradeAsync(object toObj) => await DowngradeAsync((To)toObj);

        public abstract Task<From> DowngradeAsync(To toObj);
        public abstract Task<To> UpgradeAsync(From fromObj);
    }
}
