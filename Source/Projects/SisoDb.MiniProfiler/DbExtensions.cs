﻿namespace SisoDb.MiniProfiler
{
    public static class DbExtensions
    {
        public static void ActivateProfiler(this ISisoDatabase db)
        {
            db.ProviderFactory.ConnectionManager.OnConnectionCreated =
                con => !(con is ProfiledConnectionWrapper)
                    ? new ProfiledConnectionWrapper(con, StackExchange.Profiling.MiniProfiler.Current)
                    : con;
        }

        public static void DeactivateProfiler(this ISisoDatabase db)
        {
            db.ProviderFactory.ConnectionManager.Reset();
        }
    }
}
