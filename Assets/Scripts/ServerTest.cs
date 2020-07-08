using UnityEngine;
using SocketIO;

public class ServerTest : MonoBehaviour
{
    public SocketIOComponent socket;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        socket.On("connect", (SocketIOEvent e) =>
        {
            print("connected");
        });
    }

    private void Update()
    {
        JSONObject position = new JSONObject();

        position.AddField("x", player.transform.position.x);
        position.AddField("z", player.transform.position.z);

        socket.Emit("position", position);
    }
}
