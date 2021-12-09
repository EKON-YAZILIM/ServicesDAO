using Helpers.Models.KYCModels;
using Helpers.Models.SharedModels;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Helpers
{
    /// <summary>
    ///  Generic HTTP Requests class
    /// </summary>
    public static class Request
    {
        public static string Get(string url, string token="")
        {
            string result = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Headers.Add("AcceptLanguage", System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
                if(!String.IsNullOrEmpty(token))
                    request.Headers.Add("Authorization", "Bearer " + token);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())              
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {                    
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
               
            }

            return result;
        }

        public static string Post(string url, string postData, string token = "")
        {
            string result = string.Empty;

            try
            {
                
                var request = (HttpWebRequest)WebRequest.Create(url);

                var data = Encoding.UTF8.GetBytes(postData);

                request.Headers.Add("AcceptLanguage", System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
                if (!String.IsNullOrEmpty(token))
                    request.Headers.Add("Authorization", "Bearer " + token);
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST";
                request.Accept = "application/json; charset=utf-8";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {

            }

            return result;
        }
        public static string KYCPost(string url, string postData, string token = "", string contentType = "application/json")
        {
            string result = string.Empty;

            try
            {

                var request = (HttpWebRequest)WebRequest.Create(url);

                var data = Encoding.UTF8.GetBytes(postData);

                request.Headers.Add("AcceptLanguage", System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
                if (!String.IsNullOrEmpty(token))
                    request.Headers.Add("Authorization", "Token " + token);
                request.ContentType = contentType;
                request.Method = "POST";
                request.Accept = "application/json";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public static string KYCGet(string url, string token = "")
        {
            string result = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Headers.Add("AcceptLanguage", System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
                if (!String.IsNullOrEmpty(token))
                    request.Headers.Add("Authorization", "Token " + token);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public static SimpleResponse Upload(string url, IFormFile file, IFormFile file2 = null)
        {
            SimpleResponse res = new SimpleResponse();

            try
            {
                if (file != null && file.Length > 0)
                {
                    using (var client = new HttpClient())
                    {

                        byte[] data;
                        using (var br = new BinaryReader(file.OpenReadStream()))
                            data = br.ReadBytes((int)file.OpenReadStream().Length);

                        ByteArrayContent bytes = new ByteArrayContent(data);


                        MultipartFormDataContent multiContent = new MultipartFormDataContent();

                        multiContent.Add(bytes, "file", file.FileName);

                        if(file2 != null && file2.Length > 0)
                        {
                            byte[] data2;
                            using (var br2 = new BinaryReader(file2.OpenReadStream()))
                                data2 = br2.ReadBytes((int)file2.OpenReadStream().Length);

                            ByteArrayContent bytes2 = new ByteArrayContent(data2);

                            multiContent.Add(bytes2, "file2", file2.FileName);
                        }


                        var result = client.PostAsync(url, multiContent).Result;

                        res = Serializers.DeserializeJson<SimpleResponse>(result.Content.ReadAsStringAsync().Result);
                    }
                }


            }
            catch (Exception ex)
            {
            }

            return res;
        }

        public static KYCFileResponse UploadFiletoKYCAID(string url, IFormFile file, string token)
        {
            KYCFileResponse res = new KYCFileResponse();
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            try
            {

                if (file != null && file.Length > 0)
                {
                    using (var client = new HttpClient())
                    {
                       
                        byte[] data;
                        using (var br = new BinaryReader(file.OpenReadStream()))
                            data = br.ReadBytes((int)file.OpenReadStream().Length);

                        ByteArrayContent bytes = new ByteArrayContent(data);
                        MultipartFormDataContent multiContent = new MultipartFormDataContent();
                        multiContent.Add(bytes, "file", file.FileName);

                        client.DefaultRequestHeaders.Add("Authorization", "Token " + token);
                        var result = client.PostAsync(url, multiContent).Result;

                        var res2 = result.Content.ReadAsStringAsync().Result;

                        res = Serializers.DeserializeJson<KYCFileResponse>(result.Content.ReadAsStringAsync().Result);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return res;
        }

        public static string Put(string url, string postData, string token = "")
        {
            string result = string.Empty;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);

                var data = Encoding.UTF8.GetBytes(postData);

                request.Headers.Add("AcceptLanguage", System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
                if (!String.IsNullOrEmpty(token))
                    request.Headers.Add("Authorization", "Bearer " + token);
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "PUT";
                request.Accept = "application/json; charset=utf-8";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public static string Delete(string url, string token = "")
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "DELETE";
                request.Headers.Add("AcceptLanguage", System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
                if (!String.IsNullOrEmpty(token))
                    request.Headers.Add("Authorization", "Bearer " + token);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                   
                }
            }
            catch(Exception ex)
            {

            }
            return result;
        }

        public static bool Download(string url, string downloadPath)
        {
            string result = string.Empty;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);

                using (var client = new WebClient())
                {
                    string filename = System.IO.Path.GetFileName(url);
                    client.DownloadFile(url, downloadPath + "\\" + filename);
                }

                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }         
    }
}
