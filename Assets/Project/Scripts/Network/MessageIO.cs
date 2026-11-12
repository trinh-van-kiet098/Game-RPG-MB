using System;
using System.IO;
using System.Net.Sockets;
public class MessageIO
{
    public static Message read(NetworkStream dis, XorCipher xorCipher)
    {

        byte command = (byte)dis.ReadByte();
        byte hi = (byte)dis.ReadByte();
        byte lo = (byte)dis.ReadByte();
        if (xorCipher != null)
        {
            command = xorCipher.read(command);
            hi = xorCipher.read(hi);
            lo = xorCipher.read(lo);
        }
        int size = ((hi << 8) | (lo & 0xff));
        if (size > Message.MAX_PAYLOAD)
        {
            throw new IOException("Message size exceeds maximum payload: " + size);
        }

        byte[] payload = new byte[size];
        dis.Read(payload, 0, size);
        if (xorCipher != null)
        {
            for (int i = 0; i < size; i++)
            {
                payload[i] = xorCipher.read(payload[i]);
            }
        }

        return new Message(command, payload);
    }
}