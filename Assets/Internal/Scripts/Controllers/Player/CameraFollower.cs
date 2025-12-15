using Internal.Scripts.Bootstrap;
using Internal.Scripts.Bootstrap.Settings;
using UnityEngine;

namespace Internal.Scripts.Controllers.Player
{
  /// <summary>
  /// Класс, управляющий камерой, которая следует за целевым объектом с заданным смещением
  /// </summary>
  public class CameraFollower : MonoBehaviour
  {
    #region Поля

    [SerializeField] 
    private Transform target;

    private static CameraSettings settings
    {
      get => GameSettingsManager.GetInstance().Settings.CameraSettings;
    }
    
    private static GroundGeneratorSettings mapSettings
    {
      get => GameSettingsManager.GetInstance().Settings.GroundGeneratorSettings;
    }
    
    private float minX, maxX, minZ, maxZ;

    #endregion

    #region Методы Unity

    /// <summary>
    /// Вызывается при старте компонента, рассчитывает границы движения камеры
    /// </summary>
    private void Start()
    {
      CalculateBounds();
    }

    /// <summary>
    /// Вызывается каждый кадр, обновляет позицию камеры
    /// </summary>
    private void Update()
    {
      // Плавно следуем за позицией цели с указанным смещением
      var newVector = Vector3.Lerp(transform.position, target.position + settings.CameraOffset, Time.deltaTime * settings.CameraFollowSpeed);

      newVector.x = Mathf.Clamp(newVector.x, minX, maxX);
      newVector.z = Mathf.Clamp(newVector.z, minZ, maxZ);
      
      transform.position = newVector;
    }

    #endregion

    #region Вспомогательные методы

    /// <summary>
    /// Рассчитывает границы движения камеры на основе настроек карты
    /// </summary>
    private void CalculateBounds()
    {
      minX = (-mapSettings.MapSizeX + settings.BoundsMinXModifier) * mapSettings.DefaultTileSize;
      maxX = (mapSettings.MapSizeX + settings.BoundsMaxXModifier) * mapSettings.DefaultTileSize;
      
      minZ = (-mapSettings.MapSizeZ + settings.BoundsMinZModifier) * mapSettings.DefaultTileSize;
      maxZ = (mapSettings.MapSizeZ + settings.BoundsMaxZModifier) * mapSettings.DefaultTileSize;
    }

    #endregion
  }
}