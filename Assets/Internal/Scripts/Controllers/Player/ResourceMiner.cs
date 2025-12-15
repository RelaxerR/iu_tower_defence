using System;
using System.Collections.Generic;
using Internal.Scripts.Models;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace Internal.Scripts.Controllers.Player
{
  [RequireComponent(typeof(Collider))]
  public class ResourceMiner : MonoBehaviour
  {
    [CanBeNull]
    private IResource CurrentResource;
    
    public UnityEvent<string> OnCollect;

    public void OnMine()
    {
      CollectResource();
    }

    private void CollectResource()
    {
      if (CurrentResource == null) return;
      
      // Debug.Log($"Collecting resource: {CurrentResource.Id}, Amount left: {CurrentResource.Amount}");
      var resourceId = CurrentResource.Id;

      CurrentResource.Collect();
      OnCollect?.Invoke(resourceId);
      
      if (CurrentResource.Amount > 0)
        return;
      
      CurrentResource.DestroySelf();
      CurrentResource = null;
    }

    private void OnTriggerEnter(Collider other)
    {
      if (!other.TryGetComponent<IResource>(out var resource))
        return;
      
      // Debug.Log($"Entered resource area ({resource.Id})");
      resource.OnSelected();
      CurrentResource = resource;
    }
    private void OnTriggerExit(Collider other)
    {
      if (!other.TryGetComponent<IResource>(out var resource))
        return;
      
      // Debug.Log($"Exited resource area ({resource.Id})");
      resource.OnDeselected();
      CurrentResource = null;
    }
  }
}
