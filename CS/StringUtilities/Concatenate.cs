using System;
using System.Text;
using System.Data.SqlTypes;


namespace DatabaseUtilities
{
    [Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedAggregate(
        Microsoft.SqlServer.Server.Format.UserDefined,
        IsInvariantToDuplicates = false,
        IsInvariantToNulls = false,
        IsInvariantToOrder = false,
        MaxByteSize = -1,
        Name = "Concatenate"
    )]
    public struct Concatenate : Microsoft.SqlServer.Server.IBinarySerialize
    {
        private StringBuilder sb;

        public void Init()
        {
            sb = new StringBuilder();
        }

        public void Accumulate(string s)
        {
            sb.Append(s);
        }

        public void Merge(Concatenate toMerge)
        {
            sb.Append(toMerge.sb);
        }

        public SqlChars Terminate()
        {
            return new SqlChars(sb.ToString().ToCharArray());
        }


        public void Read(System.IO.BinaryReader r)
        {
            sb = new StringBuilder(r.ReadString());
        }

        public void Write(System.IO.BinaryWriter w)
        {
            w.Write(sb.ToString());
        }
    }

    [Serializable]
    [Microsoft.SqlServer.Server.SqlUserDefinedAggregate(
        Microsoft.SqlServer.Server.Format.UserDefined,
        IsInvariantToDuplicates = false,
        IsInvariantToNulls = false,
        IsInvariantToOrder = false,
        MaxByteSize = -1,
        Name = "ToDelimitedList"
    )]
    public struct ToDelimitedList : Microsoft.SqlServer.Server.IBinarySerialize
    {
        private StringBuilder sb;
        private string delimiter;
        private bool isDelimiterSet;

        public void Init()
        {
            sb = new StringBuilder();
            delimiter = "";
            isDelimiterSet = false;
        }

        public void Accumulate(string Str, string Delimiter)
        {
            if (!isDelimiterSet)
            {
                delimiter = Delimiter;
                isDelimiterSet = true;
            }

            if (sb.Length > 0)
            {
                sb.Append(delimiter);
            }
            sb.Append(Str);
        }

        public void Merge(ToDelimitedList toMerge)
        {
            sb.Append(delimiter);
            sb.Append(toMerge.sb);
        }

        public SqlChars Terminate()
        {
            return new SqlChars(sb.ToString().ToCharArray());
        }


        public void Read(System.IO.BinaryReader r)
        {
            isDelimiterSet = r.ReadBoolean();
            delimiter = r.ReadString();
            sb = new StringBuilder(r.ReadString());
        }

        public void Write(System.IO.BinaryWriter w)
        {
            w.Write(isDelimiterSet);
            w.Write(delimiter);
            w.Write(sb.ToString());
        }
    }    
    
}
