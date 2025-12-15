using Internal.Scripts.Bootstrap.Settings;
using Internal.Scripts.Models;
using UnityEngine;

namespace Internal.Scripts.Controllers.Resources
{
  public abstract class ResourceBasic : MonoBehaviour, IResource
  {
    [SerializeField]
    private ResourceSettings _resourceSettings;

    public abstract string Id { get; }
    public int Amount { get; set; }
    private Renderer _renderer;
    private void Start()
    {
      _renderer = GetComponent<Renderer>();
      Amount = Random.Range(_resourceSettings.MinAmount, _resourceSettings.MaxAmount);
    }
    public virtual void Collect()
    {
      Amount--;
      // Debug.Log($"Collected resource: {Id}, Amount left: {Amount}");
    }
    public virtual void OnSelected()
    {
      _renderer.material.color = _resourceSettings.SelectedColor;
    }
    public virtual void OnDeselected()
    {
      _renderer.material.color = _resourceSettings.DefaultColor;
    }
    public virtual void DestroySelf()
    {
      Destroy(gameObject);
    }
  }
}
