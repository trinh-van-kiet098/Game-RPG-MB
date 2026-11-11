using System;
using System.IO;
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

}