using System;
using System.Globalization;
using Machine.Specifications;
using SisoDb.NCore;

namespace SisoDb.Testing
{
    public abstract class SpecificationBase
    {
        protected static ITestContext TestContext;

        protected static Exception CaughtException;

        protected static string GetUtcOffset()
        {
            var utcOffSet = DateTimeOffset.Now.Hour;
            string stringUtcOffset = string.Empty;
            if (utcOffSet >= 0)
            {
                stringUtcOffset = (utcOffSet >= 0) ? "+" : "-" + utcOffSet.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
            }

            return stringUtcOffset;
        }

        protected SpecificationBase()
        {
            SysDateTime.NowFn = () => TestConstants.FixedDateTime;   
        }

        Cleanup after = () => 
        {
            SysDateTime.NowFn = () => TestConstants.FixedDateTime;  

            if(TestContext != null)
                TestContext.Cleanup();

            CaughtException = null;
        };
    }
}