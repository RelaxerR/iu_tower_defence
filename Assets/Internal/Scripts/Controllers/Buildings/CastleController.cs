using System;
using Internal.Scripts.Bootstrap;
using UnityEngine;
using UnityEngine.Events;

namespace Internal.Scripts.Controllers.Buildings
{
  /// <summary>
  /// Контроллер замка, управляющий его здоровьем и реакцией на повреждения
  /// </summary>
  public class CastleController : MonoBehaviour
  {
    #region Синглтон

    private static CastleController _instance;

    /// <summary>
    /// Возвращает экземпляр контроллера замка, создавая его при необходимости
    /// </summary>
    /// <returns>Единственный экземпляр CastleController в сцене</returns>
    public static CastleController GetInstance()
    {
      _instance ??= FindFirstObjectByType<CastleController>();
      return _instance;
    }

    #endregion

    #region Поля и события

    /// <summary>
    /// Событие, вызываемое при изменении здоровья замка
    /// </summary>
    public UnityEvent<float> OnHealthChanged = new();
    
    private float currentHealth = 1000;

    #endregion

    #region Методы Unity

    /// <summary>
    /// Вызывается при старте компонента, инициализирует здоровье замка
    /// </summary>
    private void Start()
    {
      currentHealth = GameSettingsManager.GetInstance().Settings.PlayerSettings.CastleMaxHealth;
      OnHealthChanged.Invoke(currentHealth);
    }

    #endregion

    #region Методы управления здоровьем

    /// <summary>
    /// Применяет урон к замку
    /// </summary>
    /// <param name="damageAmount">Количество урона</param>
    public void TakeDamage(float damageAmount)
    {
      currentHealth -= damageAmount;
      OnHealthChanged.Invoke(currentHealth);
      
      if (currentHealth <= 0)
      {
        Die();
      }
    }

    /// <summary>
    /// Уничтожает замок и завершает игру поражением
    /// </summary>
    private void Die()
    {
      // Замок уничтожен - завершаем игру поражением
      GameManager.GetInstance().CurrentGameState = GameManager.GameState.GameLoose;
      Destroy(gameObject);
    }

    #endregion
  }
}