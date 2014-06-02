using System;
using System.IO;
using Microsoft.SqlServer.Server;

namespace DatabaseUtilities
{
    public partial class FileInfoUtilities
    {
        [Microsoft.SqlServer.Server.SqlFunction(Name = "FileExists_fn")]
        [Microsoft.SqlServer.Server.SqlProcedure(Name="FileExists_prc")]
        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        [Microsoft.SqlServer.Server.SqlFunction(Name = "DirectoryExists_fn")]
        [Microsoft.SqlServer.Server.SqlProcedure(Name="DirectoryExists_prc")]
        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        [Microsoft.SqlServer.Server.SqlFunction(Name = "path_GetRoot_fn")]
        public static string GetRoot(string path)
        {
            return Path.GetPathRoot(path);
        }

        [Microsoft.SqlServer.Server.SqlFunction(Name = "path_GetDirectory_fn")]
        public static string GetDirectory(string path)
        {
            return Path.GetDirectoryName(path);
        }

        [Microsoft.SqlServer.Server.SqlFunction(Name = "path_GetFileName_fn")]
        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        [Microsoft.SqlServer.Server.SqlFunction(Name = "path_GetExtension_fn")]
        public static string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }



    }
}
