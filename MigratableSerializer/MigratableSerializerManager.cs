using MigratableSerializer.Base.Exceptions;
using MigratableSerializer.Base.Graph;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MigratableSerializer
{
    public class MigratableSerializerManager
    {
        private List<IMigration> migrations = new();
        private List<IParser> parsers = new();
        private List<IFormatter> formatters = new();

        private DirectedGraph<Type> upgradeGraph = null;
        private DirectedGraph<Type> downgradeGraph = null;

        public void Clear()
        {
            migrations.Clear();
            parsers.Clear();
            formatters.Clear();

            upgradeGraph = downgradeGraph = default;
        }

        public void AddMigration(IMigration migration)
        {
            migrations.Add(migration);
            upgradeGraph = downgradeGraph = null;
        }

        public bool RemoveMigration(IMigration migration)
        {
            var r = migrations.Remove(migration);
            upgradeGraph = downgradeGraph = null;
            return r;
        }

        public void AddFormatter(IFormatter formatter)
        {
            formatters.Add(formatter);
        }

        public bool RemoveFormatter(IFormatter formatter)
        {
            return formatters.Remove(formatter);
        }

        public void AddParser(IParser parser)
        {
            parsers.Add(parser);
        }

        public bool RemoveParser(IParser parser)
        {
            return parsers.Remove(parser);
        }

        private async Task<object> TryParseAsync(byte[] buffer)
        {
            var parser = default(IParser);
            foreach (var p in parsers)
            {
                if (await p.CheckParsableAsync(buffer))
                {
                    parser = p;
                    break;
                }
            }

            if (parser is null)
                return default;

            return await parser.ParseAsync(buffer);
        }

        public async Task<object> Load(byte[] buffer, Type targetType)
        {
            var initObj = await TryParseAsync(buffer);
            if (initObj is null)
                throw new Exception($"无法生成{targetType.Name}对象: 无法找到buffer对应的Parser");

            var initType = initObj.GetType();

            //get upgrade migration chains.
            var chains = GetMigrationChain(initType, targetType, true);

            var curObj = initObj;
            foreach (var ch in chains)
            {
                var migratedObj = await ch.UpgradeAsync(curObj);
                curObj = migratedObj;
            }

            return curObj;
        }

        public async Task<T> Load<T>(byte[] buffer) => (T)(await Load(buffer, typeof(T)));

        private DirectedGraph<Type> GetUpgradeGraph()
        {
            if (upgradeGraph == null)
            {
                //rebuild
                upgradeGraph = new DirectedGraph<Type>((a, b) => a == b);
                foreach (var migration in migrations)
                    upgradeGraph.AddEdge(migration.FromType, migration.ToType);
            }

            return upgradeGraph;
        }

        private DirectedGraph<Type> GetDowngradeGraph()
        {
            if (downgradeGraph == null)
            {
                //rebuild
                downgradeGraph = new DirectedGraph<Type>((a, b) => a == b);
                foreach (var migration in migrations.Where(x => x.CanDowngradable))
                    downgradeGraph.AddEdge(migration.ToType, migration.FromType);
            }

            return downgradeGraph;
        }

        private IEnumerable<IMigration> GetMigrationChain(Type initType, Type targetType, bool isUpgrade)
        {
            var graph = isUpgrade ? GetUpgradeGraph() : GetDowngradeGraph();
            var path = graph.FindPath(initType, targetType);

            if (path == null)
                throw new MigrationException($"尝试迁移{initType.Name}到{targetType.Name}失败: 无法构造迁移实现链");

            //build chain
            for (int i = 0; i < path.Count - 1; i++)
            {
                var cur = path[i];
                var next = path[i + 1];

                var migration = isUpgrade ? migrations.FirstOrDefault(x => x.FromType == cur && x.ToType == next) : migrations.FirstOrDefault(x => x.ToType == cur && x.FromType == next);
                if (migration == null)
                    throw new MigrationException($"尝试迁移{initType.Name}到{targetType.Name}失败: 无法找到子链从{cur.Name}到{next.Name}的迁移实现.");
                yield return migration;
            }
        }

        public async Task Save(Stream stream, object fromObj, Type toType)
        {
            var fromType = fromObj.GetType();

            object curObj = fromObj;
            if (fromType != toType)
            {
                //downgrade
                var chains = GetMigrationChain(fromType, toType, false);
                foreach (var ch in chains)
                {
                    var migratedObj = await ch.DowngradeAsync(curObj);
                    curObj = migratedObj;
                }
            }

            var type = curObj.GetType();
            var parser = formatters.FirstOrDefault(x => x.WritableType == type);
            if (parser is null)
                throw new Exception($"无法找到类型 {type.Name} 对应的Formatter");
            await parser.WriteAsync(stream, curObj);
        }

        public Task Save<FromType, ToType>(Stream stream, FromType fromObj)
            => Save(stream, fromObj, typeof(ToType));

        public Task Save<ToType>(Stream stream, ToType fromObj)
            => Save<ToType, ToType>(stream, fromObj);
    }
}
