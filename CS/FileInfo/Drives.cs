using System;
using System.IO;
using System.Collections;
using System.Data.SqlTypes;

namespace DatabaseUtilities
{
    public partial class FileInfoUtilities
    {
        [Microsoft.SqlServer.Server.SqlFunction(FillRowMethodName = "FillRowDrives", TableDefinition = "driveLetter nvarchar(4000), driveLabel nvarchar(4000), driveType nvarchar(4000), totalSpaceMB bigint, freeSpaceMB bigint")]
        public static IEnumerable Drives()
        {
            try
            {
                return DriveInfo.GetDrives();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw;
            }
        }

        private static void FillRowDrives
            (
                Object obj,
                out SqlChars driveLetter,
                out SqlChars driveLabel,
                out SqlChars driveType,
                out SqlInt64 totalSpaceMB,
                out SqlInt64 freeSpaceMB
            )
        {
            DriveInfo di = (DriveInfo)obj;

            driveLetter = new SqlChars(di.Name);
            driveLabel = new SqlChars(di.VolumeLabel);
            driveType = new SqlChars(di.DriveType.ToString());
            totalSpaceMB = new SqlInt64(di.TotalSize) / 1024 / 1024;
            freeSpaceMB = new SqlInt64(di.AvailableFreeSpace) / 1024 / 1024;
        }
    }
}
