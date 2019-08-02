using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestAndroid : MonoBehaviour {

    [SerializeField]
    Text text; //用来显示结果值
    [SerializeField]
    Button btn; // 拉起登陆
    [SerializeField]
    Text userInfo;



    public void Awake()
    {

        // 注册回调
        Messenger.AddListener<string>(Msg_Define.MSG_LOGIN_BACK, setLog);
        Messenger.AddListener<string>(Msg_Define.MSG_USER_INFO_BACK, setUserInfo);
    }

    public void Start()
    {
        //Add();

        // 匿名写法 拉取QQ登陆
        btn.onClick.AddListener(() => {
            //// 固定写法
            //androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //// 固定写法
            //androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");

            //androidJavaObject.Call("LoginAndSend");

            // 调用QQ 登陆
            Messenger.Broadcast(Msg_Define.MSG_LOGIN);


        });


    }


    // 设置log
    public void setLog(string str)
    {
        Debug.Log("设置log");
        if (null != str && null != text)
        {
            text.text = str;
        }
        else
        {
            Debug.Log("发生错误，某个东西有空值");
        }
    }

    // 打印玩家info log 
    public void setUserInfo(string str)
    {
        userInfo.text = str;
    }



    //安卓工程那边已经把方法注释掉了，所以不需要
    //public void Add()
    //{
    //    //调用Android写好的add的方法，并传参，获取结果值
    //    int sum = androidJavaObject.Call<int>("Add", 5, 5);
    //    text.text = "从Android计算出来的值为:" + sum;
    //}


}
