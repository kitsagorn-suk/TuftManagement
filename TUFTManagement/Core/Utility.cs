using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.Core
{
    public class Utility
    {
        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }
    }
}