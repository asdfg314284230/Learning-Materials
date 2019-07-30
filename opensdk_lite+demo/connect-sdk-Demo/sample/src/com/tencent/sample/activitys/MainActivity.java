package com.tencent.sample.activitys;

import java.util.ArrayList;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.os.SystemClock;
import android.text.TextUtils;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.animation.Animation;
import android.view.animation.AnimationUtils;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import com.tencent.connect.UnionInfo;
import com.tencent.connect.UserInfo;
import com.tencent.connect.common.Constants;
import com.tencent.sample.AppConstants;
import com.tencent.sample.R;
import com.tencent.sample.Util;
import com.tencent.tauth.IUiListener;
import com.tencent.tauth.UiError;
import com.tencent.tauth.Tencent;

import static android.R.attr.left;
import static android.R.attr.right;
import static com.tencent.sample.R.string.msg;

public class MainActivity extends Activity {
	private static final String TAG = MainActivity.class.getName();
    public static String mAppid;
	private Button mNewLoginButton;
//    private Button mServerSideLoginBtn;
	private TextView mUserInfo;
	private ImageView mUserLogo;
    private UserInfo mInfo;
	private EditText mEtAppid = null;
	public static Tencent mTencent;
    private static Intent mPrizeIntent = null;
    private static boolean isServerSideLogin = false;
	private CheckBox mQrCk;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		Log.d(TAG, "-->onCreate");
		// setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);//
		// 固定竖屏
		setContentView(R.layout.activity_main_new);
		initViews();
		//setBarTitle("demo菜单");
		if (TextUtils.isEmpty(mAppid)) {
			mAppid = "222222";
    		mEtAppid = new EditText(this);
    		mEtAppid.setText(mAppid);
			try {
				new AlertDialog.Builder(this).setTitle("请输入APP_ID")
						.setCancelable(false)
						.setIcon(android.R.drawable.ic_dialog_info)
						.setView(mEtAppid)
						.setPositiveButton("Commit", mAppidCommitListener)
						.setNegativeButton("Use Default", mAppidCommitListener)
						.show();
			} catch (Exception e) {
			}
		} else {
		    if (mTencent == null) {
		        mTencent = Tencent.createInstance(mAppid, this);
		    }
		}
		
