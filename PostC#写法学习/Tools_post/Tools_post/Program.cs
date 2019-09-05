using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;


namespace Tools_post
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
            }
            else
            {
                return;
            }

            // 参数Json格式
            string parameter = "{\"type\":\"android\", \"bundle_id\":\"" + args[0] + "\", \"api_token\":\""+ args[1] + "\"}";

            string str_Json = Post("http://api.fir.im/apps", parameter);

            //Json转对象
            JsonInfo jsonInfo = JsonConvert.DeserializeObject<JsonInfo>(str_Json);


            Console.WriteLine(args[2]);

            string path = args[2];

            string name = args[3];

            string version = args[4];

            string build = args[5];

            // 异步上传文件
            curl(jsonInfo.cert.binary.key, jsonInfo.cert.binary.token, name,version,build,path);

            // 推送钉钉

            string url = "https://oapi.dingtalk.com/robot/send?access_token=";
            string token = args[6];
            string test = args[7];
            string messageUrl = args[8];

            PostDindDind(url, token, test, messageUrl);

            Console.WriteLine("请等上传完成后在按回车");

            Console.ReadLine();
        }


        /// <summary>
        /// 异步Post 通过表单形式上传到服务器
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <param name="build"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        static async Task curl(string key,string token,string name,string version ,string build,string path)
        {
  
            //Create List of KeyValuePairs
            List<KeyValuePair<string, string>> bodyProperties = new List<KeyValuePair<string, string>>();

            // 参数

            // 声明数据容器
            var dataContent = new MultipartFormDataContent();


            var keyValue = new ByteArrayContent(Encoding.ASCII.GetBytes(key));
            var tokenValue = new ByteArrayContent(Encoding.ASCII.GetBytes(token));
            var nameValue = new ByteArrayContent(Encoding.ASCII.GetBytes(name));
            var versionValue = new ByteArrayContent(Encoding.ASCII.GetBytes(version));
            var buildValue = new ByteArrayContent(Encoding.ASCII.GetBytes(build));

            dataContent.Add(keyValue, "key");
            dataContent.Add(tokenValue, "token");
            dataContent.Add(nameValue, "x:name");
            dataContent.Add(versionValue, "x:version");
            dataContent.Add(buildValue, "x:build");

            
            using (var client = new HttpClient())
            {
                // open your file
                using (var fs = File.OpenRead(@path))
                {
                    // create StreamContent from the file   
                    var fileValue = new StreamContent(fs);
                    // add the name and meta-data 
                    dataContent.Add(fileValue, "file", "sProject-release.apk");

                    HttpResponseMessage response = await client.PostAsync(
                              "https://up.qbox.me",
                              dataContent);

                    HttpContent responseContent = response.Content;

                   

                    using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                    {
                        Console.WriteLine(await reader.ReadToEndAsync());
                        Console.WriteLine("上传完成");
                    }
                }
            }
        }


        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <param name="content">Post提交数据内容(utf-8编码的)</param>
        /// <returns></returns>
        public static string Post(string url, string content)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/json";

            #region 添加Post 参数
            byte[] data = Encoding.UTF8.GetBytes(content);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }


        /// <summary>
        /// 指定Post地址通过Pust的方式推送钉钉
        /// </summary>
        public static void PostDindDind(string url,string token,string test,string messageUrl)
        {
            // URL地址
            string m_url = url + token;
            //"{\"type\":\"android\", \"bundle_id\":\"com.hsn.xProjectct\", \"api_token\":\"cd5d4fc7ce6c1703e525faaa6a59d044\"}"
            // 拼接Json
            string count_Json = "{\"msgtype\": \"link\",\"link\":{ \"text\": \"" + test + "!!\",\"title\": \"点击这个连接下载\",\"messageUrl\":\"" + messageUrl + "\"}}";

            Post(m_url, count_Json);
            Console.WriteLine("推送钉钉");

        }



        // 数据对象类
        public class JsonInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public string id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string @short { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Cert cert { get; set; }


            public class Cert
            {
                /// <summary>
                /// 
                /// </summary>
                public Icon icon { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public Binary binary { get; set; }
            }

            public class Icon
            {
                /// <summary>
                /// 
                /// </summary>
                public string key { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string token { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string upload_url { get; set; }
            }

            public class Binary
            {
                /// <summary>
                /// 
                /// </summary>
                public string key { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string token { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string upload_url { get; set; }
            }
        }




      
    }




}
