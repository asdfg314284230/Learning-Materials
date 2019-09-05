@echo off

REM -----------------------------------------------------------------
REM -----------------------------------------------------------------

REM chcp 65001


REM -----------------------------------------------------------------
REM 配置开始

REM 想要出资源的平台
set TYPE=android
REM unity路径
set UNITY_PATH="C:\Program Files\Unity\Editor\Unity.exe"
:: 要出包的
set SPROJET_ROOT=%~dp0sproject_%TYPE%
:: 配置文件
set templet=%~dp0android_templet
:: 拉取分支
set BRANCH=dev
:: android工程目录
set ANDROID_PROJECT_DIR=D:\sproject\sproject_android\sProject\android
::出完APK路径
set SPROJECT_RELEASE=%ANDROID_PROJECT_DIR%\sProject\build\outputs\apk\release\sProject-release.apk
:: 工具路径
set SPROJECT_TOOLS=D:\sproject\sproject_android\Tools_Post\Tools_post\Tools_post\bin\Debug\Tools_post.exe

REM //bundle_id : 包名
set bundle_id="com.hsn.sProject"
REM //api_token : api钩子
set api_token="cd5d4fc7ce6c1703e525faaa6a59d044"
REM //path :apk文件路径
set path_apk=%SPROJECT_RELEASE%
REM //name : 名称
set name="sProject"
REM //version ： 版本
set version="1"
REM //build : 包体版本
set build="1"
REM //access_token :钉钉机器人钩子
set access_token="5ec1754df54d0f9e708ad9c4a5b77a002f364191437c66a79fe596dc9d8619ab"

set test="Android Sproject!!"
REM //messageUrl : 包体URL
set messageUrl="https://fir.im/v3zd"


REM 配置结束
REM -----------------------------------------------------------------

REM (更新工程)
echo update project android

set PROJECT_ROOT=%~dp0sproject_android

if not exist %PROJECT_ROOT% (
    echo creating dir %PROJECT_ROOT%
    md %PROJECT_ROOT%
    cd %PROJECT_ROOT%
    echo cloning project...
    git clone https://git.dev.tencent.com/AS_M9/sProject.git -b %BRANCH% --recursive
)

cd %PROJECT_ROOT%\sProject\Assets\ThinkUnityFwk
git clean -df
git reset --hard

cd %PROJECT_ROOT%
git clean -df
git reset --hard

git pull
git submodule update


echo explor project android ...
:: 导出安卓工程

%UNITY_PATH% -quit -batchmode -nographics -logFile %SPROJET_ROOT%\Editor.log -projectPath %SPROJET_ROOT%\sProject -executeMethod SYFwk.Tool.AutoPackge.BuildAndroid

:: copy build.gradle  copy AndroidManifest.xml
echo copy config ...

copy %templet%\build.gradle %ANDROID_PROJECT_DIR%\sProject\

copy %templet%\AndroidManifest.xml %ANDROID_PROJECT_DIR%\sProject\src\main\AndroidManifest.xml
REM 这里需要注意\hsn\xxxxxx\需要看着工程名称调
copy %templet%\SYActivity.java %ANDROID_PROJECT_DIR%\sProject\src\main\java\com\hsn\sProjectct\SYActivity.java

cd %PROJECT_ROOT%/..

::打包apk
call export_android.bat


echo push Ok




REM 参数  
REM 需要的参数
REM bundle_id 包名
REM api_token api钩子
REM pathapk 文件路径
REM name 名称
REM version 版本
REM build 包体版本
REM access_token 钉钉机器人钩子

REM 调用Post 请求
%SPROJECT_TOOLS% %bundle_id% %api_token% %path_apk% %name% %version% %build% %access_token% %test% %messageUrl%





:LABEL_FINISH
echo finish...

pause