using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDKManger : UnitySingleton<SDKManger>
{


    protected override void Awake()
    {
        base.Awake();

        // 注册按钮点击事件
        Messenger.AddListener(Msg_Define.MSG_LOGIN, Qlogin);

    }

    AndroidJavaObject ao;

    private void Start()
    {
        AndroidJavaClass ac = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        ao = ac.GetStatic<AndroidJavaObject>("currentActivity");
    }

    // QQ登陆
    private void Qlogin()
    {
        ao.Call("LoginAndSend");
    }




    // QQ登陆回调
    public void LoginInfo(string str)
    {
        // 发送消息回调到UI层显示
        Messenger.Broadcast<string>(Msg_Define.MSG_LOGIN_BACK, str);
    }

    // QQ登陆成功后进一步获取玩家数据xianshi
    public void UserInfo(string str)
    {
        // 发送消息回调到UI层显示
        Messenger.Broadcast<string>(Msg_Define.MSG_USER_INFO_BACK, str);
    }


}
