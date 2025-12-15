using UnityEngine;

namespace Internal.Scripts.Bootstrap.Settings
{
  /// <summary>
  /// Настройки ресурса, содержащие параметры одного типа ресурса в игре
  /// </summary>
  [CreateAssetMenu(fileName = "Resource", menuName = "Settings/Resource", order = 0)]
  public class ResourceSettings : ScriptableObject
  {
    /// <summary>
    /// Уникальный идентификатор ресурса
    /// </summary>
    [Tooltip("Уникальный идентификатор ресурса")]
    public string Id;

    /// <summary>
    /// Минимальное количество ресурса при генерации (не может быть меньше 1)
    /// </summary>
    [Min(1), Tooltip("Минимальное количество ресурса при генерации (не может быть меньше 1)")]
    public int MinAmount = 3;

    /// <summary>
    /// Максимальное количество ресурса при генерации
    /// </summary>
    [Tooltip("Максимальное количество ресурса при генерации")]
    public int MaxAmount = 7;

    /// <summary>
    /// Цвет, используемый для отображения выбранного ресурса
    /// </summary>
    [Tooltip("Цвет, используемый для отображения выбранного ресурса")]
    public Color SelectedColor;

    /// <summary>
    /// Цвет по умолчанию для отображения ресурса
    /// </summary>
    [Tooltip("Цвет по умолчанию для отображения ресурса")]
    public Color DefaultColor = Color.white;

        #region Константы типов ресурсов

    /// <summary>
    /// Идентификатор ресурса дерева
    /// </summary>
    public const string TreeId = "Tree";

    /// <summary>
    /// Идентификатор ресурса камня
    /// </summary>
    public const string StoneId = "Stone";

    /// <summary>
    /// Идентификатор ресурса алмаза
    /// </summary>
    public const string DiamondId = "Diamond";

    #endregion
  }
}
