using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Internal.Scripts.Controllers.Player
{
  /// <summary>
  /// Класс инвентаря ресурсов, управляющий сбором и использованием ресурсов
  /// </summary>
  public class ResourceInventory : MonoBehaviour
  {
    #region Поля и события

    private readonly Dictionary<string, int> resourceAmounts = new();
    
    /// <summary>
    /// Событие, вызываемое при изменении количества ресурса
    /// </summary>
    public UnityEvent<string, int> OnResourceAmountChanged;

    #endregion

    #region Методы управления ресурсами

    /// <summary>
    /// Добавляет ресурс в инвентарь при его сборе
    /// </summary>
    /// <param name="resourceId">Идентификатор ресурса</param>
    public void OnResourceCollected(string resourceId)
    {
      if (!resourceAmounts.TryAdd(resourceId, 1))
        resourceAmounts[resourceId]++;
      
      OnResourceAmountChanged?.Invoke(resourceId, resourceAmounts[resourceId]);
    }
    
    /// <summary>
    /// Уменьшает количество ресурса в инвентаре при его использовании
    /// </summary>
    /// <param name="resourceId">Идентификатор ресурса</param>
    /// <exception cref="ArgumentException">Если ресурс не найден в инвентаре или его количество становится меньше нуля</exception>
    public void OnResourceDropped(string resourceId)
    {
      if (!resourceAmounts.ContainsKey(resourceId)) 
        throw new ArgumentException($"Ресурс '{resourceId}' не найден в инвентаре.");
      
      resourceAmounts[resourceId]--;
      if (resourceAmounts[resourceId] <= 0)
        throw new ArgumentException($"Количество ресурса '{resourceId}' не может быть меньше нуля.");
      
      OnResourceAmountChanged?.Invoke(resourceId, resourceAmounts[resourceId]);
    }
    
    /// <summary>
    /// Возвращает текущее количество указанного ресурса
    /// </summary>
    /// <param name="resourceId">Идентификатор ресурса</param>
    /// <returns>Количество ресурса или 0, если ресурс не найден</returns>
    public int GetResourceAmount(string resourceId)
    {
      return resourceAmounts.TryGetValue(resourceId, out var amount) ? amount : 0;
    }

    #endregion
  }
}