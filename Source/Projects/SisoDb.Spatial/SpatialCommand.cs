using SisoDb.Dac;

namespace SisoDb.Spatial
{

    public struct SpatialCommand
    {
        public IDacParameter SidParam { get; set; }
        public string Sql { get; set; }

    }
}