using Unity.WebRTC;
using UnityEngine;

public class MicrophoneManager : MonoBehaviour
{
    public const int FREQUENCY = 44100;
    private AudioClip mic;
    private int lastPos, pos;

    private AudioSource audio;

    // Use this for initialization
    void Start()
    {
        mic = Microphone.Start(null, true, 10, FREQUENCY);

        audio = GetComponent<AudioSource>();
        audio.clip = AudioClip.Create("test", 10 * FREQUENCY, mic.channels, FREQUENCY, false);
        audio.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        if ((pos = Microphone.GetPosition(null)) > 0)
        {
            if (lastPos > pos) lastPos = 0;

            if (pos - lastPos > 0)
            {
                // Allocate the space for the sample.
                float[] sample = new float[(pos - lastPos) * mic.channels];

                // Get the data from microphone.
                mic.GetData(sample, lastPos);

                // Put the data in the audio source.
                audio.clip.SetData(sample, lastPos);

                if (!audio.isPlaying) audio.Play();

                lastPos = pos;
            }
        }
    }

    void OnDestroy()
    {
        Microphone.End(null);
    }
}
