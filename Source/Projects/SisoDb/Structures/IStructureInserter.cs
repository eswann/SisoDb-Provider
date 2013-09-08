using System.Threading.Tasks;
using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public interface IStructureInserter
    {
        void Insert(IStructureSchema structureSchema, IStructure[] structures);
        Task InsertAsync(IStructureSchema structureSchema, IStructure[] structures);

        void InsertIndexesOnly(IStructureSchema structureSchema, IStructure[] structures);
        Task InsertIndexesOnlyAsync(IStructureSchema structureSchema, IStructure[] structures);

        void Replace(IStructureSchema structureSchema, IStructure structure);
        Task ReplaceAsync(IStructureSchema structureSchema, IStructure structure);
    }
}