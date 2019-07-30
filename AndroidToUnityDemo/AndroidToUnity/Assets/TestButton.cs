using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour {

    [SerializeField]
    Button button;
    // Use this for initialization

    [SerializeField]
    Text text;

	void Start () {
        button.onClick.AddListener(() => {
            CallAndroid();
        });
	}
	

    void CallAndroid()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("Callunity");

        string str = jo.Call<string>("UnityCallAndroid", "大帅哥");

        text.text = str;
    }

}
