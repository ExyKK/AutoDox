using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace AutoDox.UI.Models
{
    class DiagramGeneratorManager
    {
        string destinationDirectory;
        string[] sourceFiles;
        string[] pumlFiles;

        public void Run(object destinationDirectory)
        {
            this.destinationDirectory = (string)destinationDirectory;

            sourceFiles = ExplorerDialog.SelectFiles();

            // scan source files, create .puml`s

            Test();
        }

        public string Read()
        {
            StringBuilder pumlString = new();
            using (StreamReader reader = new(sourceFiles[0])) // change source files to puml
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    pumlString.AppendLine(line);
                }
            }
            return pumlString.ToString();
        }

        public void Write(string input)
        {
            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);

            using (StreamWriter writer = new(Path.Combine(destinationDirectory, "result.svg")))
            {
                writer.Write(input);
            }
        }

        //private void OnSendRequestButtonClicked()
        //{
        //    string targetUrl = "https://kroki.io/plantuml/svg/";
        //    RequestMethod requestMethod = RequestMethod.POST;
        //    ByteArrayContent requestBody = new ByteArrayContent(
        //        Encoding.UTF8.GetBytes(Read())
        //    );
            
        //    SendRequest(targetUrl, requestMethod, requestBody);
        //}

        private async void Test()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://kroki.io/");
            client.DefaultRequestHeaders.Add("Accept", "image/svg+xml");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "plantuml/svg/");
            request.Content = new StringContent(Read(),
                                                Encoding.UTF8,
                                                "text/plain");//CONTENT-TYPE header

            HttpResponseMessage httpResponse = null;
            //httpResponse = await client.PostAsync(target, body);

            httpResponse = client.SendAsync(request).Result;

            HttpContent responseContent = httpResponse.Content;
            string result = await responseContent.ReadAsStringAsync();

            Write(result);

            //client.SendAsync(request)
            //      .ContinueWith(responseTask =>
            //      {
            //          Write(responseTask.Result.ToString());
            //      });
        }

        //private async void SendRequest(string target, RequestMethod method, HttpContent body)
        //{
        //    RequestManager manager = RequestManager.Get();
        //    RequestManager.Response response = await manager.SendRequest(MakeAbsolute(target), method, body);

        //    string contentOutput = response.content;
        //    Write(contentOutput);

        //    string statusCodeOutput = $"Response Code: {response.statusCodeName} ({response.statusCode}).";
        //}

        //private string MakeAbsolute(string url)
        //{
        //    if (url.StartsWith("https://") || url.StartsWith("http://"))
        //    {
        //        return url;
        //    }
        //    else
        //    {
        //        return $"https://{url}";
        //    }
        //}
    }
}
