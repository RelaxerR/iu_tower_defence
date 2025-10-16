using Internal.Scripts.Bootstrap;
using Internal.Scripts.Bootstrap.Settings;

namespace Internal.Scripts.Models
{
  public interface IResource
  {
    public string Id { get; }
    public int Amount { get; set; }
    public void Collect();
    public void CheckDepletion();
    public static ResourceSettings Settings
    {
      get => GameSettingsManager.GetInstance().Settings.ResourceSettings;
    }
  }
}
