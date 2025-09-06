using System;
using Internal.Scripts.Bootstrap.Settings;
using UnityEngine;

namespace Internal.Scripts.Bootstrap
{
  public class GameSettingsManager : MonoBehaviour
  {
    [SerializeField]
    private GameSettings gameSettings;
    public GameSettings Settigns
    {
      get => gameSettings;
    }

    private static GameSettingsManager _instance;
    public static GameSettingsManager GetInstance()
    {
      _instance ??= FindFirstObjectByType<GameSettingsManager>();
      return _instance;
    }

    private void Awake()
    {
      DontDestroyOnLoad(this.gameObject);
    }
  }
}
