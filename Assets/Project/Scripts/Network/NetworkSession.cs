using UnityEngine;

public class NetworkSession : MonoBehaviour
{
    public Network network;

    void Start()
    {
        network = new Network();
        network.onMessage = HandleMessage;
        network.Connect("127.0.0.1", 12345);

        // // ví dụ gửi login (plaintext)
        // var m = new Message(CMD.LOGIN);
        // m.WriteUTF("user"); m.WriteUTF("pass");
        // network.SendMessageLogin(m);
    }

    void Update()
    {
        network.Pump();   // dispatch message về main thread
    }

    void HandleMessage(Network n, Message m)
    {
        switch (m.command)
        {
            // case CMD.SET_KEY:
            //     // server gửi key (tuỳ game - có thể plaintext lúc này)
            //     short klen = m.ReadShort();
            //     byte[] k = m.ReadBytes(klen);
            //     n.SetKey(k);
            //     break;
            // ... các case khác
        }
    }
}
