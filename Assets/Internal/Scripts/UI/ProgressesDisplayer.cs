using System;
using Internal.Scripts.Bootstrap;
using Internal.Scripts.Controllers.Buildings;
using UnityEngine;
using UnityEngine.UI;

namespace Internal.Scripts.UI
{
  public class ProgressesDisplayer : MonoBehaviour
  {
    public Slider CastleHPSlider;
    public Slider LevelProgressSlider;

    private void Start()
    {
      CastleHPSlider.maxValue = GameSettingsManager.GetInstance().Settings.PlayerSettings.CastleMaxHealth;
      LevelProgressSlider.maxValue = GameSettingsManager.GetInstance().Settings.LevelMaxValue;
      
      CastleController.GetInstance().OnHealthChanged.AddListener(OnCastleHPChanged);
      GameManager.GetInstance().OnLevelProgressChanged.AddListener(OnLevelProgressChanged);
    }

    private void OnCastleHPChanged(float value)
    {
      CastleHPSlider.value = value;
      var percent = (value / CastleHPSlider.maxValue) * 100;
      CastleHPSlider.fillRect.GetComponent<Image>().color = percent switch
      {
        > 70 => Color.green,
        > 30 => Color.yellow,
        _ => Color.red
      };
    }
    private void OnLevelProgressChanged(float value)
    {
      LevelProgressSlider.value = value;
    }
  }
}