        // 获取有奖分享的intent信息
        if (null != getIntent()) {
            mPrizeIntent = getIntent();
        }
    }

    /**
     * 有奖分享处理，未接入有奖分享可以不考虑
     */
    private void handlePrizeShare() {
        // -----------------------------------
        // 下面的注释请勿删除，编译lite版的时候需要删除, 注意//[不要有空格。
        
    }

    // -----------------------------------
    // 下面的注释请勿删除，编译lite版的时候需要删除, 注意//[不要有空格。
    

	@Override
	protected void onStart() {
		Log.d(TAG, "-->onStart");
		super.onStart();
	}

	@Override
	protected void onResume() {
		Log.d(TAG, "-->onResume");
        // 有奖分享处理
        handlePrizeShare();
		super.onResume();
	}

	@Override
	protected void onPause() {
		Log.d(TAG, "-->onPause");
		super.onPause();
	}

	@Override
	protected void onStop() {
		Log.d(TAG, "-->onStop");
		super.onStop();
	}

	@Override
	protected void onDestroy() {
		Log.d(TAG, "-->onDestroy");
		super.onDestroy();

	}

	private void initViews() {
		mNewLoginButton = (Button) findViewById(R.id.new_login_btn);
        mQrCk = (CheckBox) findViewById(R.id.ck_qr);
//        mServerSideLoginBtn = (Button) findViewById(R.id.server_side_login_btn);

		LinearLayout linearLayout = (LinearLayout) findViewById(R.id.main_container);
		OnClickListener listener = new NewClickListener();
		for (int i = 0; i < linearLayout.getChildCount(); i++) {
			View view = linearLayout.getChildAt(i);
			if (view instanceof Button) {
				view.setOnClickListener(listener);
			}
		}
		mNewLoginButton.setOnClickListener(listener);
		mQrCk.setOnClickListener(listener);
		mUserInfo = (TextView) findViewById(R.id.user_nickname);
		mUserLogo = (ImageView) findViewById(R.id.user_logo);
		updateLoginButton();
	}

	private void updateLoginButton() {
		if (mTencent != null && mTencent.isSessionValid()) {
            if (isServerSideLogin) {
                mNewLoginButton.setTextColor(Color.BLUE);
                mNewLoginButton.setText("登录");
//                mServerSideLoginBtn.setTextColor(Color.RED);
//                mServerSideLoginBtn.setText("退出Server-Side账号");
            } else {
                mNewLoginButton.setTextColor(Color.RED);
                mNewLoginButton.setText("退出帐号");
//                mServerSideLoginBtn.setTextColor(Color.BLUE);
//                mServerSideLoginBtn.setText("Server-Side登陆");
            }
		} else {
			mNewLoginButton.setTextColor(Color.BLUE);
			mNewLoginButton.setText("登录");
//            mServerSideLoginBtn.setTextColor(Color.BLUE);
//            mServerSideLoginBtn.setText("Server-Side登陆");
		}
	}

	private void updateUserInfo() {
		if (mTencent != null && mTencent.isSessionValid()) {
			IUiListener listener = new IUiListener() {

				@Override
				public void onError(UiError e) {

				}

				@Override
				public void onComplete(final Object response) {
					Message msg = new Message();
					msg.obj = response;
					msg.what = 0;
					mHandler.sendMessage(msg);
					new Thread(){

						@Override
						public void run() {
							JSONObject json = (JSONObject)response;
							if(json.has("figureurl")){
								Bitmap bitmap = null;
								try {
									bitmap = Util.getbitmap(json.getString("figureurl_qq_2"));
								} catch (JSONException e) {

								}
								Message msg = new Message();
								msg.obj = bitmap;
								msg.what = 1;
								mHandler.sendMessage(msg);
							}
						}

					}.start();
				}

				@Override
				public void onCancel() {

				}
			};
			mInfo = new UserInfo(this, mTencent.getQQToken());
			mInfo.getUserInfo(listener);

		} else {
			mUserInfo.setText("");
			mUserInfo.setVisibility(android.view.View.GONE);
			mUserLogo.setVisibility(android.view.View.GONE);
		}
	}

	private void getUnionId() {
		if (mTencent != null && mTencent.isSessionValid()) {
			IUiListener listener = new IUiListener() {
				@Override
				public void onError(UiError e) {
					Toast.makeText(MainActivity.this,"onError",Toast.LENGTH_LONG).show();
				}

				@Override
				public void onComplete(final Object response) {
					if(response != null){
						JSONObject jsonObject = (JSONObject)response;
						try {
							String unionid = jsonObject.getString("unionid");
							Util.showResultDialog(MainActivity.this, "unionid:\n"+unionid, "onComplete");
							Util.dismissDialog();
						}catch (Exception e){
							Toast.makeText(MainActivity.this,"no unionid",Toast.LENGTH_LONG).show();
						}
					}else {
						Toast.makeText(MainActivity.this,"no unionid",Toast.LENGTH_LONG).show();
					}
				}

				@Override
				public void onCancel() {
					Toast.makeText(MainActivity.this,"onCancel",Toast.LENGTH_LONG).show();
				}
			};
			UnionInfo unionInfo = new UnionInfo(this, mTencent.getQQToken());
			unionInfo.getUnionId(listener);
		} else {
			Toast.makeText(this,"please login frist!",Toast.LENGTH_LONG).show();
		}
	}

	Handler mHandler = new Handler() {

		@Override
		public void handleMessage(Message msg) {
			if (msg.what == 0) {
				JSONObject response = (JSONObject) msg.obj;
				if (response.has("nickname")) {
					try {
						mUserInfo.setVisibility(android.view.View.VISIBLE);
						mUserInfo.setText(response.getString("nickname"));
					} catch (JSONException e) {
						e.printStackTrace();
					}
				}
			}else if(msg.what == 1){
				Bitmap bitmap = (Bitmap)msg.obj;
				mUserLogo.setImageBitmap(bitmap);
				mUserLogo.setVisibility(android.view.View.VISIBLE);
			}
		}

	};

	private void onClickLogin() {
		if (!mTencent.isSessionValid()) {
			if(!mQrCk.isChecked()){
				mTencent.login(this, "all", loginListener);
			}else {
				mTencent.login(this, "all", loginListener,true);
			}
            isServerSideLogin = false;
			Log.d("SDKQQAgentPref", "FirstLaunch_SDK:" + SystemClock.elapsedRealtime());
		} else {
            if (isServerSideLogin) { // Server-Side 模式的登陆, 先退出，再进行SSO登陆
                mTencent.logout(this);
                mTencent.login(this, "all", loginListener);
                isServerSideLogin = false;
                Log.d("SDKQQAgentPref", "FirstLaunch_SDK:" + SystemClock.elapsedRealtime());
                return;
            }
		    mTencent.logout(this);
			updateUserInfo();
			updateLoginButton();
		}
	}

