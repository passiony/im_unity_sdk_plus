using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class MicrophoneInput : MonoSingleton<MicrophoneInput>
{
    protected AudioSource audioSource;
    private int channels = 1;
    private int lengthSec = 20;
    private int frequency = 44100;
    private bool isRecoding = false;

    void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    public IEnumerator RequestMicrophoneAuth()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
    }

    public bool IsRecording()
    {
        return isRecoding;
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }
    
    public void StartRecord()
    {
        Debug.Log("开始录音");

        isRecoding = true;
        audioSource.Stop();
        audioSource.loop = false;
        audioSource.mute = true;
        audioSource.clip = Microphone.Start(null, false, lengthSec, 44100);
        audioSource.Play();
    }

    public int StopRecord()
    {
        int length = Microphone.GetPosition(null);
        Microphone.End(null);
        audioSource.Stop();
        isRecoding = false;
        return length;
    }

    public void SaveAudioFile(string filePath, int length)
    {
        if (Microphone.IsRecording(null))
            return;

        byte[] data = GetClipData(audioSource, length);
        if (data == null)
        {
            return;
        }

        FileUtility.SafeWriteAllBytes(filePath, data);
        string infoLog = "total length:" + data.Length + " time:" + audioSource.time;
        Debug.Log(infoLog);
    }

    public void ReadAudioFile(string filePath)
    {
        byte[] bytes = FileUtility.SafeReadAllBytes(filePath);
        float[] samples = byte2float(bytes);

        var filename = Path.GetFileName(filePath);
        audioSource.Stop();
        audioSource.clip = AudioClip.Create(filename, samples.Length, channels, frequency, false);
        audioSource.clip.SetData(samples, 0);
        audioSource.mute = false;
        audioSource.Play();
    }

    public void PlayAudioFile(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.mute = false;
        audioSource.Play();
    }
    
    byte[] GetClipData(AudioSource source, int length)
    {
        if (length < 10)
        {
            Debug.Log("录音文件太短");
            return null;
        }

        float[] samples = new float[length];
        source.clip.GetData(samples, 0);

        byte[] outData = float2byte(samples);
        return outData;
    }

    byte[] float2byte(float[] floats)
    {
        byte[] outData = new byte[floats.Length * 2];
        int reScaleFactor = 32767;

        for (int i = 0; i < floats.Length; i++)
        {
            short tempShort = (short) (floats[i] * reScaleFactor);
            byte[] tempData = BitConverter.GetBytes(tempShort);

            outData[i * 2] = tempData[0];
            outData[i * 2 + 1] = tempData[1];
        }

        return outData;
    }

    float[] byte2float(byte[] bytes)
    {
        float reScaleFactor = 32768.0f;
        float[] data = new float[bytes.Length / 2];
        for (int i = 0; i < bytes.Length; i += 2)
        {
            short s;
            if (BitConverter.IsLittleEndian) //小端和大端顺序要调整
                s = (short) ((bytes[i + 1] << 8) | bytes[i]);
            else
                s = (short) ((bytes[i] << 8) | bytes[i + 1]);
            // convert to range from -1 to (just below) 1
            data[i / 2] = s / reScaleFactor;
        }

        return data;
    }
}