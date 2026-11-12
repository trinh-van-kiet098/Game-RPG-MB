using System;
public class XorCipher {

<<<<<<< HEAD
    private byte[] key;
=======
    private  byte[] key;
>>>>>>> 4ee85cb289424c079502c400343c9ade465c1977
    private int curR = 0;
    private int curW = 0;

    public XorCipher(byte[] derivedKey) {
        this.key = derivedKey;
    }

    /** Biến đổi khoá thô nhận được trong GET_SESSION_ID thành khoá dùng thật. */
    public static byte[] deriveKey(byte[] rawKey) {
        byte[] k = (byte[]) rawKey.Clone();
        for (int i = 0; i < k.Length - 1; i++)
            k[i + 1] ^= k[i];
        return k;
    }

    /** Giải mã 1 byte vừa đọc từ socket. */
    public byte read(byte b) {
        byte r = (byte) ((key[curR++] & 0xff) ^ (b & 0xff));
        if (curR >= key.Length)
            curR = 0;
        return r;
    }

    /** Mã hoá 1 byte trước khi ghi ra socket. */
    public byte write(byte b) {
        byte r = (byte) ((key[curW++] & 0xff) ^ (b & 0xff));
        if (curW >= key.Length)
            curW = 0;
        return r;
    }
}