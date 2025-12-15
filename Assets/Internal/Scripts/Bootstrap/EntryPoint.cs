using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Internal.Scripts.Bootstrap
{
  /// <summary>
  /// Точка входа в игру, обеспечивающая инициализацию основных менеджеров
  /// </summary>
  public class EntryPoint : MonoBehaviour
  {
    private static EntryPoint _instance;

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

    /// <summary>
    /// Вызывается при старте компонента, инициализирует основные менеджеры игры
    /// </summary>
    private void Start()
    {
      // Инициализируем менеджеры игры
      GameSettingsManager.GetInstance();
      TileManager.GetInstance();
    }
  }
}
