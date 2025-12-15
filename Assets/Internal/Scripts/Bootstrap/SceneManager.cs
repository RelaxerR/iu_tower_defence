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

    /// <summary>
    /// Загружает сцену по имени
    /// </summary>
    /// <param name="sceneName">Имя сцены для загрузки</param>
    /// <exception cref="Exception"></exception>
    public async void LoadScene(string sceneName)
    {
      try
      {
        await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
      }
      catch (Exception e)
      {
        // ReSharper disable once AsyncVoidMethod
        throw new Exception($"Не удалось загрузить сцену {sceneName}", e);
      }
    }

    #endregion
    
    #region Синглтон

    private static SceneManager _instance;

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Возвращает экземпляр менеджера сцен, создавая его при необходимости
    /// </summary>
    /// <returns>Единственный экземпляр SceneManager в сцене</returns>
    public static SceneManager GetInstance()
    {
      if (_instance)
        return _instance;
      _instance = FindFirstObjectByType<SceneManager>();
      if (_instance)
        return _instance;
      // Если не найден на сцене, создаем пустой объект
      var sceneManagerObj = new GameObject("SceneManager");
      _instance = sceneManagerObj.AddComponent<SceneManager>();
      return _instance;
    }
    
    #endregion

    #region Методы Unity

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
    
    #endregion
  }
}