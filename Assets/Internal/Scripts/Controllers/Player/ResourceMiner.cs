using System;
using System.Collections.Generic;
using Internal.Scripts.Models;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace Internal.Scripts.Controllers.Player
{
  /// <summary>
  /// Класс, отвечающий за сбор ресурсов при взаимодействии с ними
  /// </summary>
  [RequireComponent(typeof(Collider))]
  public class ResourceMiner : MonoBehaviour
  {
    #region Поля и события

    [CanBeNull]
    private IResource CurrentResource;
    
    /// <summary>
    /// Событие, вызываемое при сборе ресурса
    /// </summary>
    public UnityEvent<string> OnCollect;

    #endregion

    #region Методы взаимодействия

    /// <summary>
    /// Вызывается для сбора текущего ресурса
    /// </summary>
    public void OnMine()
    {
      CollectResource();
    }

    #endregion

    #region Внутренние методы

    /// <summary>
    /// Собирает текущий ресурс
    /// </summary>
    private void CollectResource()
    {
      if (CurrentResource == null) return;
      
      var resourceId = CurrentResource.Id;

      CurrentResource.Collect();
      OnCollect?.Invoke(resourceId);
      
      if (CurrentResource.Amount > 0)
        return;
      
      CurrentResource.DestroySelf();
      CurrentResource = null;
    }

    #endregion

    #region Методы физики

    /// <summary>
    /// Вызывается при входе в триггер другого объекта
    /// </summary>
    /// <param name="other">Объект, с которым произошло столкновение</param>
    private void OnTriggerEnter(Collider other)
    {
      if (!other.TryGetComponent<IResource>(out var resource))
        return;
      
      resource.OnSelected();
      CurrentResource = resource;
    }
    
    /// <summary>
    /// Вызывается при выходе из триггера другого объекта
    /// </summary>
    /// <param name="other">Объект, с которым произошло столкновение</param>
    private void OnTriggerExit(Collider other)
    {
      if (!other.TryGetComponent<IResource>(out var resource))
        return;
      
      resource.OnDeselected();
      CurrentResource = null;
    }

    #endregion
  }
}