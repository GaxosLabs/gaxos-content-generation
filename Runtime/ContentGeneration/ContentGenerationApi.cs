using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ContentGeneration.Models;
using ContentGeneration.Models.DallE;
using ContentGeneration.Models.Meshy;
using ContentGeneration.Models.Stability;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace ContentGeneration
{
    public class ContentGenerationApi
    {
        public static readonly ContentGenerationApi Instance = new();

        enum ApiMethod
        {
            Get,
            Post,
            Delete,
            Patch
        }

        const string BaseUrl = "https://content-generation-21ab4.web.app/";
        // const string BaseUrl = "http://localhost:5002/";

        Task<string> SendRequest(ApiMethod method, string endpoint,
            Dictionary<string, string> headers = null,
            object data = null)
        {
            var ret = new TaskCompletionSource<string>();
            Dispatcher.instance.StartCoroutine(SendRequestCo(method, endpoint, headers, data, ret));
            return ret.Task;
        }

        IEnumerator SendRequestCo(ApiMethod method, string endpoint, Dictionary<string, string> headers, object data,
            TaskCompletionSource<string> ret)
        {
            var url = BaseUrl + endpoint.TrimStart('/');
            if (data != null && method == ApiMethod.Get)
            {
                var parametersStr = new StringBuilder();
                var properties = data.GetType().GetProperties();
                if (properties.Length > 0)
                {
                    foreach (var property in properties)
                    {
                        parametersStr.Append(
                            $"&{UnityWebRequest.EscapeURL(property.Name)}={UnityWebRequest.EscapeURL(property.GetValue(data)?.ToString())}");
                    }
                }

                parametersStr[0] = '?';
                url += parametersStr;
            }

            var www = new UnityWebRequest(url, method.ToString().ToUpperInvariant());
            www.downloadHandler = new DownloadHandlerBuffer();

            headers ??= new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {Settings.instance.apiKey}");

            foreach (var header in headers)
            {
                www.SetRequestHeader(header.Key, header.Value);
            }

            var contentData = Array.Empty<byte>();
            if (data != null && method != ApiMethod.Get)
            {
                var dataJson = JsonConvert.SerializeObject(data,
                    Formatting.None,
                    new GeneratorTypeConverter());
                contentData = Encoding.UTF8.GetBytes(dataJson);
            }

            www.uploadHandler = new UploadHandlerRaw(contentData);
            www.uploadHandler.contentType = "application/json";

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                ret.SetException(new ContentGenerationApiException(www, headers, data));
                yield break;
            }

#if DEBUG_CONTENT_GENERATION
            Debug.LogWarning(ContentGenerationApiException.GetWwwDetails(www, headers));
#endif

            ret.SetResult(www.downloadHandler.text);
        }

        public async Task<float> GetCredits()
        {
            return float.Parse(await SendRequest(ApiMethod.Get, "credits"));
        }

        public async Task<Request[]> GetRequests()
        {
            return JsonConvert.DeserializeObject<Request[]>(await SendRequest(ApiMethod.Get, "request"));
        }

        public async Task<Request> GetRequest(string id)
        {
            return JsonConvert.DeserializeObject<Request>(await SendRequest(ApiMethod.Get, $"request/{id}"));
        }

        public async Task<PublishedImage[]> DeleteRequest(string id)
        {
            return JsonConvert.DeserializeObject<PublishedImage[]>(await SendRequest(ApiMethod.Delete,
                $"request/{id}"));
        }

        public Task<string> RequestGeneration(
            Generator generator,
            object generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return SendRequest(ApiMethod.Post,
                "request",
                data: new
                {
                    data,
                    generator = GeneratorTypeConverter.ToString(generator),
                    generator_parameters = generatorParameters,
                    options
                });
        }

        public Task<string> RequestStabilityTextToImageGeneration(
            StabilityTextToImageParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.StabilityTextToImage,
                generatorParameters, options, data);
        }

        public Task<string> RequestStabilityImageToImageGeneration(
            StabilityImageToImageParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.StabilityImageToImage,
                generatorParameters, options, data);
        }

        public Task<string> RequestStabilityMaskedImageGeneration(
            StabilityMaskedImageParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.StabilityMasking,
                generatorParameters, options, data);
        }

        public Task<string> RequestDallETextToImageGeneration(
            DallETextToImageParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.DallETextToImage,
                generatorParameters, options, data);
        }


        public Task<string> RequestDallEInpaintingGeneration(
            DallEInpaintingParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.DallEInpainting,
                generatorParameters, options, data);
        }

        public async Task MakeImagePublic(string id, uint index, bool makeItPublic)
        {
            await SendRequest(ApiMethod.Patch, $"request/{id}/{(makeItPublic ? "publish" : "unpublish")}/{index}");
        }

        public async Task<PublishedImage[]> GetPublishedImages()
        {
            return JsonConvert.DeserializeObject<PublishedImage[]>(await SendRequest(ApiMethod.Get,
                $"image"));
        }

        public async Task<PublishedImage> GetPublishedImage(string publishedImageId)
        {
            return JsonConvert.DeserializeObject<PublishedImage>(await SendRequest(ApiMethod.Get,
                $"image/{publishedImageId}"));
        }

        public async Task<string> ImprovePrompt(string prompt, string generator)
        {
            return (await SendRequest(ApiMethod.Get,
                "improvePrompt" +
                $"?generator={WebUtility.UrlDecode(generator)}" +
                $"&prompt={WebUtility.UrlDecode(prompt)}"
            )).Trim('"');
        }

        public Task<string> RequestMeshyTextToMeshGeneration(MeshyTextToMeshParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.MeshyTextToMesh,
                generatorParameters, options, data);
        }
        
        public async Task RefineMeshyTextToMesh(string id, MeshyRefineTextToMeshParameters parameters = null)
        {
            await SendRequest(ApiMethod.Patch, $"request/{id}/meshy-refine-text-to-mesh", data: parameters);
        }
        
        public Task<string> RequestMeshyTextToTextureGeneration(
            MeshyTextToTextureParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.MeshyTextToTexture,
                generatorParameters, options, data);
        }
    }
}