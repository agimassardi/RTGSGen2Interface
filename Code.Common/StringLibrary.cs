using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code.Common
{
    public static class StringLibrary
    {
        public static string AddLeadingZero(this object number,int stringLength)
        {
            string result = "";
            for (int i = 0; i < stringLength; i++)
            {
                result += "0";
            }

            if (number == null) number = 0;

            result = result + number.ToString();
            result = result.Substring(result.Length - stringLength);

            return result;
        }        
    }
}
