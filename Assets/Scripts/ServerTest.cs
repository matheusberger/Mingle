using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.Events;

public class ServerTest : MonoBehaviour
{
    public SocketIOController socket;
    public GameObject jammo;

    private PlayerData playerData;
    private bool connected = false;

    // Start is called before the first frame update
    void Start()
    { 
        socket.On("connect", (SocketIOEvent e) =>
        {
            print("connected");
            connected = true;

            playerData = new PlayerData(0, socket.SocketID, jammo.transform.position.x, jammo.transform.position.z, jammo.transform.rotation.y, null);
        });

        socket.Connect();
    }

    private void Update()
    {
        if (connected)
        {
            playerData.SetPosition(jammo.transform.position.x, jammo.transform.position.z);
            playerData.SetRotation(jammo.transform.rotation.y);

            var json = JsonUtility.ToJson(playerData);

            socket.Emit("position", json);
        }
    }
}
