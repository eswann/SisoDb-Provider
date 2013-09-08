namespace SisoDb.Dac
{
    public struct DacCommand
    {

        public string Sql { get; set; }

        public IDacParameter[] Parameters { get; set; }
    }
}