using System;

namespace CoreORM;

public static class ORMFunctions
{
    public static bool IsNullableType(string type)
    {
        if (type == "string")
        {
            return false;
        }
        return true;
    }

    public static string ColumnNameToLabel(string colname)
    {
        string rv = "";
        foreach (char c in colname)
        {
            if (Char.IsUpper(c))
            {
                rv += " " + c;
            }
            else
            {
                rv += c;
            }
        }
        rv = rv.Trim();
        return rv;
    }
}
