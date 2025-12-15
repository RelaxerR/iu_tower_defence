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
      DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
      GameSettingsManager.GetInstance();
      TileManager.GetInstance();
    }
  }
}
