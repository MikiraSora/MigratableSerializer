using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MigratableSerializer
{
    public interface IMigration
    {
        Type FromType { get; }
        Type ToType { get; }

        bool CanDowngradable { get; }

        Task<object> UpgradeAsync(object fromObj);
        Task<object> DowngradeAsync(object toObj);
    }

    public interface IMigration<FromType, ToType> : IMigration
    {
        Task<ToType> UpgradeAsync(FromType fromObj);
        Task<FromType> DowngradeAsync(ToType toObj);
    }
}
