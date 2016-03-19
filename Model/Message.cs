using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Message
    {
        public byte Flag1 { get; set; }
        public byte Flag2 { get; set; }
        public int MsgLength { get; set; }
        public byte[] BufferData { get; set; }
        public byte[] BufferDuoYu { get; set; }

        /// <summary>
        /// 讲一个message对象变为byte数组
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public byte[] TransferToBuffer()
        {
            byte[] bs = null;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(this.Flag1);//写入标志1
                bw.Write(this.Flag2);//写入标志2
                bw.Write(MsgLength);//写入流的长度
                if (BufferData != null && BufferData.Length > 0)
                {
                    bw.Write(BufferData);//写入流的内容
                }
                bs = ms.ToArray();
                ms.Dispose();
                bw.Dispose();
            }
            return bs;
        }

        public static Message TransferToMessage(byte[] bs)
        {
            var length = bs.LongLength;
            Message msg = new Message();
            using (MemoryStream ms = new MemoryStream(bs))
            {
                BinaryReader br = new BinaryReader(ms);
                msg.Flag1 = br.ReadByte();
                msg.Flag2 = br.ReadByte();
                msg.MsgLength = br.ReadInt32();
                if (msg.MsgLength > 0)
                {
                    msg.BufferData = br.ReadBytes(msg.MsgLength);
                    //判断是否存在多余的数据
                    if (msg.MsgLength < bs.Length - 6)
                    {
                        msg.BufferDuoYu = br.ReadBytes(bs.Length - 6 - msg.MsgLength);
                    }
                }
                ms.Dispose();
                br.Dispose();
            }
            return msg;
        }
    }
}
