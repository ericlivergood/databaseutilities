using System;
using System.Collections;
using System.IO;
using System.Data.SqlTypes;

namespace DatabaseUtilities
{
    public partial class FileInfoUtilities
    {
        [Microsoft.SqlServer.Server.SqlFunction(FillRowMethodName = "FillRowDirectories", TableDefinition = "name nvarchar(4000), fullname nvarchar(4000), createDate datetime, modifiedDate datetime")]
        public static IEnumerable Directories(string path, string searchPattern, bool searchSubFolders)
        {
            try
            {
                SearchOption opt = searchSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                DirectoryInfo di = new DirectoryInfo(path.ToString());
                DirectoryInfo[] f = di.GetDirectories(searchPattern, opt);
                return f;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw;
            }
        }

        private static void FillRowDirectories
            (
                Object obj,
                out SqlChars name,
                out SqlChars fullName,
                out SqlDateTime createDate,
                out SqlDateTime modifiedDate
            )
        {
            DirectoryInfo di = (DirectoryInfo)obj;

            name = new SqlChars(di.Name);
            fullName = new SqlChars(di.FullName);
            createDate = new SqlDateTime(di.CreationTime);
            modifiedDate = new SqlDateTime(di.LastWriteTime);
        }
    }
}
