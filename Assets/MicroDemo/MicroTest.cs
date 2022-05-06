using UnityEngine;
using UnityEngine.UI;

public class MicroTest : MonoBehaviour
{
    public Button startBtn;
    public Button stopBtn;
    
    void Awake()
    {
        startBtn.onClick.AddListener(this.OnStartClick);
        stopBtn.onClick.AddListener(this.OnStopClick);
    }

    /// <summary>
    /// 开始录制
    /// </summary>
    void OnStartClick()
    {
        Debug.Log("OnStartClick");
        
        if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            StartCoroutine(MicrophoneInput.Instance.RequestMicrophoneAuth());
            return;
        }
        
        MicrophoneInput.Instance.StartRecord();
    }
    
    /// <summary>
    /// 停止录制
    /// </summary>
    private void OnStopClick()
    {
        Debug.Log("OnStopClick");

        int length = MicrophoneInput.Instance.StopRecord();
        var path = Application.persistentDataPath + "/audios/micro.m4a";
        MicrophoneInput.Instance.SaveAudioFile(path, length);

        PlayAudio();
    }

    /// <summary>
    /// 播放录音
    /// </summary>
    private void PlayAudio()
    {
        var path = Application.persistentDataPath + "/audios/micro.m4a";
        //读取本地文件播放
        MicrophoneInput.Instance.ReadAudioFile(path);
    }

}
