using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Network
{
    public int userID = -1;
    public int serviceID = -1;
    public string username = "";
    public string version = "0.1";
    public byte firmWare = 0;

    private TcpClient client;
    private NetworkStream stream;
    private volatile bool connected;

    // key XOR
    private byte[] key;
    private int curR, curW;

    private Thread readThread, writeThread;
    private readonly BlockingCollection<byte[]> sendQueue = new BlockingCollection<byte[]>();
    private readonly ConcurrentQueue<Message> recvQueue = new ConcurrentQueue<Message>();

    // callback xử lý message (gọi trên main thread - xem Update())
    public Action<Network, Message> onMessage;

    public bool IsConnected => connected;

    public void Connect(string host, int port)
    {
        client = new TcpClient();
        client.NoDelay = true;
        client.Connect(host, port);
        stream = client.GetStream();
        connected = true;

        readThread  = new Thread(ReadLoop)  { IsBackground = true, Name = "READ" };
        writeThread = new Thread(WriteLoop) { IsBackground = true, Name = "SEND" };
        readThread.Start();
        writeThread.Start();
    }

    public void SetKey(byte[] k)
    {
        key = k;
        curR = curW = 0;
    }

    // =================== XOR helpers ===================
    private byte ReadKey(byte b)
    {
        byte r = (byte)(key[curR] ^ b);
        curR = (curR + 1) % key.Length;
        return r;
    }
    private byte WriteKey(byte b)
    {
        byte r = (byte)(key[curW] ^ b);
        curW = (curW + 1) % key.Length;
        return r;
    }

    // =================== READ ===================
    private void ReadLoop()
    {
        try
        {
            while (connected)
            {
                Message m = ReadMessage();
                if (m == null) break;
                recvQueue.Enqueue(m);
            }
        }
        catch (Exception e) { Debug.LogWarning("[Session] read: " + e.Message); }
        finally { Disconnect(); }
    }

    private Message ReadMessage()
    {
        int b = stream.ReadByte();
        if (b < 0) return null;
        byte cmd = (byte)b;
        if (key != null) cmd = ReadKey(cmd);

        int hi = stream.ReadByte();
        int lo = stream.ReadByte();
        if (hi < 0 || lo < 0) return null;

        short size;
        if (key != null)
        {
            int h = ReadKey((byte)hi) & 0xff;
            int l = ReadKey((byte)lo) & 0xff;
            size = (short)((h << 8) | l);
        }
        else
            size = (short)((hi << 8) | lo);

        // Java check >10000 → throw
        if (size < 0 || size > 10000) throw new Exception("Data too big: " + size);

        byte[] data = new byte[size];
        int read = 0;
        while (read < size)
        {
            int len = stream.Read(data, read, size - read);
            if (len <= 0) return null;
            read += len;
        }

        if (key != null)
            for (int i = 0; i < data.Length; i++)
                data[i] = ReadKey(data[i]);

        return new Message(cmd, data);
    }

    // =================== WRITE ===================
    public void SendMessage(Message m)
    {
        if (!connected || m == null) return;

        byte[] raw = m.ToByteArray();          // [cmd][len BE][data]
        if (key != null && raw[0] != unchecked((byte)-40))
        {
            byte[] enc = new byte[raw.Length];
            for (int i = 0; i < raw.Length; i++)
                enc[i] = WriteKey(raw[i]);
            raw = enc;
        }
        sendQueue.Add(raw);
        m.Cleanup();
    }

    // Gửi thẳng như sendMessageLogin — dùng ở giai đoạn handshake (không XOR)
    public void SendMessageLogin(Message m)
    {
        if (stream == null) return;
        byte[] raw = m.ToByteArray();
        stream.Write(raw, 0, raw.Length);
        stream.Flush();
        m.Cleanup();
    }

    private void WriteLoop()
    {
        try
        {
            foreach (var bytes in sendQueue.GetConsumingEnumerable())
            {
                if (!connected) break;
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }
        }
        catch (Exception e) { Debug.LogWarning("[Session] send: " + e.Message); }
    }

    // =================== Unity main-thread dispatcher ===================
    // Gọi trong Update() của một MonoBehaviour
    public void Pump()
    {
        while (recvQueue.TryDequeue(out var m))
            onMessage?.Invoke(this, m);
    }

    public void Disconnect()
    {
        if (!connected) return;
        connected = false;
        try { sendQueue.CompleteAdding(); } catch { }
        try { stream?.Close(); }  catch { }
        try { client?.Close(); }  catch { }
    }
}