using System;
using System.IO;

using System.Net.Sockets;
using System.Threading;
public class Network{
    private TcpClient tcpClient;
    private string ipAddress ="127.0.0.1";
    private int port = 19129;
    private NetworkStream networkStream;
    public StreamWriter dos;
    public StreamReader dis;
    private XorCipher xorCipher;
    public Network(){
        tcpClient = new TcpClient();
        tcpClient.Connect(ipAddress, port);
        tcpClient.NoDelay = true;
        networkStream = tcpClient.GetStream();
        dos = new StreamWriter (networkStream);
        dis = new StreamReader(networkStream);
        Thread thread = new Thread(() =>
        {
            while (true)
            {
                if (networkStream.DataAvailable)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                    // Xử lý dữ liệu nhận được từ server
                    Console.WriteLine("Received data: " + BitConverter.ToString(buffer, 0, bytesRead));
                }
            }
        });
        thread.Start();
    }
}