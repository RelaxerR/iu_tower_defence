using Internal.Scripts.Bootstrap.Settings;
using Internal.Scripts.Models;
using UnityEngine;

namespace Internal.Scripts.Controllers.Resources
{
  [RequireComponent(typeof(Renderer))]
  public class ResourceDiamond : ResourceBasic
  {
    public override string Id { get => ResourceSettings.DiamondId; }
  }
}
