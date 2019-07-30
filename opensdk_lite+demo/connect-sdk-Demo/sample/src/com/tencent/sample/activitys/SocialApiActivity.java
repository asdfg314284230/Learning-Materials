package com.tencent.sample.activitys;

import java.io.FileNotFoundException;

import com.tencent.connect.common.Constants;
import com.tencent.open.SocialApi;
import com.tencent.open.SocialConstants;
import com.tencent.sample.BaseUIListener;
import com.tencent.sample.GetAskGiftParamsDialog;
import com.tencent.sample.GetGradeParamsDialog;
import com.tencent.sample.GetInviteParamsDialog;
import com.tencent.sample.GetLbsParamsDialog;
import com.tencent.sample.GetPkBragParamsDialog;
import com.tencent.sample.GetReactiveParamsDialog;
import com.tencent.sample.GetStoryParamsDialog;
import com.tencent.sample.GetVoiceParamsDialog;
import com.tencent.sample.R;
import com.tencent.sample.GetInviteParamsDialog.OnGetInviteParamsCompleteListener;
import com.tencent.sample.GetVoiceParamsDialog.OnGetVoiceParamsCompleteListener;
import com.tencent.tauth.IUiListener;
import com.tencent.tauth.Tencent;

import android.app.Activity;
import android.content.ContentResolver;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Bundle;
import android.os.SystemClock;
import android.support.v4.util.LongSparseArray;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.LinearLayout;

public class SocialApiActivity extends BaseActivity implements OnClickListener {
	// set to 1 for test params
	private int mNeedInputParams = 1;

	//private SocialApi mSocialApi = null;
    public static final int REQUEST_VOICE_PIC = 1002;
    private Tencent mTencent;
    private LongSparseArray<IUiListener> mLiteners;
    
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setBarTitle("社交API");
		setLeftButtonEnable();
		setContentView(R.layout.social_api_activity);
		LinearLayout linearLayout = (LinearLayout) findViewById(R.id.main_container);
		for (int i = 0; i < linearLayout.getChildCount(); i++) {
			View view = linearLayout.getChildAt(i);
			if (view instanceof Button) {
				view.setOnClickListener(this);
			}
		}
		mTencent = Tencent.createInstance(MainActivity.mAppid, this);
		mLiteners = new LongSparseArray<IUiListener>(10);
	}

	@Override
	public void onClick(View v) {
		switch (v.getId()) {
		case R.id.send_story_btn:
			onClickSendStory();
			break;
		case R.id.invite_btn:
			onClickInvite();
			break;
		case R.id.ask_gift_btn:
            onClickAskGift();
            break;
        //-----------------------------------
        //下面的注释请勿删除，编译lite版的时候需要删除
        
		}

	}

	private void onClickSendStory() {
		if (mTencent.isReady()) {
		    if (mNeedInputParams == 1) {
                new GetStoryParamsDialog(this,
                        new GetStoryParamsDialog.OnGetStoryParamsCompleteListener() {

                            @Override
                            public void onGetParamsComplete(Bundle params) {
                                //mSocialApi.story(MainActivity.this, params, new BaseUiListener());
                                mTencent.story(SocialApiActivity.this, params, new BaseUIListener(SocialApiActivity.this));
                            }
                        }).show();
            } else {
                Bundle params = new Bundle();

                params.putString(SocialConstants.PARAM_TITLE,
                        "AndroidSdk_1_3:UiStory title");
                params.putString(SocialConstants.PARAM_COMMENT,
                        "AndroidSdk_1_3: UiStory comment");
                params.putString(SocialConstants.PARAM_IMAGE,
                        "http://imgcache.qq.com/qzone/space_item/pre/0/66768.gif");
                params.putString(SocialConstants.PARAM_SUMMARY,
                        "AndroidSdk_1_3: UiStory summary");
                params.putString(
                        SocialConstants.PARAM_PLAY_URL,
                        "http://player.youku.com/player.php/Type/Folder/"
                                + "Fid/15442464/Ob/1/Pt/0/sid/XMzA0NDM2NTUy/v.swf");
                params.putString(SocialConstants.PARAM_ACT, "进入应用");
                //mSocialApi.story(MainActivity.this, params, new BaseUiListener());
                mTencent.story(SocialApiActivity.this, params, new BaseUIListener(SocialApiActivity.this));
            }
		}
	}

	private void onClickInvite() {
		if (mTencent.isReady()) {
		    if (mNeedInputParams == 1) {
                new GetInviteParamsDialog(this, new OnGetInviteParamsCompleteListener() {

                    @Override
                    public void onGetParamsComplete(Bundle params) {
                        //mSocialApi.invite(MainActivity.this, params, new BaseUiListener());
                        mTencent.invite(SocialApiActivity.this, params, new BaseUIListener(SocialApiActivity.this));
                        Log.d("SDKQQAgentPref", "GetInviteFriendSwitch_SDK:" + SystemClock.elapsedRealtime());
                    }
                }).show();
            } else {
                Bundle params = new Bundle();
                // TODO keywords.
                params.putString(SocialConstants.PARAM_APP_ICON,
                        "http://imgcache.qq.com/qzone/space_item/pre/0/66768.gif");
                params.putString(SocialConstants.PARAM_APP_DESC ,
                        "AndroidSdk_2_0: voice description!");
                params.putString(SocialConstants.PARAM_ACT, "进入应用");
                
                //mSocialApi.voice(MainActivity.this, params, new BaseUiListener());
                mTencent.invite(SocialApiActivity.this, params, new BaseUIListener(SocialApiActivity.this));
            }
		}
	}

	private void onClickAskGift() {
		if (mTencent.isReady()) {
		    if (mNeedInputParams == 1) {
                new GetAskGiftParamsDialog(
                        this,
                        new GetAskGiftParamsDialog.OnGetAskGiftParamsCompleteListener() {
                            @Override
                            public void onGetParamsComplete(Bundle params) {
                                if ("request".equals(params.getString(SocialConstants.PARAM_TYPE))) {
                                    mTencent.ask(SocialApiActivity.this, params, new BaseUIListener(SocialApiActivity.this));
                                } else {
                                    mTencent.gift(SocialApiActivity.this, params, new BaseUIListener(SocialApiActivity.this));
                                }
                            }
                        }).show();
            } else {
                Bundle params = new Bundle();
                params.putString(SocialConstants.PARAM_TITLE, "title字段测试");
                params.putString(SocialConstants.PARAM_SEND_MSG, "msg字段测试");
                params.putString(SocialConstants.PARAM_IMG_URL,"http://i.gtimg.cn/qzonestyle/act/qzone_app_img/app888_888_75.png");
                mTencent.ask(this, params, new BaseUIListener(SocialApiActivity.this));
            }
		}
	}

	//-----------------------------------
    //下面的注释请勿删除，编译lite版的时候需要删除
    
    
    @Override
    protected void onDestroy() {
        super.onDestroy();
        
        for (int i = 0; i < mLiteners.size(); i++) {
        	BaseUIListener listener = (BaseUIListener) mLiteners.valueAt(i);
        	listener.cancel();
		}
    }
}
