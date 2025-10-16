using System;
using Internal.Scripts.Bootstrap.Settings;
using Internal.Scripts.Controllers.Player;
using Internal.Scripts.Models;
using TMPro;
using UnityEngine;

namespace Internal.Scripts.UI
{
  [RequireComponent(typeof(TMP_Text))]
  public class ResourceDisplayer : MonoBehaviour
  {
    [SerializeField]
    private ResourceSettings _resourceSettings;
    private TMP_Text _text;

    private void Start()
    {
      _text = GetComponent<TMP_Text>();
    }

    public void OnResourceAmountChanged(string resourceId, int newAmount)
    {
      if (resourceId != _resourceSettings.Id) return;
      if (_text.text != $"{newAmount}") // Avoid unnecessary updates
        _text.text = $"{newAmount}";
    }
  }
}
