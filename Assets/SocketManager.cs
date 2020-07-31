using System;
using Unity.WebRTC;
using UnityEngine;
using UnitySocketIO;
using UnitySocketIO.Events;


public struct Offer
{
    public string sid;
    public string data;

    public Offer(string id, string desc)
    {
        sid = id;
        data = desc;
    }
}

public struct Answer
{
    public string sid;
    public string data;

    public Answer(string id, string desc)
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

    public void SendOffer(RTCSessionDescription desc)
    {
        Offer offer = new Offer(socket.SocketID, desc.sdp);

        var json = JsonUtility.ToJson(offer);
        socket.Emit("offer", json);
    }

    public void ListenToOffers(Action<Offer> callback)
    {
        socket.On("offer", (SocketIOEvent e) =>
        {
            var offer = JsonUtility.FromJson<Offer>(e.data);
            print("received offer");
            callback(offer);
        });
    }

    public void SendIceCandidate(RTCIceCandidate candidate)
    {
        print("sending ice candidate");
        socket.Emit("candidate");
    }

    public void SendAnswer(RTCSessionDescription desc)
    {
        Answer answer = new Answer(socket.SocketID, desc.sdp);
        var json = JsonUtility.ToJson(answer);
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
