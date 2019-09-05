# git --version
basepath=$(cd `dirname $0`; pwd)
# echo $basepath

# ------------------------------
# 配置参数开始
branch=dev
name=test


PACK_NAME="com.hsn.sproject"
PACK_DISPLAY_NAME="sProject"
PACK_BundleVersion=1


MAC_PACK="${basepath}/mac_pack"
archivePath="${MAC_PACK}/archive/Unity-iPhone.xcarchive"
IPA="${basepath}/ipa/"

# codeSignIdentity="iPhone Developer: fan zhang (SHXHA9LKD7)"
codeSignIdentity="iPhone Developer"
provisioningProfileSpecifier="sproject_dev"
exportOptionsPlist="${MAC_PACK}/plist/ExportOptions.plist"
TEAM_ID="KH89LAUFYJ"

project_root=$basepath/sProject
project_dir=$project_root/$branch

#选择复制文件的文件夹名称
file=dev

# 存放ios工程的根目录
ios_root=$basepath/ios
# ios工程目录
ios_project_dir=$project_root/${file}/sProject/sProject/ios

# ios xocode 目录
project="${ios_project_dir}/Unity-iPhone.xcodeproj"

configuration="Release"
scheme="Unity-iPhone"

# 配置参数结束
# ------------------------------


# 还没有拉取分支
# if [ ! -d $project_dir ];then
#     echo $project_dir not exist
#     exit
# fi

# 更新工程
cd $project_dir/sProject
git clean -df
git reset --hard

cd $project_dir/sProject/sProject/Assets/ThinkUnityFwk
git clean -df
git reset --hard

git pull
git submodule update


if [ ! -d $ios_root ];then
    echo making dir $ios_root
    mkdir $ios_root
fi

# # 删除原有的工程目录
# if [ -d $ios_project_dir ];then
#     rm -r $ios_project_dir
# fi
# mkdir $ios_project_dir

#复制图标文件
# icon_root=$basepath/other/property/${file}"/icon/"
# taget_icon=$project_dir/sProject/sProject/Assets/game_icon/
# rm -f $taget_icon*
# cp -f $icon_root* $taget_icon


UNITY_PATH="/Applications/Unity2017/Unity.app/Contents/MacOS/Unity"

# unity项目路径
unity_project_path="${project_dir}/sProject/sProject"


$UNITY_PATH -quit -batchmode -nographics -logFile "${project_dir}/Editor.log" -projectPath $unity_project_path -executeMethod SYFwk.Tool.AutoPackge.BuildIOS


# 拷贝zip
mkdir -m 777 ${ios_project_dir}/Data/Raw/zip_ab
cp -f $basepath/other/zip_ab/* ${ios_project_dir}/Data/Raw/zip_ab/


# 拷贝配置文件
target_info=${ios_project_dir}/
source=$basepath/other/property/${file}
target_param=${ios_project_dir}/Data/Raw/

Project_Plist=$source/Info.plist

# 将文件的plist 的build版本号加一，并设置到plist文件中
/usr/libexec/PlistBuddy -c "Set :CFBundleVersion ${PACK_BundleVersion}" ${Project_Plist}
/usr/libexec/PlistBuddy -c "Set :CFBundleDisplayName ${PACK_DISPLAY_NAME}" ${Project_Plist}
/usr/libexec/PlistBuddy -c "Set :CFBundleIdentifier ${PACK_NAME}" ${Project_Plist}

cp -f $source/Param_config.json  $target_param
cp -f $source/Info.plist  $target_info

echo $target_param
echo $source/Param_config.json

# cd ${target_info}

# xcodebuild clean -configuration Release -alltargets
# # # xcodebuild archive -project "$project" -scheme "$scheme" -configuration "$configuration" -archivePath "$archivePath" CODE_SIGN_IDENTITY="$codeSignIdentity" PROVISIONING_PROFILE="$provisioningProfile" -arch arm64
xcodebuild archive -project "$project" -scheme "$scheme" -configuration "$configuration" -archivePath "$archivePath" CODE_SIGN_IDENTITY="$codeSignIdentity" DEVELOPMENT_TEAM="$TEAM_ID" PROVISIONING_PROFILE_SPECIFIER="$provisioningProfileSpecifier" -arch arm64
xcodebuild -exportArchive -archivePath "$archivePath" -exportOptionsPlist "$exportOptionsPlist" -exportPath "${IPA}"

echo "发布成功"


/usr/local/bin/fir publish ${IPA}/Unity-iPhone.ipa -T=cd5d4fc7ce6c1703e525faaa6a59d044
curl -X POST \
  'https://oapi.dingtalk.com/robot/send?access_token=5ec1754df54d0f9e708ad9c4a5b77a002f364191437c66a79fe596dc9d8619ab' \
  -H 'cache-control: no-cache' \
  -H 'content-type: application/json' \
  -d '{
    "msgtype": "link", 
    "link": {
        "text":"IOS最新测试版本Sproject!!", 
       "title": "点击这个连接下载", 
        "messageUrl": "http://fir.im/kym9"
    }
}'

echo "finish"