//    private void onClickServerSideLogin() {
//        if (!mTencent.isSessionValid()) {
//            mTencent.loginServerSide(this, "all", loginListener);
//            isServerSideLogin = true;
//            Log.d("SDKQQAgentPref", "FirstLaunch_SDK:" + SystemClock.elapsedRealtime());
//        } else {
//            if (!isServerSideLogin) { // SSO模式的登陆，先退出，再进行Server-Side模式登陆
//                mTencent.logout(this);
//                mTencent.loginServerSide(this, "all", loginListener);
//                isServerSideLogin = true;
//                Log.d("SDKQQAgentPref", "FirstLaunch_SDK:" + SystemClock.elapsedRealtime());
//                return;
//            }
//            mTencent.logout(this);
//            isServerSideLogin = false;
//            updateUserInfo();
//            updateLoginButton();
//        }
//    }

	public static String getAppid() {
		if (TextUtils.isEmpty(mAppid)) {
			mAppid = "222222";
		}

		return mAppid;
	}

	public static boolean ready(Context context) {
		if (mTencent == null) {
			return false;
		}
		boolean ready = mTencent.isSessionValid()
				&& mTencent.getQQToken().getOpenId() != null;
		if (!ready) {
            Toast.makeText(context, "login and get openId first, please!",
					Toast.LENGTH_SHORT).show();
        }
		return ready;
	}

	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
	    Log.d(TAG, "-->onActivityResult " + requestCode  + " resultCode=" + resultCode);
	    if (requestCode == Constants.REQUEST_LOGIN ||
	    	requestCode == Constants.REQUEST_APPBAR) {
	    	Tencent.onActivityResultData(requestCode,resultCode,data,loginListener);
	    }

	    super.onActivityResult(requestCode, resultCode, data);
	}

	public static void initOpenidAndToken(JSONObject jsonObject) {
        try {
            String token = jsonObject.getString(Constants.PARAM_ACCESS_TOKEN);
            String expires = jsonObject.getString(Constants.PARAM_EXPIRES_IN);
            String openId = jsonObject.getString(Constants.PARAM_OPEN_ID);
            if (!TextUtils.isEmpty(token) && !TextUtils.isEmpty(expires)
                    && !TextUtils.isEmpty(openId)) {
                mTencent.setAccessToken(token, expires);
                mTencent.setOpenId(openId);
            }
        } catch(Exception e) {
        }
    }

	IUiListener loginListener = new BaseUiListener() {
        @Override
        protected void doComplete(JSONObject values) {
        	Log.d("SDKQQAgentPref", "AuthorSwitch_SDK:" + SystemClock.elapsedRealtime());
            initOpenidAndToken(values);
            updateUserInfo();
            updateLoginButton();
        }
    };

	private class BaseUiListener implements IUiListener {

		@Override
		public void onComplete(Object response) {
            if (null == response) {
                Util.showResultDialog(MainActivity.this, "返回为空", "登录失败");
                return;
            }
            JSONObject jsonResponse = (JSONObject) response;
            if (null != jsonResponse && jsonResponse.length() == 0) {
                Util.showResultDialog(MainActivity.this, "返回为空", "登录失败");
                return;
            }
			Util.showResultDialog(MainActivity.this, response.toString(), "登录成功");
            // 有奖分享处理
            handlePrizeShare();
			doComplete((JSONObject)response);
		}

		protected void doComplete(JSONObject values) {

		}

		@Override
		public void onError(UiError e) {
			Util.toastMessage(MainActivity.this, "onError: " + e.errorDetail);
			Util.dismissDialog();
		}

		@Override
		public void onCancel() {
			Util.toastMessage(MainActivity.this, "onCancel: ");
			Util.dismissDialog();
            if (isServerSideLogin) {
                isServerSideLogin = false;
            }
		}
	}

	private DialogInterface.OnClickListener mAppidCommitListener = new DialogInterface.OnClickListener() {

		@Override
		public void onClick(DialogInterface dialog, int which) {
			mAppid = AppConstants.APP_ID;
			switch (which) {
			case DialogInterface.BUTTON_POSITIVE:
				// 用输入的appid
				String editTextContent = mEtAppid.getText().toString().trim();
				if (!TextUtils.isEmpty(editTextContent)) {
				    mAppid = editTextContent;
				}
				break;
			case DialogInterface.BUTTON_NEGATIVE:
				// 默认appid
				break;
			}
			mTencent = Tencent.createInstance(mAppid, MainActivity.this);
            // 有奖分享处理
            handlePrizeShare();
		}
	};

    private void onClickIsSupportSSOLogin() {
        if (mTencent.isSupportSSOLogin(MainActivity.this)) {
            Toast.makeText(MainActivity.this, "支持SSO登陆", Toast.LENGTH_SHORT).show();
        } else {
            Toast.makeText(MainActivity.this, "不支持SSO登陆", Toast.LENGTH_SHORT).show();
        }
    }

	class NewClickListener implements OnClickListener {
		@Override
		public void onClick(View v) {
			Context context = v.getContext();
			Animation shake = AnimationUtils.loadAnimation(context,
					R.anim.shake);
			Class<?> cls = null;
			boolean isAppbar = false;
			switch (v.getId()) {
			case R.id.new_login_btn:
				onClickLogin();
				v.startAnimation(shake);
				return;
			case R.id.ck_qr:
				if(mQrCk.isChecked()){
					Toast.makeText(MainActivity.this,"没有装手q时支持二维码登录，一般用于电视等设备",Toast.LENGTH_LONG).show();
				}
				return;
//			case R.id.server_side_login_btn:
//			    onClickServerSideLogin();
//			    v.startAnimation(shake);
//			    return;
			case R.id.main_sso_btn:
                    onClickIsSupportSSOLogin();
			    return;
			case R.id.main_getInfo_btn:
				cls = AccountInfoActivity.class;
				break;
			case R.id.app_get_unionid:
				getUnionId();
				break;
			case R.id.main_qqShare_btn:
				cls = QQShareActivity.class;
				break;
			case R.id.main_qzoneShare_btn:
				cls = QZoneShareActivity.class;
				break;
			/*case R.id.main_social_api_btn:
                cls = SocialApiActivity.class;
                break;*/
			case R.id.main_is_qq_installed_btn:
				Toast.makeText(MainActivity.this, mTencent.isQQInstalled(MainActivity.this) + "" , Toast.LENGTH_SHORT).show();
				break;
			case R.id.check_token_valid:
				JSONObject jsonObject = null;
				boolean isValid = mTencent.checkSessionValid(mAppid);
				if(!isValid) {
					Util.showResultDialog(MainActivity.this, "token过期，请调用登录接口拉起手Q授权登录", "登录失败");
					return;
				} else {
					jsonObject = mTencent.loadSession(mAppid);
					mTencent.initSessionCache(jsonObject);
				}
				Util.showResultDialog(MainActivity.this, jsonObject.toString(), "登录成功");
				updateUserInfo();
				updateLoginButton();
				break;
			case R.id.game_add_friend:
				cls = GameLogicActivity.class;
				break;
			case R.id.main_qqgroup_btn:
				cls = QQGroupActivity.class;
				break;
				//-----------------------------------
				// 下面的注释请勿删除，编译lite版的时候需要删除, 注意//[不要有空格。
				
			}
			v.startAnimation(shake);
			if (cls != null) {
				Intent intent = new Intent(context, cls);
				if (isAppbar) { //APP内应用吧登录需接收登录结果
					startActivityForResult(intent, Constants.REQUEST_APPBAR);
				} else {
					context.startActivity(intent);
				}
			}
		}
	}
}
