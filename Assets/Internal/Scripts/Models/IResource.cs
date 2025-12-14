using Internal.Scripts.Bootstrap;
using Internal.Scripts.Bootstrap.Settings;
using UnityEngine;

namespace Internal.Scripts.Models
{
  public interface IResource
  {
    public string Id { get; }
    public int Amount { get; set; }
    public void Collect();
    public void OnSelected();
    public void OnDeselected();
    public void DestroySelf();
  }
}
