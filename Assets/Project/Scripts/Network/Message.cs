using System;
using System.IO;
<<<<<<< HEAD
using System.Net;
using System.Text;

public class Message
{
    public int command;
    private MemoryStream os;
    private BinaryWriter dos;
    private MemoryStream is_;
    private BinaryReader dis;
    private byte[] bytes;

    public Message(int command)
    {
        this.command = command;
        os = new MemoryStream();
        dos = new BinaryWriter(os);
    }

    public Message(int command, byte[] data)
    {
        this.command = command;
        is_ = new MemoryStream(data);
        dis = new BinaryReader(is_);
    }

    // ---------- WRITE ----------
    public void WriteByte(byte v)   => dos.Write(v);
    public void WriteSByte(sbyte v) => dos.Write(v);
    public void WriteShort(short v) => dos.Write(IPAddress.HostToNetworkOrder(v));   // big-endian
    public void WriteInt(int v)     => dos.Write(IPAddress.HostToNetworkOrder(v));
    public void WriteLong(long v)   => dos.Write(IPAddress.HostToNetworkOrder(v));
    public void WriteBool(bool v)   => dos.Write(v);
    public void WriteBytes(byte[] b)=> dos.Write(b);

    // Java DataOutputStream.writeUTF(): 2-byte length + modified UTF-8
    public void WriteUTF(string s)
    {
        byte[] raw = Encoding.UTF8.GetBytes(s);
        WriteShort((short)raw.Length);
        dos.Write(raw);
    }

    // ---------- READ ----------
    public byte   ReadByte()   => dis.ReadByte();
    public sbyte  ReadSByte()  => dis.ReadSByte();
    public short  ReadShort()  => IPAddress.NetworkToHostOrder(dis.ReadInt16());
    public int    ReadInt()    => IPAddress.NetworkToHostOrder(dis.ReadInt32());
    public long   ReadLong()   => IPAddress.NetworkToHostOrder(dis.ReadInt64());
    public bool   ReadBool()   => dis.ReadBoolean();
    public byte[] ReadBytes(int n) => dis.ReadBytes(n);
    public int    Available    => (int)(is_.Length - is_.Position);

    public string ReadUTF()
    {
        short len = ReadShort();
        byte[] raw = dis.ReadBytes(len);
        return Encoding.UTF8.GetString(raw);
    }

    // ---------- SERIALIZE (giống Java) ----------
    public byte[] ToByteArray()
    {
        if (bytes != null) return bytes;

        dos.Flush();
        byte[] data = os.ToArray();
        int datalen = data.Length;

        using (var bos = new MemoryStream(3 + datalen))
        using (var w = new BinaryWriter(bos))
        {
            w.Write((byte)command);                          // command
            w.Write(IPAddress.HostToNetworkOrder((short)datalen)); // length BE
            if (datalen > 0) w.Write(data);
            w.Flush();
            bytes = bos.ToArray();
        }
        return bytes;
    }

    public void Cleanup()
    {
        try { dis?.Close(); } catch { }
        try { dos?.Close(); } catch { }
    }
=======
using System.Net.Sockets;
public class Message
{
    public static int MAX_PAYLOAD = 10000;
    public byte command;
    public StreamWriter dos;
    public StreamWriter dis;

    public Message(byte command)
    {
        this.command = command;
        dos = new StreamWriter(new MemoryStream());
    }
    public Message(byte command, byte[] payload)
    {
        this.command = command;
        dos = new StreamWriter(new MemoryStream());
        dos.Write(payload);
    }
    // public byte[] toByteArray()
    // {
    //     // if(dos=null)
    //     // {
    //     //     return new byte[0];
    //     // }
    // }

>>>>>>> 4ee85cb289424c079502c400343c9ade465c1977
}