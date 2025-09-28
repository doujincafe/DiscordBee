namespace MusicBeePlugin.DiscordTools.Assets.Uploader
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using Newtonsoft.Json;

  public class ImgurUploader : IAssetUploader
  {
    private readonly ImgurClient.ImgurClient _client;
    private readonly string _albumSavePath;

    private readonly Dictionary<string, string> _album;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public ImgurUploader(string albumSavePath, string imgurClientId)
    {
      // Album save path cannot be null or empty
      if (string.IsNullOrEmpty(albumSavePath))
      {
        throw new ArgumentNullException(nameof(albumSavePath));
      }

      _client = new ImgurClient.ImgurClient(imgurClientId);

      // Load album
      _albumSavePath = albumSavePath;
      _album = new Dictionary<string, string>();
      if (File.Exists(_albumSavePath))
      {
        try
        {
          var file = File.ReadAllText(_albumSavePath);
          _album = JsonConvert.DeserializeObject<Dictionary<string, string>>(file);
        }
        catch (Exception)
        {
          // Ignore errors
          _album = new Dictionary<string, string>();
        }
      }
    }

    public Task<bool> DeleteAsset(AlbumCoverData assetData)
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
      _client.Dispose();

      try
      {
        var serialized = JsonConvert.SerializeObject(_album);
        File.WriteAllText(_albumSavePath, serialized, Encoding.UTF8);
      } catch (Exception)
      {
        // Ignore errors
        Debug.Write("Unable to write cache due to an exception!");
      }
    }

    public async Task<Dictionary<string, string>> GetAssets()
    {
      return await Task.FromResult(_album);
    }

    public async Task<bool> Init()
    {
      return await Task.FromResult(true);
    }

    public bool IsAssetCached(AlbumCoverData assetData)
    {
      return false;
    }

    public UploaderHealthInfo GetHealth()
    {
      var health = new UploaderHealthInfo();
      var (status, info) = _client.IsRateLimited();

      health.IsHealthy = !status;
      health.AddInfo(info);

      return health;
    }

    public async Task<UploadResult> UploadAsset(AlbumCoverData assetData)
    {
      if (_album.TryGetValue(assetData.Hash, out var value))
      {
        return new UploadResult { Hash = assetData.Hash, Link = value };
      }

      var uploaded = await _client.UploadImage(assetData.Hash, assetData.ImageB64);
      if (uploaded.StatusCode == 200 && !string.IsNullOrEmpty(uploaded.ImageInfo.Url))
      {
        _album.Add(assetData.Hash, uploaded.ImageInfo.Url);
      }

      return new UploadResult { Hash = assetData.Hash, Link = uploaded.ImageInfo.Url };
    }
  }
}
