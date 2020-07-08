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

    public GameObject playerPrefab;
    private Dictionary<string, Player> players;

    void Start()
    {
        socket.On("connect", (SocketIOEvent e) =>
        {
            players = new Dictionary<string, Player>();
            print(e.data);
        });

        socket.On("update", (SocketIOEvent e) =>
        {
            //e.data.RemoveField(socket.sid); //remove self from data;
            var playerIDS = e.data.ToDictionary().Keys;
            
            foreach (var id in playerIDS)
            {
                print(id);
            }
        });
    }
}
