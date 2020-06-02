using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.ProgramContent
{
    class CollectionConversion
    {
        public static byte[] AddToEndArray(byte[] first, byte[] second)
        {
            Array.Resize(ref first, first.Length + second.Length);
            for (int i = 0; i < second.Length; i++)
            {
                first[first.Length - second.Length + i] = second[i];
            }
            return first;
        }

        public static (byte[], byte, byte[]) GetSenderInformation(byte[] source)
        {
            var result = (userID: new byte[4], operation: (byte)0, data: new byte[source.Length - 5]);
            result.operation = source[source.Length - 5];
            Array.Copy(source, source.Length - 4, result.userID, 0, 4);
            Array.Copy(source, 0, result.data, 0, source.Length - 5);
            return result;
        }

        public static List<T> ConcatList<T>(List<T> first, List<T> second)
        {
            List<T> newList = new List<T>();
            newList.AddRange(first.ToArray());
            newList.AddRange(second.ToArray());
            return newList;
        }
    }
}
