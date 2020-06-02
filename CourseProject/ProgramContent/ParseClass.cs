using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.ProgramContent
{
    class ParseClass
    {
        public static (bool, string, int) IPAddressParse(string str)
        {
            IPAddress tmp;
            var result = (answer: false, ip: "", port: 0);
            if (str.LastIndexOf(':') != -1)
            {
                result.ip = str.Substring(0, str.LastIndexOf(':'));
                if (Int32.TryParse(str.Substring(str.LastIndexOf(':') + 1), out result.port))
                {
                    if (IPAddress.TryParse(result.ip, out tmp))
                    {
                        result.answer = true;
                    }
                }
            }
            return result;
        }
    }
}
