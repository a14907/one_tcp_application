using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建一个socket连接上服务器，发送消息
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 23456));
            Console.WriteLine("按任意键发送数据！");

            string path = @"F:\音乐\你是我的眼.mp3";


            int totalSize = 0;

            //using (FileStream fs = new FileStream(path, FileMode.Open))
            //{
            //    int maxlength = (int)fs.Length;
            //    Console.WriteLine(maxlength);
            //    byte[] imgBuffer = new byte[100000];
            //    while (true)
            //    {
            //        int count = 0;
            //        count = fs.Read(imgBuffer, 0, imgBuffer.Length);
            //        totalSize += count;
            //        if (count == 0)
            //        {
            //            break;
            //        }
            //        Message msg = new Message
            //        {
            //            Flag1 = 0,
            //            Flag2 = 1,
            //            MsgLength = maxlength,
            //            BufferData = imgBuffer
            //        };
            //        var res = msg.TransferToBuffer();
            //        s.Send(res);
            //        Console.WriteLine("发送！");
            //    }
            //}

            for (int i = 0; i < 100; i++)
            {
                var res = Encoding.UTF8.GetBytes("维迪奇" + i);
                Message msg2 = new Message
                {
                    Flag1 = 0,
                    Flag2 = 0,
                    BufferData = res,
                    MsgLength = res.Length
                };
                int intStr = s.Send(msg2.TransferToBuffer());
                Thread.Sleep(10);
                if (intStr == 0)
                {
                    Console.WriteLine("发送出错");
                }
            }


            Console.ReadKey();
        }
    }
}
