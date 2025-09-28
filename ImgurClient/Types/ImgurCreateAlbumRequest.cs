namespace MusicBeePlugin.ImgurClient.Types
{
  using System.Text.Json.Serialization;

  public class ImgurCreateAlbumRequest
  {
    [JsonPropertyName("title")]
    public string Title { get; set; } = "DiscordBee";
  }
}
