using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.Events;


public class SocketManager : MonoBehaviour
{
    public SocketIOController socket;

    private bool isConnected = false;

    public bool ConfirmConnection()
    {
        return isConnected;
    }

    public string GetSocketID()
    {
        return socket.SocketID;
    }

    public void ReceiveData(Action<PlayerDataArray> callback)
    {
        socket.On("update", (SocketIOEvent e) =>
        {
            var players = JsonUtility.FromJson<PlayerDataArray>(e.data);
            callback(players);
        });
    }

    public void SendData(PlayerData playerData)
    {
        var json = JsonUtility.ToJson(playerData);

        socket.Emit("position", json);
    }

    // Start is called before the first frame update
    void Start()
    {
        socket.On("connect", (SocketIOEvent e) =>
        {
            isConnected = true;
            print("Connected to server!");
        });

        socket.Connect();
    }
}
