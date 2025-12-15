using System;
using Internal.Scripts.Bootstrap;
using Internal.Scripts.Controllers.Buildings;
using UnityEngine;
using UnityEngine.UI;

namespace Internal.Scripts.UI
{
  /// <summary>
  /// Класс, отображающий прогресс здоровья замка и прогресс уровня
  /// </summary>
  public class ProgressesDisplayer : MonoBehaviour
  {
    #region Поля

    /// <summary>
    /// Слайдер отображения здоровья замка
    /// </summary>
    public Slider CastleHPSlider;
    
    /// <summary>
    /// Слайдер отображения прогресса уровня
    /// </summary>
    public Slider LevelProgressSlider;

    #endregion

    #region Методы Unity

    /// <summary>
    /// Вызывается при старте компонента, инициализирует слайдеры и подписывается на события
    /// </summary>
    private void Start()
    {
      CastleHPSlider.maxValue = GameSettingsManager.GetInstance().Settings.PlayerSettings.CastleMaxHealth;
      LevelProgressSlider.maxValue = GameSettingsManager.GetInstance().Settings.LevelMaxValue;
      
      // ReSharper disable once HeapView.DelegateAllocation
      CastleController.GetInstance().OnHealthChanged.AddListener(OnCastleHPChanged);
      // ReSharper disable once HeapView.DelegateAllocation
      GameManager.GetInstance().OnLevelProgressChanged.AddListener(OnLevelProgressChanged);
    }

    #endregion

    #region Методы обработки событий

    /// <summary>
    /// Обрабатывает изменение здоровья замка
    /// </summary>
    /// <param name="value">Новое значение здоровья</param>
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
    
    /// <summary>
    /// Обрабатывает изменение прогресса уровня
    /// </summary>
    /// <param name="value">Новое значение прогресса</param>
    private void OnLevelProgressChanged(float value)
    {
      LevelProgressSlider.value = value;
    }

    #endregion
  }
}
