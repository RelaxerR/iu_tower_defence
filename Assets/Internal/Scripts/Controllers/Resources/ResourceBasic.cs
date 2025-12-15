using Internal.Scripts.Bootstrap.Settings;
using Internal.Scripts.Models;
using UnityEngine;

namespace Internal.Scripts.Controllers.Resources
{
  /// <summary>
  /// Абстрактный базовый класс для ресурсов, реализующий основную функциональность
  /// </summary>
  public abstract class ResourceBasic : MonoBehaviour, IResource
  {
    #region Поля

    [SerializeField]
    private ResourceSettings _resourceSettings;

    private Renderer _renderer;

    #endregion

    #region Реализация IResource

    /// <summary>
    /// Возвращает идентификатор ресурса (реализуется в наследниках)
    /// </summary>
    public abstract string Id { get; }
    
    /// <summary>
    /// Возвращает или устанавливает количество ресурса
    /// </summary>
    public int Amount { get; set; }

    #endregion

    #region Методы Unity

    /// <summary>
    /// Вызывается при старте компонента, инициализирует ресурс
    /// </summary>
    private void Start()
    {
      _renderer = GetComponent<Renderer>();
      Amount = Random.Range(_resourceSettings.MinAmount, _resourceSettings.MaxAmount);
    }

    #endregion

    #region Реализация IResource

    /// <summary>
    /// Собирает ресурс, уменьшая его количество
    /// </summary>
    public virtual void Collect()
    {
      Amount--;
    }

    /// <summary>
    /// Вызывается при выборе ресурса (изменяет цвет материала)
    /// </summary>
    public virtual void OnSelected()
    {
      _renderer.material.color = _resourceSettings.SelectedColor;
    }

    /// <summary>
    /// Вызывается при отмене выбора ресурса (восстанавливает цвет материала)
    /// </summary>
    public virtual void OnDeselected()
    {
      _renderer.material.color = _resourceSettings.DefaultColor;
    }

    /// <summary>
    /// Уничтожает объект ресурса
    /// </summary>
    public virtual void DestroySelf()
    {
      Destroy(gameObject);
    }

    #endregion
  }
}
