using System;
using Internal.Scripts.Bootstrap;
using Internal.Scripts.Bootstrap.Settings;
using UnityEngine;

namespace Internal.Scripts.Controllers.Player
{
  public class CameraFollower : MonoBehaviour
  {
    [SerializeField] private Transform target;

    private static CameraSettings settings
    {
      get => GameSettingsManager.GetInstance().Settings.CameraSettings;
    }
    private static GroundGeneratorSettings mapSettings
    {
      get => GameSettingsManager.GetInstance().Settings.GroundGeneratorSettings;
    }
    
    private float minX, maxX, minZ, maxZ;
    private void CalculateBounds()
    {
      minX = (-mapSettings.MapSizeX + settings.BoundsMinXModifier) * mapSettings.DefaultTileSize;
      maxX = (mapSettings.MapSizeX + settings.BoundsMaxXModifier) * mapSettings.DefaultTileSize;
      
      minZ = (-mapSettings.MapSizeZ + settings.BoundsMinZModifier) * mapSettings.DefaultTileSize;
      maxZ = (mapSettings.MapSizeZ + settings.BoundsMaxZModifier) * mapSettings.DefaultTileSize;
    }

    private void Start()
    {
      CalculateBounds();
    }

    private void Update()
    {
      // Smoothly follow the target's position with the specified offset
      var newVector = Vector3.Lerp(transform.position, target.position + settings.CameraOffset, Time.deltaTime * settings.CameraFollowSpeed);

      newVector.x = Mathf.Clamp(newVector.x, minX, maxX);
      newVector.z = Mathf.Clamp(newVector.z, minZ, maxZ);
      
      transform.position = newVector;
    }
  }
}
