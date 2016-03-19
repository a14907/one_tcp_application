using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static List<Socket> listSocket = new List<Socket>();
        static void Main(string[] args)
        {
            //创建一个线程接收连接
            Thread th = new Thread(StartServer);
            th.IsBackground = true;
            th.Start();
            Console.WriteLine("监听开始！");
            //创建监听socket
            Console.ReadKey();
            //创建一个容器存储socket
        }

        static void StartServer(object obj)
        {
            Socket listaner = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 23456);
            listaner.Bind(ip);
            listaner.Listen(10);
            while (true)
            {
                Socket s = listaner.Accept();
                listSocket.Add(s);
                Console.WriteLine(s.LocalEndPoint.ToString() + "连接上来了！");
                Thread th = new Thread(Listen);
                th.IsBackground = true;
                th.Start(s);
            }
        }

        static void Listen(object obj)
        {
            Message msg = new Message();
            byte[] buffer = new byte[1024 * 1024 * 5 + 10];
            byte[] newBuffer = new byte[0];
            Socket s = obj as Socket;
            while (true)
            {
                try
                {
                    int length = s.Receive(buffer);//接收数据
                    if (length == 0)
                    {
                        continue;
                    }

                    newBuffer = BufferHelper.CombineBuffer(newBuffer, 0, newBuffer.Length, buffer, 0, length);

                    if (newBuffer.Length <= 6)//说明接受的数据还不够
                    {
                        continue;
                    }
                    else//有数据了
                    {

                        //判断是否之前已经接受了一部分，现在是继续接收
                        if (msg.BufferData != null)
                        {
                            int leftLength = msg.MsgLength - msg.BufferData.Length;
                            if (newBuffer.LongLength <= leftLength)
                            {
                                msg.BufferData = BufferHelper.CombineBuffer(msg.BufferData, 0, msg.BufferData.Length, newBuffer, 0, newBuffer.Length);
                            }
                            else
                            {

                                msg.BufferData = BufferHelper.CombineBuffer(msg.BufferData, 0, msg.BufferData.Length, newBuffer, 0, leftLength);
                                msg.BufferDuoYu = new byte[newBuffer.Length - leftLength];
                                Array.Copy(newBuffer, leftLength, msg.BufferDuoYu, 0, msg.BufferDuoYu.Length);
                            }
                        }
                        else
                        {
                            //从头开始
                            msg = Message.TransferToMessage(newBuffer);

                        }

                        //判断包的内容是否可以用于取数据
                        if ((msg.BufferData.Length == msg.MsgLength))
                        {
                            //判断发送数据的类型
                            ProcessMessage(msg);
                            msg = msg.BufferDuoYu == null ? new Message() : msg;
                        }
                        newBuffer = new byte[0];
                        while (msg.BufferDuoYu != null)
                        {
                            msg = Message.TransferToMessage(msg.BufferDuoYu);
                            ProcessMessage(msg);
                            msg = msg.BufferDuoYu == null ? new Message() : msg;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
            }
        }

        static int countStr = 0;
        private static void ProcessMessage(Message msg)
        {
            if (msg.Flag1 == 0 && msg.Flag2 == 0)
            {
                //取数据                                
                //发送的都是文本数据
                string str = Encoding.UTF8.GetString(msg.BufferData);
                Console.WriteLine("接收到：" + str);
                countStr++;
                Console.WriteLine(countStr);
                File.AppendAllLines(@"E:\data1.txt", new[] { str });
            }
            else if (msg.Flag1 == 0 && msg.Flag2 == 1)
            {
                //jpg图片
                using (FileStream fs = new FileStream(@"C:\Users\Wude\Desktop\" + Guid.NewGuid().ToString() + ".mp3", FileMode.CreateNew))
                {
                    int left = msg.MsgLength;
                    int index = 0;
                    while (left > 0)
                    {
                        int put = left > 10000 ? 10000 : (int)left;
                        fs.Write(msg.BufferData, index, put);
                        left -= 10000;
                        index += 10000;
                    }

                }
                Console.WriteLine("图片接收完毕！");
            }
        }
    }
}
