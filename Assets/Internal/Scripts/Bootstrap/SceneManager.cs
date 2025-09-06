using UnityEngine;

namespace Internal.Scripts.Bootstrap
{
  public class SceneManager : MonoBehaviour
  {
    private const string GameSceneName = "Game";
    
    #region Public methods

    public void LoadGameScene()
    {
      UnityEngine.SceneManagement.SceneManager.LoadScene(GameSceneName);
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
      DontDestroyOnLoad(this.gameObject);
    }
    
    #endregion
  }
}
