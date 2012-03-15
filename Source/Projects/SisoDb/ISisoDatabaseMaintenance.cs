namespace SisoDb
{
    public interface ISisoDatabaseMaintenance
    {
        /// <summary>
        /// Drops all structure sets.
        /// </summary>
        void Clear();

        /// <summary>
        /// Renames a structure.
        /// </summary>
        /// <param name="from">The old name, e.g Person</param>
        /// <param name="to">The new name, e.g People</param>
        void RenameStructure(string from, string to);
    }
}