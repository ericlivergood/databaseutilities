using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace DatabaseUtilities
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WriteTimeZonesToTable(args[0], args[1], "dbo");
        }

        private static void WriteTimeZonesToTable(string serverName, string databaseName, string schemaName)
        {
            Dictionary<string, int> timeZoneIds = new Dictionary<string, int>();
            SqlConnectionStringBuilder scb = new SqlConnectionStringBuilder();
            scb.DataSource = serverName;
            scb.InitialCatalog = databaseName;
            scb.IntegratedSecurity = true;

            using (SqlConnection conn = new SqlConnection(scb.ConnectionString))
            {
                conn.Open();

                SqlBulkCopy bc = new SqlBulkCopy(conn);
                List<DataRow> rows = new List<DataRow>();

                DataTable t = new DataTable();
                t.Columns.Add("timeZoneId", typeof(SqlInt16));
                t.Columns.Add("windowsTimeZoneId", typeof(SqlString));
                t.Columns.Add("displayName", typeof(SqlString));
                t.Columns.Add("baseUtcOffsetInHours", typeof(SqlDouble));
                t.Columns.Add("standardName", typeof(SqlString));
                t.Columns.Add("daylightName", typeof(SqlString));


                int i = 1;

                foreach (TimeZoneInfo tz in TimeZoneUtilities.GetTimeZones())
                {
                    timeZoneIds.Add(tz.Id, i);
                    t.Rows.Add(new object[] 
                        {
                            (short)i, 
                            tz.Id,
                            tz.DisplayName, 
                            tz.BaseUtcOffset.TotalHours,
                            tz.StandardName,
                            tz.DaylightName
                        }
                    );
                    i++;
                }

                bc.DestinationTableName = schemaName + "." + "TimeZone_tbl";
                TruncateTable(conn, bc.DestinationTableName);
                bc.WriteToServer(t);

                bc.Close();
                conn.Close();
            }
             
            scb = new SqlConnectionStringBuilder();
            scb.DataSource = serverName;
            scb.InitialCatalog = databaseName;
            scb.IntegratedSecurity = true;

            using (SqlConnection conn = new SqlConnection(scb.ConnectionString))
            {
                conn.Open();

                SqlBulkCopy bc = new SqlBulkCopy(conn);
                List<DataRow> rows = new List<DataRow>();

                DataTable t = new DataTable();
                t.Columns.Add("fromTimeZoneId", typeof(SqlInt16));
                t.Columns.Add("toTimeZoneId", typeof(SqlInt16));
                t.Columns.Add("startDate", typeof(SqlDateTime));
                t.Columns.Add("endDate", typeof(SqlDateTime));
                t.Columns.Add("offsetInMinutes", typeof(SqlInt16));


                foreach (TimeZoneOffset tz in TimeZoneUtilities.GetTimeZoneOffsetHistory(DateTime.Parse("1/1/1900"), DateTime.Parse("6/5/2079"), null, null))
                {
                    t.Rows.Add(new object[] 
                        {
                            (short)timeZoneIds[tz.FromTimeZoneId],
                            (short)timeZoneIds[tz.ToTimeZoneId],
                            tz.StartDate,
                            tz.EndDate,
                            tz.OffsetInMinutes
                        }
                    );
                }

                bc.DestinationTableName = schemaName + ".TimeZoneToTimeZoneOffset_tbl";
                TruncateTable(conn, bc.DestinationTableName);
                bc.BulkCopyTimeout = 900;
                bc.WriteToServer(t);

                bc.Close();
                conn.Close();
            }
        }

        private static void TruncateTable(SqlConnection c, string schemaQualifiedTableName)
        {
            if (c.State != ConnectionState.Open)
            {
                c.Open();
            }
            SqlCommand cmd = new SqlCommand("truncate table " + schemaQualifiedTableName, c);
            cmd.ExecuteNonQuery();

        }

        private static void WriteTimeZonesToCSV(string directory)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(directory, "TimeZones.csv"), false))
            {
                foreach(TimeZoneInfo tz in TimeZoneUtilities.GetTimeZones())
                {
                    List<object> l = new List<object>();
                    l.Add(tz.DisplayName);
                    l.Add(tz.Id);
                    l.Add(tz.BaseUtcOffset.TotalHours);
                    l.Add(tz.StandardName);
                    l.Add(tz.DaylightName);

                    sw.WriteLine(ListToCommaSeparatedString(l));
                }
                sw.Close();
            }
        }

        private static void WriteTimeZoneHistoryToCSV(string directory)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(directory, "TimeZoneOffsetHistory.csv"), false))
            {
                foreach (TimeZoneOffset tz in TimeZoneUtilities.GetTimeZoneOffsetHistory(DateTime.Parse("1/1/1900"), DateTime.Parse("12/31/2079"), null, null))
                {
                    List<object> l = new List<object>();
                    l.Add(tz.FromTimeZoneId);
                    l.Add(tz.ToTimeZoneId);
                    l.Add(tz.StartDate.ToString("yyyy-MM-dd hh:mm:ss"));
                    l.Add(tz.EndDate.ToString("yyyy-MM-dd hh:mm:ss"));
                    l.Add(tz.OffsetInMinutes);

                    sw.WriteLine(ListToCommaSeparatedString(l));
                }
                sw.Close();
            }            
        }

        private static string ListToCommaSeparatedString(List<object> l)
        {
            StringBuilder sb = new StringBuilder();
            bool firstObject = true;
            foreach (object o in l)
            {
                if (!firstObject)
                {
                    sb.Append(",");
                }
                else
                {
                    firstObject = false;
                }
                sb.Append(o.ToString().Replace(",", ""));
            }

            return sb.ToString();
        }
    }
}
