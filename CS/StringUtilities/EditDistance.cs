using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseUtilities
{
    public partial class StringUtilities
    {
        [Microsoft.SqlServer.Server.SqlFunction(Name="EditDistance_fn")]
        public static int EditDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            for (int i = 0; i <= n; d[i, 0] = i++)
            {
                ;
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
                ;
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    //Levenshtein Distance
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1,               //deletion
                                    d[i, j - 1] + 1),           //inserstion
                                    d[i - 1, j - 1] + cost);    //substitution

                    //Damerau Distance
                    if ((i > 1) && (j > 1) && (s[i - 1] == t[j - 2]) && (s[i - 2] == t[j - 1]))
                    {
                        d[i, j] = Math.Min(
                                        d[i, j],                    //Levenshtei                                                                             n cost
                                        d[i - 2, j - 2] + cost);    //transposit                                                                             ion
                    }
                }
            }
            return d[n, m];
        }
    }
}
