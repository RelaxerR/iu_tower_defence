using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Internal.Scripts.Bootstrap
{
  public class EntryPoint : MonoBehaviour
  {
    private static EntryPoint _instance;
    public static EntryPoint GetInstance()
    {
      _instance ??= FindFirstObjectByType<EntryPoint>();
      return _instance;
    }
    
    private void Awake()
    {
      DontDestroyOnLoad(this.gameObject);
    }
    
    void Start()
    {
      //TODO: Load resources etc.
      SceneManager.GetInstance().LoadGameScene();
    }
  }
}
