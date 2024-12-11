namespace MusicBeePlugin.ImgurClient.Types
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Text.Json.Serialization;
  using System.Threading.Tasks;

  public class ImgurCreateAlbumRequest
  {
    [JsonPropertyName("title")]
    public string Title { get; set; } = "DiscordBee";
  }
}
