using UnityEngine;
using SocketIO;
using System.Collections.Generic;

public interface IPlayerListener
{
    void RotatePlayer(float rotation);
    void MovePlayer(Vector2 target);
}

public class Player
{
    int id;
    string socketID;

    private Vector2 position;
    private float rotation;

    private IPlayerListener listener;

    public Player(int _id, string _socketID, float x, float z, float _rotarion, IPlayerListener _listerner)
    {
        id = _id;
        socketID = _socketID;
        position = new Vector2(x, z);
        rotation = _rotarion;

        listener = _listerner;
    }

    public void SetPosition(float x, float z)
    {
        position.x = x;
        position.y = z;

        listener.MovePlayer(position);
    }

    public void SetRotation(float _rotation)
    {
        rotation = _rotation;
        listener.RotatePlayer(rotation);
    }
}

public class DataReceiver : MonoBehaviour
{
    public SocketIOComponent socket;

    public GameObject npcPrefab;
    private Dictionary<string, Player> npcs = new Dictionary<string, Player>();

    void Start()
    {
        socket.On("connect", (SocketIOEvent e) =>
        {
            print(e.data);
        });

        socket.On("update", (SocketIOEvent e) =>
        {
            if(e.data.HasField(socket.sid))
            {
                e.data.RemoveField(socket.sid);
            }

            foreach (var id in e.data.keys)
            {
                StartCoroutine(UpdateNPC(id, e.data.GetField(id)));
                //UpdateNPC(id, e.data.GetField(id));
            }
        });
    }

    private Vector2 GetPosition(JSONObject data)
    {
        return new Vector2(data.GetField("x").f, data.GetField("z").f);
    }

    private float GetRotation(JSONObject data)
    {
        return data.GetField("r").f;
    }

    private IEnumerator<int> UpdateNPC(string id, JSONObject data)
    {
        Vector2 position = GetPosition(data);
        //float rotation = GetRotation(data);

        if (npcs.ContainsKey(id))
        {
            print("updating player position");
            npcs[id].SetPosition(position.x, position.y);
            //npcs[id].SetRotation(rotation);
        }
        else
        {
            print("new player detected with id: " + id);

            var prefab = GameObject.Instantiate(npcPrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
            var npc = new Player(0, id, position.x, position.y, 0.0f, prefab.GetComponent<RemoteController>());

            npcs.Add(id, npc);
        }

        yield return 0;
    }
}
