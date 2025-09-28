namespace MusicBeePlugin.ImgurClient.Types;

using Newtonsoft.Json;

public sealed class FreeImageHostUploadResult
{
  [JsonProperty("status_code")]
  public int StatusCode { get; set; }

  [JsonProperty("success")]
  public SuccessDetail SuccessInfo { get; set; }

  [JsonProperty("image")]
  public ImageUploadResult ImageInfo { get; set; }
}

public sealed class SuccessDetail
{
  [JsonProperty("message")]
  public string Message { get; set; }

  [JsonProperty("code")]
  public int Code { get; set; }
}

public sealed class ImageUploadResult
{
  // Omitted some things we don't use. If you are interested in them, see https://freeimage.host/page/api

  [JsonProperty("url")]
  public string Url { get; set; }

  [JsonProperty("url_viewer")]
  public string UrlViewer { get; set; }

  [JsonProperty("image")]
  public ImageVariant Image { get; set; }

  [JsonProperty("thumb")]
  public ImageVariant Thumb { get; set; }

  [JsonProperty("medium")]
  public ImageVariant Medium { get; set; }
}

public sealed class ImageVariant
{
  [JsonProperty("url")]
  public string Url { get; set; }
}
