using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioTest : MonoBehaviour
{
    private AudioClip clip;
    public float[] samples;

    private float offset = 0;

    // Start is called before the first frame update
    void Start()
    {
        clip = Microphone.Start(Microphone.devices[0], true, 10, 44100);
    }

    // Update is called once per frame
    void Update()
    {
        offset = offset % 10;
        offset += Time.deltaTime;

        samples = new float[1 * 44100];
        clip.GetData(samples, (int)offset);
    }
}
