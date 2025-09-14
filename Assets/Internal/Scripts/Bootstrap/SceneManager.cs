using System;
using UnityEngine;

namespace Internal.Scripts.Bootstrap
{
  public class SceneManager : MonoBehaviour
  {
    private const string GameSceneName = "Game";
    
    #region Public methods

    public async void LoadGameSceneAsync()
    {
      try
      {
        await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GameSceneName);
      }
      catch (Exception e)
      {
        throw new Exception($"Failed to load scene {GameSceneName}", e);
      }
    }

    #endregion
    
    #region Singleton

    private static SceneManager _instance;
    public static SceneManager GetInstance()
    {
      _instance ??= FindFirstObjectByType<SceneManager>();
      return _instance;
    }
    
    #endregion

    #region Unity Methods

    private void Awake()
    {
      DontDestroyOnLoad(gameObject);
    }
    
    #endregion
  }
}
