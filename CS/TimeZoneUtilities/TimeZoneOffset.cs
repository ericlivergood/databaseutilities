using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtilities
{
    public class TimeZoneOffset
    {
        public string FromTimeZoneId;
        public string ToTimeZoneId;

        public DateTime StartDate;
        public DateTime EndDate;
        public short OffsetInMinutes;

        public TimeZoneOffset(string fromTimeZoneId, string toTimeZoneId, DateTime startDate, DateTime endDate, short offsetInMinutes)
        {
            FromTimeZoneId = fromTimeZoneId;
            ToTimeZoneId = toTimeZoneId;
            StartDate = startDate;
            EndDate = endDate;
            OffsetInMinutes = offsetInMinutes;
        }
    }
}
