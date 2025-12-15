using System;
using Internal.Scripts.Bootstrap.Settings;
using UnityEngine;

namespace Internal.Scripts.Bootstrap
{
  /// <summary>
  /// Менеджер управления настройками игры
  /// </summary>
  public class GameSettingsManager : MonoBehaviour
  {
    [SerializeField]
    private GameSettings gameSettings;

    /// <summary>
    /// Получает текущие настройки игры
    /// </summary>
    public GameSettings Settings
    {
      get => gameSettings;
    }

    private static GameSettingsManager _instance;
    
    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Возвращает экземпляр менеджера настроек игры, создавая его при необходимости
    /// </summary>
    /// <returns>Единственный экземпляр GameSettingsManager в сцене</returns>
    public static GameSettingsManager GetInstance()
    {
      if (_instance)
        return _instance;
      _instance = FindFirstObjectByType<GameSettingsManager>();
      if (_instance)
        return _instance;
      // Если не найден на сцене, создаем пустой объект
      var settingsManagerObj = new GameObject("GameSettingsManager");
      _instance = settingsManagerObj.AddComponent<GameSettingsManager>();
      return _instance;
    }

    /// <summary>
    /// Вызывается при активации объекта, обеспечивает сохранение между сценами
    /// </summary>
    private void Awake()
    {
      if (!_instance)
      {
        _instance = this;
        DontDestroyOnLoad(gameObject);
      }
      else if (_instance != this)
      {
        // Уничтожаем дубликат
        Destroy(gameObject);
      }
    }
  }
}
