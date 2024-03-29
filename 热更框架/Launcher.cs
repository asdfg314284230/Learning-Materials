﻿using BestHTTP;
using Fwk.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviour {
    /*
     * 目录结构：
     * persistentDataPath/ver_data         ab根目录
     * persistentDataPath/ver_data/ab      zip_ab解压目录
     * persistentDataPath/ver_data/zip_ab  下载的ab压缩文件目录
     * persistentDataPath/lua_zip          下载的lua压缩文件目录
     * persistentDataPath/lua              lua_zip解压目录
     */
    // 路径定义
    string src         = "";
    string dst         = "";
    string dst_ab      = "";
    string dst_ab_zip  = "";
    string dst_lua     = "";
    string dst_lua_zip = "";

    // 在线参数
    string ver              = "";
    string cur_config_ver   = "";
    string cur_geme_ver     = "";
    string cur_flz_ver      = "";
    string game_id          = "";
    bool ignore_down_verify = false;
    string update_url       = "";
    int is_discard          = 0;
    string oss_path         = "";

    // 根据本地保存的资源版本和包体中配置的资源版本决定是否拷贝解压包体中的资源到本地
    // 保存在本地的资源版本
    string local_ab_pack_ver = "";
    // 包体中的资源版本
    string ab_pack_ver       = "";

    // 启动UI界面
    public LauncherUI launcherUI;
    // 对话框点击延时执行秒数
    private float delaySecond = 1;

    // URL
    private string onlineParamUrl = "http://xproject.racethunder.cn/online_param";
    private string serverListUrl = "";

    // oss上划分的平台
    private string platformWin     = "win";
    private string platformAndroid = "android";
    private string platformIos     = "ios";

    // Text
    // 没有网络对话框标题
    private string titleTextNetworkError   = "网络错误";
    // 没有网络对话框提示
    private string contentTextNetworkError = "请检查网络设置";
    // http请求错误标题
    private string titleTextHttpError      = "请求错误";
    // http请求错误提示
    private string contentTextHttpError    = "请求错误，请重试";
    // 换包更新对话框标题
    private string titleTextUpdatePackage  = "版本更新";
    // 换包更新对话框提示
    private string contentTextUpdatePackage= "发现新版本，前往下载";

    // 提示：无提示
    private string tipTextNull             = "";
    // 提示：正在请求在线参数
    private string tipTextOnlineParam      = "正在请求在线参数";
    // 提示：正在解压资源
    private string tipTextUnzipAb          = "正在解压资源，此过程不消耗任何流量";
    // 提示：正在拷贝资源
    private string tipTextCopy             = "正在拷贝资源，此过程不消耗任何流量";
    // 提示：正在请求md5文件
    private string tipTextMd5File          = "正在请求资源验证文件";
    // 提示：正在验证资源
    private string tipTextCheckMd5         = "正在检测资源更新";
    // 提示：正在更新资源
    private string tipTextUpdateAb         = "正在更新资源";
    // 提示：正在验证代码
    private string tipTextCheckLua         = "正在检测代码更新";
    // 提示：正在更新代码
    private string tipTextUpdateLua        = "正在更新代码";
    // 提示：正在解压代码
    private string tipTextUnzipLua         = "正在解压代码";
    // 提示：正在获取服务器列表
    private string tipTextServerList       = "正在获取服务器列表";

    IEnumerator Start () {
        src = Application.streamingAssetsPath;
        dst = Application.persistentDataPath + "/ver_data";
        dst_ab = Application.persistentDataPath + "/ver_data/ab";
        dst_ab_zip = Application.persistentDataPath + "/ver_data/zip_ab";
        dst_lua = Application.persistentDataPath + "/lua";
        dst_lua_zip = Application.persistentDataPath + "/lua_zip";

        // 设置配置路径文件
        string path = "file:///" + src + "/Param_config.json";
        if (Application.platform == RuntimePlatform.Android)
        {
            path = src + "/Param_config.json";
        }
        WWW www = new WWW(path);
        yield return www;

        // 解析本地配置
        bool success = true;
        // 缓存本地配置
        Param_config.Param_config_json = www.text.Trim();
        // 解析本地配置
        Dictionary<string, object> localParam = (Dictionary<string, object>)BestHTTP.JSON.Json.Decode(www.text.Trim(), ref success);
        foreach (var item in localParam)
        {
            // Debug.LogFormat("key = {0}, value = {1}", item.Key, item.Value);
            if (item.Key == "Umeng_key")
            {
                Param_config.Umeng_key = Convert.ToString(item.Value);
            }
            if (item.Key == "Umeng_channel")
            {
                Param_config.Umeng_channel = Convert.ToString(item.Value);
            }
            if (item.Key == "pack_ver")
            {
                Param_config.pack_ver = Convert.ToString(item.Value);
            }
            if (item.Key == "online_param")
            {
                Param_config.online_param = Convert.ToString(item.Value);
            }
            if (item.Key == "bugly_appid")
            {
                Param_config.bugly_appid = Convert.ToString(item.Value);
            }
            if (item.Key == "ab_pack_ver")
            {
                ab_pack_ver = Convert.ToString(item.Value);
                Param_config.ab_pack_ver = Convert.ToString(item.Value);
            }
        }
        // 请求在线参数
        request_online_param();
    }

    // 请求在线参数
    void request_online_param()
    {
        string url = onlineParamUrl + "/" + Param_config.online_param;
        Uri uri = new Uri(url);
        
        HTTPRequest request = new HTTPRequest(uri, on_request_online_param);
        request.DisableCache = true;
        // 请求http协议
        request.Send();
        // 告知显示
        launcherUI.setTip(tipTextOnlineParam);
    }

    // 请求完成回调
    private void on_request_online_param(HTTPRequest originalRequest, HTTPResponse response)
    {
        launcherUI.setTip(tipTextNull);

        if (originalRequest == null || response == null)
        {
            // 没有网络的异常处理
            launcherUI.showDialog(titleTextNetworkError, contentTextNetworkError, request_online_param, null, delaySecond);
            Debug.Log("no network");
            return;
        }

        Debug.Log("http_finish originalRequest.State = " + originalRequest.State + " response.IsSuccess = " + response.IsSuccess);

        if (originalRequest.State == HTTPRequestStates.Finished && response.IsSuccess)
        {
            // 赋值,获取在线参数配置
            Launcher_param.online_param = response.DataAsText;
            try
            {
                bool success = true;
                Dictionary<string, object> onlineParam = (Dictionary<string, object>)BestHTTP.JSON.Json.Decode(Launcher_param.online_param, ref success);
                // 异常 重新请求在线参数
                if (!success || onlineParam == null)
                {
                    // 请求在线参数
                    request_online_param();
                    return;
                }
                // 遍历赋值
                foreach (var item in onlineParam)
                {
                    // Debug.LogFormat("key = {0}, value = {1}", item.Key, item.Value);
                    // 热更资源版本
                    if (item.Key == "ver")
                    {
                        ver = Convert.ToString(item.Value);
                    }
                    // 配置版本
                    if (item.Key == "config_ver")
                    {
                        cur_config_ver = Convert.ToString(item.Value);
                    }
                    // 代码版本
                    if (item.Key == "geme_ver")
                    {
                        cur_geme_ver = Convert.ToString(item.Value);
                    }
                    // 框架版本
                    if (item.Key == "flz_ver")
                    {
                        cur_flz_ver = Convert.ToString(item.Value);
                    }
                    // game id
                    if (item.Key == "game_id")
                    {
                        game_id = Convert.ToString(item.Value);
                    }
                    // 整包更新地址 内部测试：ios https://fir.im/cdrg android https://fir.im/uk28
                    if (item.Key == "update_url")
                    {
                        update_url = Convert.ToString(item.Value);
                    }
                    if (item.Key == "is_discard")
                    {
                        is_discard = Convert.ToInt32(item.Value);
                    }
                    // oss地址
                    if (item.Key == "oss_path")
                    {
                        oss_path = Convert.ToString(item.Value);
                        Launcher_param.oss_path = oss_path;
                    }
                    // 是否忽略下载验证
                    if (item.Key == "ignore_down_verify")
                    {
                        ignore_down_verify = Convert.ToInt32(item.Value) == 1;
                        Launcher_param.ignore_down_verify = ignore_down_verify;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            // 判断当前平台
            if (!isSupportPlatform())
            {
                Debug.Log("launcher is not supporting this platform");
                return;
            }

            // 是否换包
            if (is_discard == 1)
            {
                // 告知UI显示,打开URL，让玩家重新下载包体
                launcherUI.showDialog(titleTextUpdatePackage, contentTextUpdatePackage, update_package, null, 0);
                return;
            }

            // 获取本地资源版本号
            local_ab_pack_ver = PlayerPrefs.GetString("local_ab_pack_ver", "0");

            // 判断本地版本号跟在线参数版本号是否不同
            if (!local_ab_pack_ver.Equals(ab_pack_ver))
            {
                // 解压文件
                StartCoroutine(Unpack());
            }
            else
            {
                // 校验MD5
                request_md5_file();
            }
        }
        else
        {
            launcherUI.showDialog(titleTextHttpError, contentTextHttpError, request_online_param, null, delaySecond);
            Debug.Log("http error");
            return;
        }
    }

    // 解压文件
    IEnumerator Unpack()
    {
        launcherUI.setTip(tipTextUnzipAb);

        Directory.CreateDirectory(dst);
        Directory.CreateDirectory(dst_ab);
        Directory.CreateDirectory(dst_ab_zip);

        /*
            放入streamingAssets中的资源：
            zip_ab 存放ab的文件夹
            zip_list.json
            android资源没法遍历
            android需要先拷贝，再解压
        */
        if (Application.platform == RuntimePlatform.Android)
        {
            // 读取资源列表
            string path = src + "/zip_list.json";
            WWW www = new WWW(path);
            yield return www;

            // 拷贝ab压缩资源到手机
            bool success = true;
            Dictionary<string, object> resList = (Dictionary<string, object>)BestHTTP.JSON.Json.Decode(www.text.Trim(), ref success);
            int index = 0;
            int total = resList.Count;
            foreach (var item in resList)
            {
                index++;
                launcherUI.setProgress(total, index);
                WWW www_zip = new WWW(src + "/zip_ab/" + item.Key + ".zip");
                yield return www_zip;

                if (www_zip.isDone)
                {
                    File.WriteAllBytes(dst_ab_zip + "/" + item.Key + ".zip", www_zip.bytes);
                }
            }

            launcherUI.setTip(tipTextCopy);

            // 解压资源
            DirectoryInfo dir = new DirectoryInfo(dst_ab_zip);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
            index = 0;
            total = fileinfo.Length;
            foreach (FileSystemInfo i in fileinfo)
            {
                string aFirstName = i.Name.Substring(0, i.Name.LastIndexOf(".")); //文件名
                ZipUtility.UnzipFile(i.FullName, dst_ab + "/");
                index++;
                launcherUI.setProgress(total, index);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Path.Combine(src, "zip_ab/");
            DirectoryInfo dir = new DirectoryInfo(path);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
            int index = 0;
            int total = fileinfo.Length;
            foreach (FileSystemInfo i in fileinfo)
            {
                string aFirstName = i.Name.Substring(0, i.Name.LastIndexOf(".")); //文件名
                ZipUtility.UnzipFile(i.FullName, dst_ab + "/");
                index++;
                launcherUI.setProgress(total, index);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            Debug.Log("Unpack is not supporting this platform");
        }
        
        yield return new WaitForSeconds(0.01f);

        PlayerPrefs.SetString("local_ab_pack_ver", ab_pack_ver);
        // 删除拷贝到手机的压缩文件
        if (Directory.Exists(dst_ab_zip))
        {
            Directory.Delete(dst_ab_zip, true);
        }
        request_md5_file();
    }

    void request_md5_file()
    {
        //获取OSS路径
        string url = oss_path + "ver/" + getOSSPlatformString() + "/ver_" + ver + "/res_list.json";
        
        Uri uri = new Uri(url);
        Debug.Log(url);
        // 开启一个HTTP协议，完成时走回调
        HTTPRequest request = new HTTPRequest(uri, on_resquest_md5_file);
        request.DisableCache = true;
        request.Send();

        launcherUI.setTip(tipTextMd5File);
    }

    void on_resquest_md5_file(HTTPRequest originalRequest, HTTPResponse response)
    {
        launcherUI.setTip(tipTextNull);

        // 没有网络
        if (originalRequest == null || response == null)
        {
            launcherUI.showDialog(titleTextNetworkError, contentTextNetworkError, request_md5_file, null, delaySecond);
            Debug.Log("no network");
            return;
        }

        Debug.Log("http_finish originalRequest.State = " + originalRequest.State + " response.IsSuccess = " + response.IsSuccess);


        if (originalRequest.State == HTTPRequestStates.Finished && response.IsSuccess)
        {
            // 获取资源列表
            Launcher_param.res_list = response.DataAsText;
            // 下载AB
            check_ab_update();
        }
        else
        {
            // HTTP协议报错
            launcherUI.showDialog(titleTextHttpError, contentTextHttpError, request_md5_file, null, delaySecond);
            Debug.Log("http error");
            return;
        }
    }

    // 检查AB是否需要更新
    void check_ab_update()
    {
        // 告知UI层
        launcherUI.setTip(tipTextCheckMd5);

        List<string> down_list = new List<string>();
        Dictionary<string, string> local_res_md5 = new Dictionary<string, string>();
        DirectoryInfo dir_info = new DirectoryInfo(dst_ab);
        FileSystemInfo[] fileinfo = dir_info.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
        // 遍历所有目录下的子文件
        foreach (FileSystemInfo i in fileinfo)
        {
            string md5 = SYFwk.Core.Extension.EncodeMd5File(i.FullName, true);
            local_res_md5.Add(i.Name, md5);
        }

        bool success = true;
        Dictionary<string, object> resList = (Dictionary<string, object>)BestHTTP.JSON.Json.Decode(Launcher_param.res_list, ref success);
        // 遍历获取到的资源文件列表
        foreach (var item in (Dictionary<string, object>)resList["res_list"])
        {
            bool add = true;
            bool contain = local_res_md5.ContainsKey(item.Key);
            if (contain)
            {
                bool same = local_res_md5[item.Key] == item.Value.ToString();
                if (same)
                {
                    add = false;
                }
            }

            // 添加需要下载的资源的Key
            if (add)
            {
                down_list.Add(item.Key);
            }
        }

        // 下载资源
        download_ab(down_list);
    }

    // 下载资源
    void download_ab(List<string> down_list, int down_index = 0)
    {
        launcherUI.setTip(tipTextUpdateAb);

        // 下载数量大于或资源数量
        if (down_index >= down_list.Count)
        {
            // 下载完成
            Debug.Log("download_ab finish!");
            // 检查lua资源
            check_lua_update();
            return;
        }
        Action cb = () =>
        {
            down_index++;
            launcherUI.setProgress(down_list.Count, down_index);
            download_ab(down_list, down_index);
        };

        string url = oss_path + "ver/" + getOSSPlatformString() + "/ver_" + ver + "/ab/" + down_list[down_index];
        Uri uri = new Uri(url);
        download(cb, url, down_list[down_index], dst_ab);
    }

    // 检查Lua脚本是否需要更新
    void check_lua_update()
    {
        launcherUI.setTip(tipTextCheckLua);

        List<string[]> down_list = new List<string[]>();
        List<string[]> list = new List<string[]>();
        string[] fwk = { "fwk", cur_flz_ver };
        string[] game = { "game", cur_geme_ver };
        string[] config = { "config", cur_config_ver };
        list.Add(fwk);
        list.Add(game);
        list.Add(config);

        // 遍历对不上的资源
        foreach (var k in list)
        {
            if (!PlayerPrefs.GetString(k[0], "").Equals(k[1]))
            {
                Debug.Log(k[1]);
                down_list.Add(k);
            }
        }
        // 下载Lua
        download_lua(down_list);
    }

    // 下载Lua
    void download_lua(List<string[]> down_list, int down_index = 0)
    {
        launcherUI.setTip(tipTextUpdateLua);

        if (down_index >= down_list.Count)
        {
            // Lua下载完成
            Debug.Log("download_lua finish!");
            StartCoroutine(UnZip_lua());
            return;
        }
        Action cb = () =>
        {
            PlayerPrefs.SetString(down_list[down_index][0], down_list[down_index][1]);
            down_index++;
            download_lua(down_list, down_index);
        };
        string filename = down_list[down_index][1] + ".zip";
        string url = oss_path + "pkg/" + filename;
        Uri uri = new Uri(url);

        download(cb, url, filename, dst_lua_zip);
    }

    // 解压脚本
    IEnumerator UnZip_lua()
    {
        launcherUI.setTip(tipTextUnzipLua);

        if (Directory.Exists(dst_lua_zip))
        {
            if (!Directory.Exists(dst_lua))
            {
                Directory.CreateDirectory(dst_lua);
                yield return new WaitForSeconds(1.01f);
            }

            DirectoryInfo dir = new DirectoryInfo(dst_lua_zip);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
            foreach (FileSystemInfo i in fileinfo)
            {
                string aFirstName = i.Name.Substring(0, i.Name.LastIndexOf(".")); //文件名
                ZipUtility.UnzipFile(i.FullName, dst_lua);
                yield return new WaitForSeconds(0.01f);
            }

            // 解压完毕，删除目录，避免累积
            Directory.Delete(dst_lua_zip, true);
        }

        // 全部热更完成 加载主场景
        TryEnterGeme();
        // TODO：request_server_list
    }

    /*
     * 请求服务器列表
     */
    void request_server_list()
    {
        string url = serverListUrl;
        Uri uri = new Uri(url);
        HTTPRequest request = new HTTPRequest(uri, on_request_server_list);
        request.DisableCache = true;
        request.Send();

        launcherUI.setTip(tipTextServerList);
    }

    void on_request_server_list(HTTPRequest originalRequest, HTTPResponse response)
    {
        launcherUI.setTip(tipTextNull);

        if (originalRequest == null || response == null)
        {
            launcherUI.showDialog(titleTextNetworkError, contentTextNetworkError, request_server_list, null, delaySecond);
            Debug.Log("no network");
            return;
        }

        Debug.Log("http_finish originalRequest.State = " + originalRequest.State + " response.IsSuccess = " + response.IsSuccess);

        if (originalRequest.State == HTTPRequestStates.Finished && response.IsSuccess)
        {
            Launcher_param.server_list = response.DataAsText;
            try
            {
                bool success = true;
                Dictionary<string, object> serverList = (Dictionary<string, object>)BestHTTP.JSON.Json.Decode(Launcher_param.server_list, ref success);
                if (!success || serverList == null)
                {
                    request_server_list();
                    return;
                }
            }
            catch (Exception)
            {
                throw;
            }
            TryEnterGeme();
        }
        else
        {
            launcherUI.showDialog(titleTextHttpError, contentTextHttpError, request_server_list, null, delaySecond);
            Debug.Log("http error");
            return;
        }

    }

    // 加载主场景
    void TryEnterGeme()
    {
        SceneManager.LoadScene("main");
    }

    /*
     * 获取oss上平台字符串，用于拼接oss地址
     */
    string getOSSPlatformString()
    {
        string platform = platformWin;
        if (Application.platform == RuntimePlatform.Android)
        {
            platform = platformAndroid;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            platform = platformIos;
        }
        return platform;
    }

    bool isSupportPlatform()
    {
        return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
    }

    /*
     * 下载接口
     * cb：下载完成回调
     * url：下载地址
     * name：保存的文件名
     * dir：保存的目录
     */
    void download(Action cb, string url, string name, string dir)
    {
        Debug.Log("[download] " + url);
        Downloader.AsyncLoadFile(url, dir, name,
            (process) =>
            {
                Debug.Log(process);
            },
            (status) =>
            {
                // 出错重试
                download(cb, url, name, dir);
                Debug.Log(status);
            },
            (status, path) =>
            {
                if (status == "Streaming finished!")
                {
                    Debug.Log("download success:" + path);
                    cb();
                }
                else
                {
                    // 出错重试
                    download(cb, url, name, dir);
                    Debug.Log("download fail:" + path);
                }
            });
    }

    public void update_package()
    {
        Application.OpenURL(update_url);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
