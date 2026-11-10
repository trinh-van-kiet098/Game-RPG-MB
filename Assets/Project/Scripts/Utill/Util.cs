using System;
using System.Text;

public static class Util
{
    public static int GetByte(byte[] buffer, int index, out byte value)
    {
        EnsureRange(buffer, index, 1);
        value = buffer[index];
        return index + 1;
    }

    public static int GetShort(byte[] buffer, int index, out short value)
    {
        EnsureRange(buffer, index, 2);
        int rawValue = buffer[index] | (buffer[index + 1] << 8);
        value = unchecked((short)rawValue);
        return index + 2;
    }

    public static int GetInteger(byte[] buffer, int index, out int value)
    {
        EnsureRange(buffer, index, 4);
        value = unchecked(
            buffer[index]
            | (buffer[index + 1] << 8)
            | (buffer[index + 2] << 16)
            | (buffer[index + 3] << 24));
        return index + 4;
    }

    public static int GetFloat(byte[] buffer, int index, out float value)
    {
        index = GetInteger(buffer, index, out int bits);
        value = BitConverter.Int32BitsToSingle(bits);
        return index;
    }

    public static int GetString(byte[] buffer, int index, out string value)
    {
        index = GetShort(buffer, index, out short length);
        if (length < 0)
            throw new FormatException("Invalid string length.");

        EnsureRange(buffer, index, length);
        value = Encoding.UTF8.GetString(buffer, index, length);
        return index + length;
    }

    public static byte[] InitToByte(int value)
    {
        byte[] bytes = new byte[4];
        SetInteger(bytes, 0, value);
        return bytes;
    }

    public static byte[] ShortToByte(int value)
    {
        byte[] bytes = new byte[2];
        SetShort(bytes, 0, value);
        return bytes;
    }

    public static int SetShort(byte[] buffer, int index, int value)
    {
        EnsureRange(buffer, index, 2);
        buffer[index] = (byte)value;
        buffer[index + 1] = (byte)(value >> 8);
        return index + 2;
    }

    public static int SetByte(byte[] buffer, int index, byte value)
    {
        EnsureRange(buffer, index, 1);
        buffer[index] = value;
        return index + 1;
    }

    public static int SetInteger(byte[] buffer, int index, int value)
    {
        EnsureRange(buffer, index, 4);
        buffer[index] = (byte)value;
        buffer[index + 1] = (byte)(value >> 8);
        buffer[index + 2] = (byte)(value >> 16);
        buffer[index + 3] = (byte)(value >> 24);
        return index + 4;
    }

    public static int SetFloat(byte[] buffer, int index, float value)
    {
        return SetInteger(buffer, index, BitConverter.SingleToInt32Bits(value));
    }

    public static int SetString(byte[] buffer, int index, string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        byte[] encoded = Encoding.UTF8.GetBytes(value);
        if (encoded.Length > short.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(value), "Encoded string is too long.");

        EnsureRange(buffer, index, sizeof(short) + encoded.Length);
        index = SetShort(buffer, index, encoded.Length);
        Buffer.BlockCopy(encoded, 0, buffer, index, encoded.Length);
        return index + encoded.Length;
    }

    private static void EnsureRange(byte[] buffer, int index, int count)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        if (index < 0 || count < 0 || index > buffer.Length - count)
            throw new ArgumentOutOfRangeException(nameof(index));
    }
    // Thêm vào class Util
    public static int SetLong(byte[] buf, int idx, long val)
    {
        Buffer.BlockCopy(BitConverter.GetBytes(val), 0, buf, idx, 8);
        return idx + 8;
    }

    public static int GetLong(byte[] buf, int idx, out long val)
    {
        val = BitConverter.ToInt64(buf, idx);
        return idx + 8;
    }
}
