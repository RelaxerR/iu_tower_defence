using System;
using Internal.Scripts.Bootstrap;
using Internal.Scripts.Bootstrap.Settings;
using Internal.Scripts.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Internal.Scripts.Controllers.Resources
{
  public class ResourceTree : ResourceDiamond
  {
    public override string Id { get => ResourceSettings.TreeId; }
  }
}
