package com.example.sdk_project;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.TextUtils;

import com.tencent.connect.common.Constants;
import com.tencent.tauth.IUiListener;
import com.tencent.tauth.Tencent;
import com.tencent.tauth.UiError;
import com.unity3d.player.UnityPlayerActivity;

import org.json.JSONObject;

// 提供给Unity的话需要继承Unity的Class
public class MainActivity extends UnityPlayerActivity {

    public static Tencent mTencent;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
//        setContentView(R.layout.activity_main);
    }


    //登录方法
    public void LoginAndSend()
    {//定义一个对象，里面的第一个参数是自己在QQ开放平台上申请的APPID。
        mTencent = Tencent.createInstance("1109218679",this.getApplicationContext());
        if (!mTencent.isSessionValid())
        {
            mTencent.login(this, "all", loginListener);
        }
    }

    /** QQ登录第二步：存储token和openid */
    public static void initOpenidAndToken(JSONObject jsonObject) {
        try {
            String token = jsonObject.getString(Constants.PARAM_ACCESS_TOKEN);
            String expires = jsonObject.getString(Constants.PARAM_EXPIRES_IN);
            String openId = jsonObject.getString(Constants.PARAM_OPEN_ID);
            // 做判空，存储相对应的数据
            if (!TextUtils.isEmpty(token) && !TextUtils.isEmpty(expires)
                    && !TextUtils.isEmpty(openId)) {
                mTencent.setAccessToken(token, expires);
                mTencent.setOpenId(openId);
            }
        } catch(Exception e) {
        }
    }

    // 获取用户信息返回给Unity
    public void updataUserInfo(){

        // 判空处理
        if(mTencent != null && mTencent.isSessionValid()){
            // 回调注册
            IUiListener infoListener = new BaseUiListener(){

                public void onComplete(Object response){
                    if(response != null){
                        // 发送给Unity 得到QQ玩家数据

                    }
                }

            };
        }
    }


    // 回调实例回调调用解析函数
    IUiListener loginListener = new BaseUiListener() {
        @Override
        protected void doComplete(JSONObject values) {
            initOpenidAndToken(values);
            updataUserInfo();
        }
    };


    @Override  //这段代码非常重要，不加的话无法获取回调
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == Constants.REQUEST_LOGIN ||
                requestCode == Constants.REQUEST_APPBAR) {

            Tencent.onActivityResultData(requestCode,resultCode,data,loginListener);
        }
        super.onActivityResult(requestCode, resultCode, data);
    }


    //重写SDK中的抽象类，用于登陆回调获取玩家信息
    private class BaseUiListener implements IUiListener {

        @Override
        public void onComplete(Object response) {
            if (null == response) {

                return;
            }
            JSONObject jsonResponse = (JSONObject) response;
            if (null != jsonResponse && jsonResponse.length() == 0) {

                return;
            }

            doComplete((JSONObject) response);
        }

        protected void doComplete(JSONObject values) {

        }

        @Override
        public void onError(UiError e) {

        }

        @Override
        public void onCancel() {

        }
    }

}
