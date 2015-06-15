using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericIndexing.Common.Utils
{
    public static class StringHelper
    {
        public static string StripCdata(string input)
        {
            if (input == null)
                return String.Empty;

            String output = input;

            if (input.IndexOf("<![CDATA[") == 0)
            {
                output = input.Substring(9, input.Length - 9);
                if (output.IndexOf("]]>") == output.Length - 3)
                    output = output.Substring(0, output.Length - 3);
            }

            return output;
        }
    }
}
