using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;

namespace AutoDox.UI.Models
{
    internal class HttpRequestManager
    {
        private readonly HttpClient client;

        public HttpRequestManager() 
        {
            client = new HttpClient
            {
                BaseAddress = new Uri("https://kroki.io/plantuml/svg/")
            };
        }

        public async void GetSvgFromPlantUml(string pumlPath)
        {
            byte[] compressedBytes = ZlibDeflate(Encoding.UTF8.GetBytes(DiagramGeneratorManager.ReadPlantUml(pumlPath)));
            string encodedOutput = Convert.ToBase64String(compressedBytes).Replace('+', '-').Replace('/', '_');

            using HttpResponseMessage response = await client.GetAsync(encodedOutput);
            HttpContent responseContent = response.Content;
            string result = await responseContent.ReadAsStringAsync();
            DiagramGeneratorManager.WriteSvg(result, Path.Combine(Path.GetDirectoryName(pumlPath),
                                                     Path.GetFileNameWithoutExtension(pumlPath) + ".svg"));
        }

        public async void GetSvgFromPlantUml(List<string> pumlPaths)
        {
            foreach (string path in pumlPaths)
            {
                byte[] compressedBytes = ZlibDeflate(Encoding.UTF8.GetBytes(DiagramGeneratorManager.ReadPlantUml(path)));
                string encodedOutput = Convert.ToBase64String(compressedBytes).Replace('+', '-').Replace('/', '_');

                using HttpResponseMessage response = await client.GetAsync(encodedOutput);
                HttpContent responseContent = response.Content;
                string result = await responseContent.ReadAsStringAsync();
                DiagramGeneratorManager.WriteSvg(result, Path.Combine(Path.GetDirectoryName(path),
                                                         Path.GetFileNameWithoutExtension(path) + ".svg"));
            }                                    
        }

        private static byte[] ZlibDeflate(byte[] data, CompressionLevel? level = null)
        {
            byte[] newData;
            using (var memStream = new MemoryStream())
            {
                memStream.WriteByte(0x78);
                switch (level)
                {
                    case CompressionLevel.NoCompression:
                    case CompressionLevel.Fastest:
                        memStream.WriteByte(0x01);
                        break;
                    case CompressionLevel.Optimal:
                        memStream.WriteByte(0xDA);
                        break;
                    default:
                        memStream.WriteByte(0x9C);
                        break;
                }

                using (var dflStream = level.HasValue
                           ? new DeflateStream(memStream, level.Value)
                           : new DeflateStream(memStream, CompressionMode.Compress
                           )) dflStream.Write(data, 0, data.Length);
                newData = memStream.ToArray();
            }

            uint a1 = 1, a2 = 0;
            foreach (byte b in data)
            {
                a1 = (a1 + b) % 65521;
                a2 = (a2 + a1) % 65521;
            }

            var adlerPos = newData.Length;
            Array.Resize(ref newData, adlerPos + 4);
            newData[adlerPos] = (byte)(a2 >> 8);
            newData[adlerPos + 1] = (byte)a2;
            newData[adlerPos + 2] = (byte)(a1 >> 8);
            newData[adlerPos + 3] = (byte)a1;
            return newData;
        }
    }
}
