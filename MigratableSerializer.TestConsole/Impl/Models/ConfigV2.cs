using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigratableSerializer.TestConsole.Impl.Models
{
    internal class ConfigV2 : ConfigV1
    {
        public string NewDescription { get; set; }
    }
}
