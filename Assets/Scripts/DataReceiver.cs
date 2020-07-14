using UnityEngine;
using System.Collections.Generic;
using UnitySocketIO;
using UnitySocketIO.Events;
using System;
using System.Linq;

public interface IPlayerListener
{
    void RotatePlayer(float rotation);
    void MovePlayer(Vector2 target);
}

[System.Serializable]
public class PlayerData
{
    public int id;
    public string socketID;

    public Vector2 position;
    public float rotation;

    private IPlayerListener listener;

    public PlayerData(int _id, string _socketID, float x, float z, float _rotarion, IPlayerListener _listerner)
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

        if(listener != null)
        {
            listener.MovePlayer(position);
        }
    }

    public void SetRotation(float _rotation)
    {
        rotation = _rotation;
        if (listener != null)
        {
            listener.RotatePlayer(rotation);
        }
    }

    public void SetListener(IPlayerListener _listener)
    {
        listener = _listener;
    }
}

[System.Serializable]
public class PlayerDataArray
{
    public PlayerData[] data;
}

public class DataReceiver : MonoBehaviour
{
    public SocketIOController socket;
    private bool connected = false;

    public GameObject npcPrefab;
    private Dictionary<string, PlayerData> npcs = new Dictionary<string, PlayerData>();

    void Start()
    {
        socket.On("connect", (SocketIOEvent e) =>
        {
            connected = true;
        });

        socket.On("update", (SocketIOEvent e) =>
        {
            var players = JsonUtility.FromJson<PlayerDataArray>(e.data);
            print("updating " + players.data.Length + " players");

            foreach (var playerData in players.data)
            {
                if(playerData.socketID != socket.SocketID)
                {
                    StartCoroutine(UpdateNPC(playerData.socketID, playerData));
                }
            }
        });

        socket.Connect();
    }

    private IEnumerator<int> UpdateNPC(string id, PlayerData data)
    {
        if (npcs.ContainsKey(id))
        {
            npcs[id].SetPosition(data.position.x, data.position.y);
            npcs[id].SetRotation(data.rotation);
        }
        else
        {
            print("new player detected with id: " + id);

            var prefab = GameObject.Instantiate(npcPrefab, new Vector3(data.position.x, 0, data.position.y), Quaternion.identity);
            var npc = data;
            
            npc.SetListener(prefab.GetComponent<RemoteController>());

            npcs.Add(id, npc);
        }

        yield return 0;
    }
}
