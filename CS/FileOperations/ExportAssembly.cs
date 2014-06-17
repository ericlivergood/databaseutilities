using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;


namespace DatabaseUtilities
{
    public partial class FileOperationUtilities
    {

        [Microsoft.SqlServer.Server.SqlProcedure(Name="ExportAssembly_prc")]
        public static void ExportAssembly(string assemblyName, string path, string databaseName)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                if (databaseName != null)
                {
                    conn.ChangeDatabase(databaseName);
                }

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                select top 1 content 
                from sys.assembly_files f
                    inner join sys.assemblies a
                        on f.assembly_id = a.assembly_id
                where file_id = 1
                and a.name = @name";

                    cmd.Parameters.Add("@name", SqlDbType.VarChar);
                    cmd.Parameters[0].Value = assemblyName;

                    SqlDataReader r = cmd.ExecuteReader();
                    r.Read();
                    SqlBytes b = r.GetSqlBytes(0);

                    FileStream fs = new FileStream(path, FileMode.CreateNew);
                    fs.Write(b.Value, 0, (int)b.Length);
                    fs.Close();
                }
            }
        }
    }

}