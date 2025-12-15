using System;
using UnityEngine;

namespace Internal.Scripts.Bootstrap
{
  /// <summary>
  /// Менеджер управления сценами в игре
  /// </summary>
  public class SceneManager : MonoBehaviour
  {
    private const string GameSceneName = "Game";
    
    #region Публичные методы

    /// <summary>
    /// Асинхронно загружает игровую сцену
    /// </summary>
    /// <exception cref="Exception"></exception>
    public async void LoadGameSceneAsync()
    {
      try
      {
        await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GameSceneName);
      }
      catch (Exception e)
      {
        // ReSharper disable once AsyncVoidMethod
        throw new Exception($"Не удалось загрузить сцену {GameSceneName}", e);
      }
    }

    #endregion
    
    #region Синглтон

    private static SceneManager _instance;

    /// <summary>
    /// Возвращает экземпляр менеджера сцен, создавая его при необходимости
    /// </summary>
    /// <returns>Единственный экземпляр SceneManager в сцене</returns>
    public static SceneManager GetInstance()
    {
      _instance ??= FindFirstObjectByType<SceneManager>();
      return _instance;
    }
    
    #endregion

    #region Методы Unity

    /// <summary>
    /// Вызывается при активации объекта, обеспечивает сохранение между сценами
    /// </summary>
    private void Awake()
    {
      // Не уничтожаем объект при загрузке новой сцены
      DontDestroyOnLoad(gameObject);
    }
    
    #endregion
  }
}
