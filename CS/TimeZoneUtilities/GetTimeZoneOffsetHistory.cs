using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace DatabaseUtilities
{
    public partial class TimeZoneUtilities
    {

        [Microsoft.SqlServer.Server.SqlFunction(Name="GetTimeZoneOffsetHistory_fn", FillRowMethodName = "FillTimeZoneOffsetRow", TableDefinition = "fromTimeZoneId nvarchar(4000), toTimeZoneId nvarchar(4000), startDate datetime, endDate datetime, offsetInMinutes smallint")]
        public static IEnumerable<TimeZoneOffset> GetTimeZoneOffsetHistory(DateTime startDate, DateTime endDate, string fromTimeZoneID, string toTimeZoneID)
        {
            List<TimeZoneInfo> convertFrom = new List<TimeZoneInfo>();
            List<TimeZoneInfo> convertTo = new List<TimeZoneInfo>();

            if (fromTimeZoneID == "" || fromTimeZoneID == null)
            {
                convertFrom.AddRange(TimeZoneInfo.GetSystemTimeZones());
            }
            else
            {
                convertFrom.Add(TimeZoneInfo.FindSystemTimeZoneById(fromTimeZoneID));
            }

            if (toTimeZoneID == "" || toTimeZoneID == null)
            {
                convertTo.AddRange(TimeZoneInfo.GetSystemTimeZones());
            }
            else
            {
                convertTo.Add(TimeZoneInfo.FindSystemTimeZoneById(toTimeZoneID));
            }
            int i = 0;
            foreach (TimeZoneInfo from in convertFrom)
            {
                foreach (TimeZoneInfo to in convertTo)
                {
                    foreach (TimeZoneOffset o in GetOffsetHistory(from, to, startDate, endDate))
                    {
                        yield return o;
                    }
                    i++;
                }
            }
        }

        private static IEnumerable<TimeZoneOffset> GetOffsetHistory(TimeZoneInfo from, TimeZoneInfo to, DateTime startDate, DateTime endDate)
        {
            DateTime current = startDate;
            DateTime lastChange = startDate;
            short lastOffset = GetOffsetInMinutes(from, to, current);

            int minutesToSkip = 60 * 24 * 30; //skip 30 days.

            while (current <= endDate)
            {
                current = current.AddMinutes(minutesToSkip);
                short offset = GetOffsetInMinutes(from, to, current);

                if (lastOffset != offset)
                {
                    DateTime nextChange = GetExactOffsetEndDate(from, to, current.AddMinutes(-1 * minutesToSkip), current, minutesToSkip);
                    yield return new TimeZoneOffset(from.Id, to.Id, lastChange, nextChange, lastOffset);
                    lastOffset = offset;
                    lastChange = nextChange;
                }

            }
            yield return new TimeZoneOffset(from.Id, to.Id, lastChange, endDate, lastOffset);
        }

        private static DateTime GetExactOffsetEndDate(TimeZoneInfo from, TimeZoneInfo to, DateTime startDate, DateTime endDate, int minutesToSkip)
        {
            DateTime current = startDate;
            short lastOffset = GetOffsetInMinutes(from, to, current);
            short offset = lastOffset;
            int m = minutesToSkip / 5;
            if (m < 1)
            {
                m = 1;
            }
            while (current <= endDate)
            {
                offset = GetOffsetInMinutes(from, to, current.AddMinutes(m));

                if (lastOffset != offset)
                {
                    if (m == 1)
                    {
                        return current.AddMinutes(m);
                    }
                    else
                    {
                        return GetExactOffsetEndDate(from, to, current, current.AddMinutes(m), m);
                    }
                }

                current = current.AddMinutes(m);
            }

            if (m == 1)
            {
                throw new Exception("The time zone offset did not change in this period");
            }
            else
            {
                return GetExactOffsetEndDate(from, to, startDate, endDate, m);
            }
        }

        private static short GetOffsetInMinutes(TimeZoneInfo from, TimeZoneInfo to, DateTime date)
        {
            return (short)(from.GetUtcOffset(date).TotalMinutes - to.GetUtcOffset(date).TotalMinutes);
        }


        private static void FillTimeZoneOffsetRow
            (
                Object o
            ,   out SqlChars fromTimeZoneId
            ,   out SqlChars toTimeZoneId
            ,   out SqlDateTime startDate
            ,   out SqlDateTime endDate
            ,   out SqlInt16 offsetInMinutes
            )
        {
            TimeZoneOffset tzo = (TimeZoneOffset)o;

            fromTimeZoneId = new SqlChars(tzo.FromTimeZoneId);
            toTimeZoneId = new SqlChars(tzo.ToTimeZoneId);
            startDate = new SqlDateTime(tzo.StartDate);
            endDate = new SqlDateTime(tzo.EndDate);
            offsetInMinutes = new SqlInt16(tzo.OffsetInMinutes);
        }


    }
}
