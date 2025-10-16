using System;
using Internal.Scripts.Bootstrap;
using Internal.Scripts.Bootstrap.Settings;
using Internal.Scripts.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Internal.Scripts.Controllers.Resources
{
  public class ResourceTree : MonoBehaviour, IResource
  {
    public string Id { get => "Tree"; }
    public int Amount { get; set; }

    private void Start()
    {
      Amount = Random.Range(IResource.Settings.TreeMinAmount, IResource.Settings.TreeMaxAmount);
    }
    public void Collect()
    {
      Amount--;
    }
    public void CheckDepletion()
    {
      Destroy(gameObject);
    }
  }
}
