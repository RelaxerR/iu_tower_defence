using Internal.Scripts.Bootstrap.Settings;
using UnityEngine;

namespace Internal.Scripts.Controllers.Resources
{
  /// <summary>
  /// Класс ресурса алмаза, наследующий базовую функциональность ресурса
  /// </summary>
  [RequireComponent(typeof(Renderer))]
  public class ResourceDiamond : ResourceBasic
  {
    /// <summary>
    /// Возвращает идентификатор ресурса алмаза
    /// </summary>
    public override string Id { get => ResourceSettings.DiamondId; }
  }
}
