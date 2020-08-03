﻿using System;
using System.Collections;
using UnityEngine;
using Unity.WebRTC;

public class AudioTest : MonoBehaviour
{
    public SocketManager socketManager;

    private RTCPeerConnection peerConnection;
    private RTCDataChannel dataChannel;

    private RTCOfferOptions OfferOptions = new RTCOfferOptions
    {
        iceRestart = false,
        offerToReceiveAudio = true,
        offerToReceiveVideo = true
    };

    private RTCAnswerOptions AnswerOptions = new RTCAnswerOptions
    {
        iceRestart = false,
    };

    RTCConfiguration GetSelectedSdpSemantics()
    {
        RTCConfiguration config = default;
        config.iceServers = new RTCIceServer[]
        {
            new RTCIceServer { urls = new string[] { "stun:stun.l.google.com:19302" } }
        };

        return config;
    }

    private void Awake()
    {
        WebRTC.Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(socketManager.ConfirmConnection())
        {
            socketManager.AwaitRTCOffer(offer =>
            {
                StartCoroutine(CreateAnswer(offer));
            });
        }
    }

    private void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            StartCoroutine(Call());
        }
    }

    IEnumerator Call()
    {
        var configuration = GetSelectedSdpSemantics();
        peerConnection = new RTCPeerConnection(ref configuration);
        peerConnection.OnIceCandidate = e =>
        {
            if (!string.IsNullOrEmpty(e.candidate))
            {
                //send candidate to server
            }
        };
        peerConnection.OnIceConnectionChange = (RTCIceConnectionState state) =>
        {
            print("local ice status: " + state);
        };

        var dataConfig = new RTCDataChannelInit(true);
        dataChannel = peerConnection.CreateDataChannel("data", ref dataConfig);

        var op = peerConnection.CreateOffer(ref OfferOptions);
        yield return op;

        if (!op.IsError)
        {
            yield return StartCoroutine(OnCreateOffer(op.Desc));
        }
    }

    IEnumerator OnCreateOffer(RTCSessionDescription offerDesc)
    {
        var op = peerConnection.SetLocalDescription(ref offerDesc);
        yield return op;

        if (!op.IsError)
        {
            socketManager.SendRTCOffer(offerDesc, answer =>
            {
                StartCoroutine(OnReceiveAnswer(answer));
            });
        }
    }

    IEnumerator OnReceiveAnswer(RTCSessionDescription desc)
    {
        var op = peerConnection.SetRemoteDescription(ref desc);
        yield return op;

        if (!op.IsError)
        {
            //deu tudo certo!
            print("aeee caraaaai");
        }
    }

    IEnumerator CreateAnswer(RTCSessionDescription desc)
    {
        var op = peerConnection.SetRemoteDescription(ref desc);
        yield return op;

        if  (!op.IsError)
        {
            print("setei remote sem erro");
            var op2 = peerConnection.CreateAnswer(ref AnswerOptions);
            yield return op2;

            if(!op2.IsError)
            {
                yield return OnCreateAnswerSuccess(op2.Desc);
            }
        }
    }

    IEnumerator OnCreateAnswerSuccess(RTCSessionDescription desc)
    {
        print("criei resposta sem erro");
        var op = peerConnection.SetLocalDescription(ref desc);
        yield return op;

        if (!op.IsError)
        {
            //send answer
        }
    }

    private void OnDestroy()
    {
        if (peerConnection != null)
        {
            peerConnection.Close();
            peerConnection = null;
        }

        if (dataChannel != null)
        {
            dataChannel.Close();
        }

        WebRTC.Dispose();
    }
}
