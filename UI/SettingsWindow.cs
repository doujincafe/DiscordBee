namespace MusicBeePlugin.UI
{
  using System;
  using System.Drawing;
  using System.Text.RegularExpressions;
  using System.Windows.Forms;

  public partial class SettingsWindow : Form
  {
    private readonly Plugin _parent;
    private PlaceholderTableWindow _placeholderTableWindow;
    private readonly Settings _settings;
    private bool _defaultsRestored;

    public SettingsWindow(Plugin parent, Settings settings)
    {
      _parent = parent;
      _settings = settings;
      InitializeComponent();
      UpdateValues(_settings);
      Text += " (v" + parent.GetVersionString() + ")";

      FormClosing += OnFormClosing;
      Shown += OnShown;
      VisibleChanged += OnVisibleChanged;
    }

    private void OnVisibleChanged(object sender, EventArgs eventArgs)
    {
      if (Visible)
      {
        UpdateValues(_settings);
      }
    }

    private void OnShown(object sender, EventArgs eventArgs)
    {
      UpdateValues(_settings);
    }

    private void OnFormClosing(object sender, FormClosingEventArgs e)
    {
      if (e.CloseReason != CloseReason.UserClosing)
      {
        return;
      }

      Hide();
      e.Cancel = true;
    }

    private void UpdateValues(Settings settings)
    {
      textBoxTrackNo.Text = settings.PresenceTrackNo;
      textBoxTrackCnt.Text = settings.PresenceTrackCnt;
      textBoxDetails.Text = settings.PresenceDetails;
      textBoxState.Text = settings.PresenceState;
      textBoxLargeImage.Text = settings.LargeImageText;
      textBoxSmallImage.Text = settings.SmallImageText;
      textBoxSeparator.Text = settings.Separator;
      textBoxDiscordAppId.Text = settings.DiscordAppId;
      textBoxImgurClientId.Text = settings.ImgurClientId;
      checkBoxPresenceUpdate.Checked = settings.UpdatePresenceWhenStopped;
      checkBoxShowTime.Checked = settings.ShowTime;
      checkBoxTextOnly.Checked = settings.TextOnly;
      checkBoxShowPlayState.Checked = settings.ShowPlayState;
      checkBoxShowOnlyNonPlayingState.Checked = settings.ShowOnlyNonPlayingState;
      checkBoxArtworkUpload.Checked = settings.UploadArtwork;
      customButtonLabel.Text = settings.ButtonLabel;
      customButtonUrl.Text = settings.ButtonUrl;
      customButtonLabel2.Text = settings.ButtonLabel2;
      customButtonUrl2.Text = settings.ButtonUrl2;
      customButtonEnable.Checked = settings.Button1Enabled;
      customButtonEnable2.Checked = settings.Button2Enabled;

      ValidateInputs();
    }

    private void buttonPlaceholders_Click(object sender, EventArgs e)
    {
      _placeholderTableWindow = new PlaceholderTableWindow();
      _placeholderTableWindow.UpdateTable(_parent.GenerateMetaDataDictionary());
      _placeholderTableWindow.Show(this);
    }

    private void buttonRestoreDefaults_Click(object sender, EventArgs e)
    {
      _settings.Clear();
      UpdateValues(_settings);
      _defaultsRestored = true;
    }

    private void buttonSaveClose_Click(object sender, EventArgs e)
    {
      if (!ValidateInputs())
      {
        return;
      }

      _settings.PresenceTrackNo = textBoxTrackNo.Text;
      _settings.PresenceTrackCnt = textBoxTrackCnt.Text;
      _settings.PresenceDetails = textBoxDetails.Text;
      _settings.PresenceState = textBoxState.Text;
      _settings.LargeImageText = textBoxLargeImage.Text;
      _settings.SmallImageText = textBoxSmallImage.Text;
      _settings.Separator = textBoxSeparator.Text;
      _settings.DiscordAppId = string.IsNullOrWhiteSpace(textBoxDiscordAppId.Text) ? null : textBoxDiscordAppId.Text;
      _settings.ImgurClientId = string.IsNullOrWhiteSpace(textBoxImgurClientId.Text) ? null : textBoxImgurClientId.Text;
      _settings.UpdatePresenceWhenStopped = checkBoxPresenceUpdate.Checked;
      _settings.ShowTime = checkBoxShowTime.Checked;
      _settings.TextOnly = checkBoxTextOnly.Checked;
      _settings.ShowPlayState = checkBoxShowPlayState.Checked;
      _settings.ShowOnlyNonPlayingState = checkBoxShowOnlyNonPlayingState.Checked;
      _settings.UploadArtwork = checkBoxArtworkUpload.Checked;
      _settings.ButtonUrl = customButtonUrl.Text;
      _settings.ButtonLabel = customButtonLabel.Text;
      _settings.ButtonLabel2 = customButtonLabel2.Text;
      _settings.ButtonUrl2 = customButtonUrl2.Text;
      _settings.Button1Enabled = customButtonEnable.Checked;
      _settings.Button2Enabled = customButtonEnable2.Checked;

      if (_defaultsRestored && !_settings.IsDirty)
      {
        _settings.Delete();
        _defaultsRestored = false;
      }

      _settings.Save();
      Hide();
    }

    private bool ValidateInputs()
    {
      bool ContainsDigitsOnly(string s)
      {
        foreach (char c in s)
        {
          if (c < '0' || c > '9')
          {
            return false;
          }
        }
        return true;
      }

      bool validateDiscordId()
      {
        var exp = new Regex(@"^[0-9]{18,}$", RegexOptions.None);
        if (exp.IsMatch(textBoxDiscordAppId.Text))
        {
          textBoxDiscordAppId.BackColor = Color.White;
          return true;
        }

        textBoxDiscordAppId.BackColor = Color.Red;
        return false;
      }

      if (textBoxDiscordAppId.Text.Length > 0 && !validateDiscordId())
      {
        return false;
      }

      bool validateImgurClientId()
      {
        var exp = new Regex(@"^[a-f0-9]{32}$", RegexOptions.IgnoreCase);
        if (exp.IsMatch(textBoxImgurClientId.Text ?? "") && textBoxImgurClientId.Text.Length == 32)
        {
          textBoxImgurClientId.BackColor = Color.White;
          return true;
        }

        textBoxImgurClientId.BackColor = Color.Red;
        return false;
      }

      if (checkBoxArtworkUpload.Checked && textBoxImgurClientId.Text.Length > 0 && !validateImgurClientId())
      {
        return false;
      }

      // First Button validation
      if (customButtonEnable.Checked && (!validateUriFromTextBox(customButtonUrl) || string.IsNullOrEmpty(customButtonLabel.Text)))
      {
        return false;
      }

      // Second Button validation
      if (customButtonEnable2.Checked && (!validateUriFromTextBox(customButtonUrl2) || string.IsNullOrEmpty(customButtonLabel2.Text)))
      {
        return false;
      }

      ResetErrorIndications();

      return true;
    }

    private bool validateUriFromTextBox(TextBox textBox)
    {
      if (!ValidationHelpers.ValidateUri(textBox.Text))
      {
        textBox.BackColor = Color.PaleVioletRed;
        return false;
      }

      textBox.BackColor = Color.FromArgb(114, 137, 218);
      return true;
    }

    private void ResetErrorIndications()
    {
      textBoxDiscordAppId.BackColor = SystemColors.Window;
      textBoxImgurClientId.BackColor = SystemColors.Window;
      customButtonUrl.BackColor = Color.FromArgb(114, 137, 218);
    }

    private void textBoxDiscordAppId_TextChanged(object sender, EventArgs e)
    {
      ValidateInputs();
    }

    private void checkBoxArtworkUpload_CheckedChanged(object sender, EventArgs e)
    {
      ValidateInputs();
    }

    private void customButtonUrl_TextChanged(object sender, EventArgs e)
    {
      ValidateInputs();
    }

    private void textBoxImgurClientId_TextChanged(object sender, EventArgs e)
    {
      ValidateInputs();
    }
  }
}
