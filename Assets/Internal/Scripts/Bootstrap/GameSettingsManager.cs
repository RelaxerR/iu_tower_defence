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

    /// <summary>
    /// Возвращает экземпляр менеджера настроек игры, создавая его при необходимости
    /// </summary>
    /// <returns>Единственный экземпляр GameSettingsManager в сцене</returns>
    public static GameSettingsManager GetInstance()
    {
      _instance ??= FindFirstObjectByType<GameSettingsManager>();
      return _instance;
    }

    /// <summary>
    /// Вызывается при активации объекта, обеспечивает сохранение между сценами
    /// </summary>
    private void Awake()
    {
      // Не уничтожаем объект при загрузке новой сцены
      DontDestroyOnLoad(gameObject);
    }
  }
}
