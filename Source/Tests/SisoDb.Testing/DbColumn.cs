﻿using System;

namespace SisoDb.Testing
{
    [Serializable]
    public class DbColumn
    {
        public string Name { get; private set; }

        public string DbDataType { get; private set; }

        public DbColumn(string name, string dbDataType)
        {
            Name = name;
            DbDataType = dbDataType;
        }
    }
}