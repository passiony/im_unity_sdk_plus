using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.tencent.imsdk.unity;
using com.tencent.imsdk.unity.callback;
using com.tencent.imsdk.unity.enums;
using com.tencent.imsdk.unity.types;
using demo.tencent.im.unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IMDemo : MonoBehaviour
{
    public Text imText;
    public InputField username;
    public InputField input;
    public Button microBtn;
    public Button craeteGroupBtn;
    public Button joinGroupBtn;
    public Button getGroupBtn;
    public Button sendBtn;
    public Button loginBtn;
    public Button logoutBtn;
    public Button getMsgListBtn;
    private Text microText;

    private bool voice = false;

    StringBuilder sb = new StringBuilder();

    void Awake()
    {
        loginBtn.onClick.AddListener(onLoginClick);
        logoutBtn.onClick.AddListener(onLogoutClick);
        microText = microBtn.transform.GetChild(0).GetComponent<Text>();
        sendBtn.onClick.AddListener(onSendClick);
        microBtn.onClick.AddListener(onMicroClick);
        craeteGroupBtn.onClick.AddListener(onCreateClick);
        joinGroupBtn.onClick.AddListener(onJoinClick);
        getGroupBtn.onClick.AddListener(onGetGroupClick);
        getMsgListBtn.onClick.AddListener(onGetMessageList);
    }

    private void Start()
    {
        IMManager.Instance.Init();
        AddListeners();
    }
    
    public void AddListeners()
    {
        TencentIMSDK.AddRecvNewMsgCallback(RecvNewMsgCallback); // 接收消息事件
        TencentIMSDK.SetConvEventCallback(OnConvCallback);//拉取会话列表,会话更新
        // TencentIMSDK.SetMsgReadedReceiptCallback(MsgReadedReceiptCallback); // 消息已读回调
        // TencentIMSDK.SetMsgRevokeCallback(MsgRevokeCallback); // 消息撤回回调
        // TencentIMSDK.SetMsgElemUploadProgressCallback(MsgElemUploadProgressCallback); // 多媒体消息发送进度回调
        TencentIMSDK.SetGroupTipsEventCallback(GroupTipsEventCallback); // 群tips回调
        // TencentIMSDK.SetGroupAttributeChangedCallback(GroupAttributeChangedCallback); // 群属性改变
    }

    
    void DealMessage(Message msg)
    {
        var elem = msg.message_elem_array[0];
        if (elem.elem_type == TIMElemType.kTIMElem_Text)
        {
            AddMsgText(elem.text_elem_content);
        }
        else if (elem.elem_type == TIMElemType.kTIMElem_Sound)
        {
            AddMsgText("语音");
            string url = elem.sound_elem_url;
            string filepath = Application.persistentDataPath + CustomConfigs.IM_Sound_Path + elem.sound_elem_file_id;
            UnityWebRequestManager.Instance.DownloadFile(url, filepath, (www) =>
            {
                Debug.Log("下载语音成功");
                MicrophoneInput.Instance.ReadAudioFile(filepath);
            });
        }
    }
    
    void AddMsgText(string text)
    {
        sb.AppendLine(text);
        imText.text = sb.ToString();
    }
    
    private void OnConvCallback(TIMConvEvent conv_event, List<ConvInfo> conv_list, string user_data)
    {
        foreach (var conv in conv_list)
        {
            if (conv.conv_type == TIMConvType.kTIMConv_Group)
            {
                if (conv.conv_is_has_lastmsg)
                {
                    DealMessage(conv.conv_last_msg);
                }
            }
        }
    }

    private void GroupTipsEventCallback(List<GroupTipsElem> message, string user_data)
    {
        foreach (var msg in message)
        {
            Debug.Log(msg.ToString());
        }
    }

    private void RecvNewMsgCallback(List<Message> message, string user_data)
    {
        Debug.Log("RecvNewMsgCallback");
        foreach (var msg in message)
        {
            DealMessage(msg);
        }
    }

    #region Button Event
    
    private void onGetMessageList()
    {
        var res = IMManager.Instance.GetMessgeList();
        if (res == TIMResult.TIM_SUCC)
        {
            Debug.Log("拉取历史消息列表");
        }
    }
    private void onMicroClick()
    {
        if (voice)
        {
            voice = false;
            microText.text = "开始录制";
            int length = MicrophoneInput.Instance.StopRecord();
            string audioPath = Application.persistentDataPath + CustomConfigs.IM_Sound_Path+DateTime.Now+".wav";
            MicrophoneInput.Instance.SaveAudioFile(audioPath, length);

            Debug.Log("send sound msg:" + audioPath);
            var res = IMManager.Instance.SendSoundMessage(audioPath);
            if (res == TIMResult.TIM_SUCC)
            {
                AddMsgText("录音");
            }
        }
        else
        {
            voice = true;
            MicrophoneInput.Instance.StartRecord();
            microText.text = "停止录制";
        }
    }

    private void onLoginClick()
    {
        if (string.IsNullOrEmpty(username.text))
        {
            Debug.Log("input a userid and user_sig first");
            return;
        }

        int index = int.Parse(username.text);
        string userid = index == 0 ? CustomConfigs.user_id1 : CustomConfigs.user_id2;
        string user_sig = index == 0 ? CustomConfigs.user_sig1 : CustomConfigs.user_sig2;

        IMManager.Instance.SetUser(userid,user_sig);
        var res = IMManager.Instance.Login();
        if (res == TIMResult.TIM_SUCC)
        {
            AddMsgText("登录成功");
        }
    }

    private void onLogoutClick()
    {
        var res = IMManager.Instance.Logout();
        if (res == TIMResult.TIM_SUCC)
        {
            AddMsgText("退出登录");
        }
    }
    
    private void onSendClick()
    {
        var text = input.text;
        if (string.IsNullOrEmpty(text))
        {
            Debug.Log("input 文本为空");
            return;
        }
       var res = IMManager.Instance.SendTextMessage(text);
       if (res == TIMResult.TIM_SUCC)
       {
           AddMsgText(text);
       }
    }

    private void onCreateClick()
    {
        var res = IMManager.Instance.GroupCreate("","");
        if (res == TIMResult.TIM_SUCC)
        {
            AddMsgText("获取群聊列表");
        }
    }

    private void onJoinClick()
    {
        var res = IMManager.Instance.GroupJoin("");
        if (res == TIMResult.TIM_SUCC)
        {
            AddMsgText("加入群聊成功");
        }
    }

    public void onGetGroupClick()
    {
        var res = IMManager.Instance.GetGroups();
        if (res == TIMResult.TIM_SUCC)
        {
            AddMsgText("获取群聊列表");
        }
    }

    #endregion

}