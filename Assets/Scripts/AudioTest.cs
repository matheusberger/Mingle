﻿using System;
using System.Collections;
using UnityEngine;
using Unity.Collections;
using Unity.WebRTC;
using Boo.Lang;
using System.Linq;

public class AudioTest : MonoBehaviour
{
    public SocketManager socketManager;

    private RTCPeerConnection peerConnection;
    private RTCDataChannel dataChannel;

    private List<RTCRtpSender> peerSenders;
    private List<RTCRtpSender> peerReceivers;
    private MediaStream audioStream;
    [SerializeField]
    private bool audioUpdateStarted = false;

    private RTCOfferOptions OfferOptions = new RTCOfferOptions
    {
        iceRestart = false,
        offerToReceiveAudio = true,
        offerToReceiveVideo = true
    };

    private RTCAnswerOptions AnswerOptions = new RTCAnswerOptions
    {
        iceRestart = false
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
        peerSenders = new List<RTCRtpSender>();
        peerReceivers = new List<RTCRtpSender>();
    }

    private void Start()
    {
        var configuration = GetSelectedSdpSemantics();
        peerConnection = new RTCPeerConnection(ref configuration);
        peerConnection.OnIceCandidate = e =>
        {
            if (!string.IsNullOrEmpty(e.candidate))
            {
                //send candidate to server
                socketManager.SendICECandidate(e);
            }
        };
        peerConnection.OnIceConnectionChange = (RTCIceConnectionState state) =>
        {
            print("local ice status: " + state);
        };

        socketManager.AwaitICECandidate(candidate =>
        {
            if (!string.IsNullOrEmpty(candidate.candidate))
            {
                peerConnection.AddIceCandidate(ref candidate);
            }
        });

        peerConnection.OnTrack = e =>
        {
            print("remote added a track");
            print(" before i had " + peerConnection.GetSenders().Count());
            peerReceivers.Add(peerConnection.AddTrack(e.Track, audioStream));
            print(" now i have " + peerConnection.GetSenders().Count());
        };

        var dataConfig = new RTCDataChannelInit(true);
        dataChannel = peerConnection.CreateDataChannel("data", ref dataConfig);

        socketManager.AwaitRTCOffer(offer =>
        {
            StartCoroutine(CreateAnswer(offer));
        });

        audioStream = Audio.CaptureStream();
        AddTracks();
    }

    private void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            StartCoroutine(Call());
        }
    }

    private void AddTracks()
    {
        foreach (var track in audioStream.GetTracks())
        {
            peerSenders.Add(peerConnection.AddTrack(track, audioStream));
        }
        if (!audioUpdateStarted)
        {
            StartCoroutine(WebRTC.Update());
            audioUpdateStarted = true;
        }
    }

    IEnumerator Call()
    {
        if (socketManager.ConfirmConnection())
        {
           var op = peerConnection.CreateOffer(ref OfferOptions);
           yield return op;

            if (!op.IsError)
            {
                yield return StartCoroutine(OnCreateOffer(op.Desc));
            }
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
        }
    }

    IEnumerator CreateAnswer(RTCSessionDescription desc)
    {
        var op = peerConnection.SetRemoteDescription(ref desc);
        yield return op;

        if  (!op.IsError)
        {
            var op2 = peerConnection.CreateAnswer(ref AnswerOptions);
            yield return op2;

            if(!op2.IsError)
            {
                yield return OnCreateAnswerSuccess(op2.Desc);
            }
        }
        else
        {
            print(op.Error);
        }
    }

    IEnumerator OnCreateAnswerSuccess(RTCSessionDescription desc)
    {
        var op = peerConnection.SetLocalDescription(ref desc);
        yield return op;

        if (!op.IsError)
        {
            //send answer
            socketManager.SendRTCAnswer(desc);
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        print("updating audio");
        Audio.Update(data, channels);
    }

    private void OnDestroy()
    {
        if (peerConnection != null)
        {
            foreach (var track in peerReceivers)
            {
                peerConnection.RemoveTrack(track);
            }
            foreach (var track in peerSenders)
            {
                peerConnection.RemoveTrack(track);
            }

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
