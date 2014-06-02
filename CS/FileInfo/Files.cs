using System;
using System.Collections;
using System.IO;
using System.Data.SqlTypes;

namespace DatabaseUtilities
{
    public partial class FileInfoUtilities
    {
        [Microsoft.SqlServer.Server.SqlFunction(Name="files_fn", FillRowMethodName = "FillRowFiles", TableDefinition = "name nvarchar(4000), fullname nvarchar(4000), length bigint, createDate datetime, lastModifiedDate datetime")]
        public static IEnumerable Files(string path, string searchPattern, bool searchSubFolders)
        {
            try
            {
                SearchOption opt = searchSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                DirectoryInfo di = new DirectoryInfo(path.ToString());
                FileInfo[] f = di.GetFiles(searchPattern, opt);
                return f;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw;
            }
        }

        private static void FillRowFiles
            (
                Object obj,
                out SqlChars name,
                out SqlChars fullName,
                out SqlInt64 length,
                out SqlDateTime createDate,
                out SqlDateTime modifiedDate
            )
        {
            FileInfo fi = (FileInfo)obj;

            name = new SqlChars(fi.Name);
            fullName = new SqlChars(fi.FullName);
            length = new SqlInt64(fi.Length);
            createDate = new SqlDateTime(fi.CreationTime);
            modifiedDate = new SqlDateTime(fi.LastWriteTime);
        }
    }
}
