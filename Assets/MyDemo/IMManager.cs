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

public class IMManager : MonoSingleton<IMManager>
{
    public static string groupId = CustomConfigs.groupId;
    public static string sdkappid = CustomConfigs.sdk_app_id.ToString();

    public static string userid;
    public static string user_sig;


    public void SetUser(string id,string sig)
    {
        userid = id;
        user_sig = sig;
    }
    
    public TIMResult Init()
    {
        SdkConfig sdkConfig = new SdkConfig();

        sdkConfig.sdk_config_config_file_path = Application.persistentDataPath + "/TIM-Config";

        sdkConfig.sdk_config_log_file_path = Application.persistentDataPath + "/TIM-Log";

        if (sdkappid == "")
        {
            Debug.Log("input a sdkappid first");
            return TIMResult.TIM_ERR_PARAM;
        }

        TIMResult res = TencentIMSDK.Init(long.Parse(sdkappid), sdkConfig);
        Debug.Log("Init:" + res);
        return res;
    }
    
    public TIMResult Login()
    {
        // 业务可以替换自己的 sdkappid
        TIMResult res = TencentIMSDK.Login(userid, user_sig, addAsyncDataToConsole);
        Debug.Log("Login:" + res);
        return res;
    }

    public TIMResult Logout()
    {
        // 业务可以替换自己的 sdkappid
        TIMResult res = TencentIMSDK.Logout(addAsyncDataToConsole);
        Debug.Log("Logout:" + res);
        return res;
    }
    
    public TIMResult GroupCreate(string gorupId,string groupName)
    {
        CreateGroupParam param = new CreateGroupParam();
        param.create_group_param_group_id = groupId;
        param.create_group_param_group_name = "name";
        param.create_group_param_group_type = TIMGroupType.kTIMGroup_ChatRoom;
        param.create_group_param_add_option = TIMGroupAddOption.kTIMGroupAddOpt_Any;
        param.create_group_param_notification = "create_group_param_notification";
        param.create_group_param_introduction = "create_group_param_introduction";
        param.create_group_param_face_url = "https://yq.sukeni.com/Logo.jpg";
        TIMResult res = TencentIMSDK.GroupCreate(param, addAsyncDataToConsole);
        Debug.Log("GroupCreate:" + res);
        return res;
    }

    public TIMResult GroupJoin(string gorupId)
    {
        TIMResult res = TencentIMSDK.GroupJoin(groupId, "hello", addAsyncDataToConsole);
        Debug.Log("GroupJoin:" + res);
        return res;
    }

    public TIMResult GetGroups()
    {
        TIMResult res = TencentIMSDK.GroupGetJoinedGroupList(OnGetJoinGroupList);
        Debug.Log("GetGroups:" + res);
        return res;
    }

    private void OnGetJoinGroupList(int code, string desc, string data, string user_data)
    {
        Debug.Log(code + " " + desc + " " + data);
    }

    public void addAsyncDataToConsole(int code, string desc, string json_param, string user_data)
    {
        Debug.Log(user_data + "Asynchronous return: " + "code: " + code.ToString() + " desc:" + desc + " json_param: " +
                  json_param + " ");
    }

    public TIMResult GetMessgeList()
    {
        TIMResult res = TencentIMSDK.ConvGetConvList(addAsyncDataToConsole);
        return res;
    }
    
    public TIMResult SendTextMessage(string text)
    {
        string conv_id = groupId;
        Message message = new Message();
        message.message_conv_id = conv_id;
        message.message_conv_type = TIMConvType.kTIMConv_Group;
        List<Elem> messageElems = new List<Elem>();
        Elem textMessage = new Elem();
        textMessage.elem_type = TIMElemType.kTIMElem_Text;
        textMessage.text_elem_content = text;
        messageElems.Add(textMessage);
        message.message_elem_array = messageElems;
        message.message_cloud_custom_str = "unity local custom data";
        StringBuilder messageId = new StringBuilder(128);

        TIMResult res = TencentIMSDK.MsgSendMessage(conv_id, TIMConvType.kTIMConv_Group, message, messageId,
            addAsyncDataToConsole);
        Debug.Log("SendTextMessage:" + res);
        return res;
    }

    public TIMResult SendSoundMessage(string path)
    {
        string conv_id = groupId;
        Message message = new Message();
        message.message_conv_id = conv_id;
        message.message_conv_type = TIMConvType.kTIMConv_Group;
        List<Elem> messageElems = new List<Elem>();
        Elem textMessage = new Elem();
        textMessage.elem_type = TIMElemType.kTIMElem_Sound;
        textMessage.sound_elem_file_path = path;
        messageElems.Add(textMessage);
        message.message_elem_array = messageElems;
        message.message_cloud_custom_str = "unity local custom data";
        StringBuilder messageId = new StringBuilder(128);

        TIMResult res = TencentIMSDK.MsgSendMessage(conv_id, TIMConvType.kTIMConv_Group, message, messageId,
            addAsyncDataToConsole);
        Debug.Log("SendSoundMessage:" + res);
        return res;
    }
}
