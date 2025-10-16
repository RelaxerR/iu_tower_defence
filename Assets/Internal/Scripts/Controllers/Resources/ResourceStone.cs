using Internal.Scripts.Bootstrap.Settings;
using Internal.Scripts.Models;
using UnityEngine;

namespace Internal.Scripts.Controllers.Resources
{
  public class ResourceStone : ResourceDiamond
  {
    public override string Id { get => ResourceSettings.StoneId; }
  }
}
