using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class BufferHelper
    {
        public static byte[] CombineBuffer(byte[] b1, int firstIndex, int firstLength, byte[] b2, int secondIndex, int secondLength)
        {
            byte[] res = null;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(b1, firstIndex, firstLength);
                bw.Write(b2, secondIndex, secondLength);
                res = ms.ToArray();
                bw.Dispose();
            }
            return res;
        }
    }
}
