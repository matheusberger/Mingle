using System;
using Unity.WebRTC;
using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.Events;


public struct RTCDesc
{
    public string sid;
    public string data;

    public RTCDesc(string id, string desc)
    {
        sid = id;
        data = desc;
    }
}


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

    public void ReceivePlayerData(Action<PlayerDataArray> callback)
    {
        socket.On("update", (SocketIOEvent e) =>
        {
            var players = JsonUtility.FromJson<PlayerDataArray>(e.data);
            callback(players);
        });
    }

    public void SendPlayerData(PlayerData playerData)
    {
        var json = JsonUtility.ToJson(playerData);

        socket.Emit("position", json);
    }

    public void SendRTCOffer(RTCSessionDescription offerDesc, Action<RTCSessionDescription> callback)
    {
        print("Sending offer to server");

        var json = JsonUtility.ToJson(offerDesc);
        socket.Emit("offer", json);

        AwaitRTCAnswer(callback);
    }

    private void AwaitRTCAnswer(Action<RTCSessionDescription> callback)
    {
        socket.On("answer", data =>
        {
            print("received answer from server: " + data.data);
            var answer = JsonUtility.FromJson<RTCSessionDescription>(data.data);
            callback(answer);
        });
    }

    public void AwaitRTCOffer(Action<RTCSessionDescription> callback)
    {
        socket.On("offer", data =>
        {
            print("received offer from server");

            var offer = JsonUtility.FromJson<RTCSessionDescription>(data.data);
            callback(offer);
        });
    }

    public void SendRTCAnswer(RTCSessionDescription answerDesc)
    {
        print("Sending offer to server");

        var json = JsonUtility.ToJson(answerDesc);
        socket.Emit("answer", json);
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

    private void OnDestroy()
    {
        socket.Close();
    }
}
