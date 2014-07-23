using System;
using System.Collections.Generic;
using System.Data.SqlTypes;


namespace DatabaseUtilities
{
    public partial class TimeZoneUtilities 
    {

        [Microsoft.SqlServer.Server.SqlFunction(
        Name="GetTimeZones_fn",   
        FillRowMethodName = "FillTimeZoneRow", 
        TableDefinition = "timeZoneId nvarchar(4000), timeZoneName nvarchar(4000), currentUTCOffset decimal, baseUTCOffset decimal, isDST bit, standardTimeZoneName nvarchar(4000), dstTimeZoneName nvarchar(4000)")]
        public static IEnumerable<TimeZoneInfo> GetTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones();
        }

        private static void FillTimeZoneRow
        (
            Object o
        ,   out SqlChars timeZoneId
        ,   out SqlChars timeZoneName
        ,   out SqlDecimal currentUTCOffset
        ,   out SqlDecimal baseUTCOffset
        ,   out SqlBoolean isDST
        ,   out SqlChars standardTimeZoneName
        ,   out SqlChars dstTimeZoneName

        )
        {
            TimeZoneInfo tz = (TimeZoneInfo)o;

            timeZoneName = new SqlChars(tz.DisplayName);
            timeZoneId = new SqlChars(tz.Id);
            baseUTCOffset = new SqlDecimal(tz.BaseUtcOffset.TotalHours);
            standardTimeZoneName = new SqlChars(tz.StandardName);
            dstTimeZoneName = new SqlChars(tz.DaylightName);
            isDST = new SqlBoolean(tz.IsDaylightSavingTime(DateTime.Now));
            currentUTCOffset = new SqlDecimal(tz.GetUtcOffset(DateTime.Now).TotalHours);
           
        }
    }
}
