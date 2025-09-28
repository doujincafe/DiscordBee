namespace MusicBeePlugin.ImgurClient
{
  using Types;
  using RestSharp;
  using RestSharp.Serializers.NewtonsoftJson;
  using System;
  using System.Threading.Tasks;

  public class ImgurClient : IDisposable
  {
    public const string ImgurApiUrl = "https://freeimage.host/api/1/";
    private readonly RestClient _client;
    private readonly string _clientKey;

    public ImgurClient(string clientId)
    {
      var options = new RestClientOptions(ImgurApiUrl)
      {
        ThrowOnAnyError = true,
        FollowRedirects = true,
      };

      _clientKey = clientId;
      _client = new RestClient(options, configureSerialization: s => s.UseNewtonsoftJson());
    }

    public async Task<FreeImageHostUploadResult> UploadImage(string title, string dataB64)
    {
      var request = new RestRequest("upload", Method.Post);

      request.AddParameter("action", "upload");
      request.AddParameter("key", _clientKey);
      request.AddParameter("source", dataB64);
      request.AddParameter("format", "json");
      var response = await _client.PostAsync<FreeImageHostUploadResult>(request);
      return response;
    }

    public (bool status, string info) IsRateLimited()
    {
      return (false, string.Empty);
    }

    public void Dispose()
    {
      _client?.Dispose();
      GC.SuppressFinalize(this);
    }
  }
}
