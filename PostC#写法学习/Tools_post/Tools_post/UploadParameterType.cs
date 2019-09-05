
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Tools_post
{
    public class UploadParameterType
    {

        public Dictionary<string, string> PostParameters = new Dictionary<string, string>();

        public string FileNameKey = "";
        public string FileNameValue = "";
        public string Url = "";

        public byte[] GetBytes(string endBoundary)
        {
            throw new NotImplementedException();
        }
    }
}