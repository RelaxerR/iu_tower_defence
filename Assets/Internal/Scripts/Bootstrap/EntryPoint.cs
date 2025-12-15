using UnityEngine;

namespace Internal.Scripts.Bootstrap
{
  /// <summary>
  /// Точка входа в игру, обеспечивающая инициализацию основных менеджеров
  /// </summary>
  public class EntryPoint : MonoBehaviour
  {
    private static EntryPoint _instance;

    /// <summary>
    /// Возвращает экземпляр точки входа, создавая его при необходимости
    /// </summary>
    /// <returns>Единственный экземпляр EntryPoint в сцене</returns>
    public static EntryPoint GetInstance()
    {
      _instance ??= FindFirstObjectByType<EntryPoint>();
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
