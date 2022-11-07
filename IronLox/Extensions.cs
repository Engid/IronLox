using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronLox;

public static class Extensions
{
    public static string SubStr(this string source, int startIdx, int endIdx)
    {
        if (startIdx > source.Length || endIdx > source.Length) throw new ArgumentException("attempted to index outside of string lenght");

        return source.Substring(startIdx, endIdx - startIdx);
    }
}
