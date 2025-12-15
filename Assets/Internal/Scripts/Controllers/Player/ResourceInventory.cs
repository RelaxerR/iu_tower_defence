using System;
using System.Collections.Generic;
using System.Linq;
using Internal.Scripts.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Internal.Scripts.Controllers.Player
{
  public class ResourceInventory : MonoBehaviour
  {
    private readonly Dictionary<string, int> resourceAmounts = new();
    
    public UnityEvent<string, int> OnResourceAmountChanged;

    public void OnResourceCollected(string resourceId)
    {
      if (!resourceAmounts.TryAdd(resourceId, 1))
        resourceAmounts[resourceId]++;
      
      OnResourceAmountChanged?.Invoke(resourceId, resourceAmounts[resourceId]);
    }
    
    public void OnResourceDropped(string resourceId)
    {
      if (!resourceAmounts.ContainsKey(resourceId)) throw new ArgumentException($"Resource '{resourceId}' not found in inventory.");
      
      resourceAmounts[resourceId]--;
      if (resourceAmounts[resourceId] <= 0)
        throw new ArgumentException($"Resource '{resourceId}' amount cannot be less than zero.");
      
      OnResourceAmountChanged?.Invoke(resourceId, resourceAmounts[resourceId]);
    }
    
    // Новый метод для получения текущего количества ресурса
    public int GetResourceAmount(string resourceId)
    {
      return resourceAmounts.ContainsKey(resourceId) ? resourceAmounts[resourceId] : 0;
    }
  }
}
