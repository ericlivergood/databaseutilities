using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace DatabaseUtilities
{
    public partial class StringUtilities
    {
        [Microsoft.SqlServer.Server.SqlFunction(IsDeterministic = true, Name="RegexMatch_fn")]
        public static bool RegexMatch_fn(string input, string pattern)
        {
            return Regex.IsMatch(input, pattern);
        }

        [Microsoft.SqlServer.Server.SqlFunction(IsDeterministic = true, Name="RegexReplace_fn")]
        public static string RegexReplace_fn(string input, string pattern, string replacement)
        {
            return Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase);
        }
        
        [Microsoft.SqlServer.Server.SqlFunction(IsDeterministic = true, Name = "RegexRemove_fn")]
        public static string RegexRemove_fn(string input, string pattern)
        {
            return Regex.Replace(input, pattern, String.Empty, RegexOptions.IgnoreCase);
        }

        [Microsoft.SqlServer.Server.SqlFunction(FillRowMethodName = "FillRowMatches", IsDeterministic = true, Name="RegexMatches_fn", TableDefinition="match nvarchar(max)")]
        public static IEnumerable RegexMatches_fn(string input, string pattern)
        {
            return Regex.Matches(input, pattern, RegexOptions.IgnoreCase);
        }

        public static void FillRowMatches(Object obj, out SqlChars match)
        {
            Match _match = (Match)obj;

            match = new SqlChars(_match.Value);
        }

        [Microsoft.SqlServer.Server.SqlFunction(FillRowMethodName = "FillRowSplit", IsDeterministic = true, Name="RegexSplit_fn", TableDefinition="splitString nvarchar(max)")]
        public static IEnumerable RegexSplit(string input, string pattern)
        {
            return Regex.Split(input, pattern, RegexOptions.IgnoreCase);
        }

        public static void FillRowSplit(Object obj, out SqlChars splitString)
        {
            splitString = new SqlChars((string)obj);
        }

        [Microsoft.SqlServer.Server.SqlFunction(FillRowMethodName = "FillSplitRowMatches", IsDeterministic = true, Name = "RegexSplitWord_fn", TableDefinition = "match nvarchar(max), stringIndex int, splitString nvarchar(max)")]
        public static IEnumerable RegexSplitWord(string input, string pattern)
        {
            return new SplitNodeIterator(input, pattern);
        }

        public static void FillSplitRowMatches(Object obj, out SqlChars match, out SqlInt32 index, out SqlChars splitString)
        {
            SplitNode _match = (SplitNode)obj;
            
            if (_match.Match == null)
            {
                splitString =  new SqlChars(_match.Value.Replace(" ", ""));
                match = new SqlChars("");
                index = new SqlInt32(0);
            }
            else
            {
                splitString = new SqlChars(_match.Value.Substring(_match.Match.Index + 1).Replace(" ", ""));
                match = new SqlChars(_match.Match.Value);
                index = new SqlInt32(_match.Match.Index);
            }
        }

        internal class SplitNode
        {
            private string _value;
            private Match _match;

            public string Value { get { return _value; } }

            public Match Match { get { return _match; } }

            public SplitNode(string value, Match match)
            {
                _value = value;
                _match = match;
            }
        }

        
        internal class SplitNodeIterator : IEnumerable
        {
            private Regex _regex;
            private String _input;

            public SplitNodeIterator(String input, String pattern)
            {
                _regex = new Regex(pattern, RegexOptions.IgnoreCase);
                _input = input;
            }

            public IEnumerator GetEnumerator()
            {
                Match current = null;
                do
                {
                    current = (current == null) ?
                        _regex.Match(_input) : current.NextMatch();
                    if (current.Success)
                    {
                        yield return new SplitNode(_input, current);
                    }
                    else
                    {
                        yield return new SplitNode(_input, null);
                    }
                }
                while (current.Success);
            }
        }
        
         

    }

}
