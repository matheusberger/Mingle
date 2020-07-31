using System;
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
        socketManager.ListenToOffers(offer =>
        {
            var desc = new RTCSessionDescription();
            desc.sdp = offer.data;
            StartCoroutine(CreateAnswer(desc));
        });
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
                socketManager.SendIceCandidate(e);
            }
        };
        peerConnection.OnIceConnectionChange = (RTCIceConnectionState state) =>
        {
            print("local ice mudou de estado: " + state);
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
            //send desc to server
            socketManager.SendOffer(offerDesc);
            // and wait for answer
        }
    }

    IEnumerator OnReceiveAnswer(RTCSessionDescription desc)
    {
        print("recebi uma resposta");
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
        print("recebi proposta, vou criar resposta");
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
            print("to enviando a resposta");
            socketManager.SendAnswer(desc);
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
