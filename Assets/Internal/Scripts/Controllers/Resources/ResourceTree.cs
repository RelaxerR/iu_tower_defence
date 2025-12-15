using Internal.Scripts.Bootstrap.Settings;
using UnityEngine;

namespace Internal.Scripts.Controllers.Resources
{
  /// <summary>
  /// Класс ресурса дерева, наследующий базовую функциональность ресурса
  /// </summary>
  [RequireComponent(typeof(Renderer))]
  public class ResourceTree : ResourceBasic
  {
    /// <summary>
    /// Возвращает идентификатор ресурса дерева
    /// </summary>
    public override string Id { get => ResourceSettings.TreeId; }
  }
}
