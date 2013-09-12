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
            var utcOffSet = TimeZone.CurrentTimeZone.GetUtcOffset(new DateTime(2000, 01, 01)).Hours;
            string stringUtcOffset = (utcOffSet >= 0) ? "+" : "-" + Math.Abs(utcOffSet).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');

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