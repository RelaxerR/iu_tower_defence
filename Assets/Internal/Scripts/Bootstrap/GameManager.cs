using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace Internal.Scripts.Bootstrap
{
  public class GameManager : MonoBehaviour
  {
    [CanBeNull]
    private static GameManager Instance;
    public static GameManager GetInstance()
    {
      Instance ??= FindFirstObjectByType<GameManager>();
      return Instance;
    }
    
    private void Awake()
    {
      DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
      var levelMaxValue = GameSettingsManager.GetInstance().Settings.LevelMaxValue;
      var levelDuration = GameSettingsManager.GetInstance().Settings.LevelDurationSeconds;
      StartCoroutine(LevelProgressCoroutine(levelMaxValue, levelDuration));
    }

    public enum GameState
    {
      Menu,
      Game,
      Paused,
      GameWin,
      GameLoose
    }
    
    public UnityEvent<GameState> OnGameStateChanged;
    private GameState currentGameState = GameState.Menu;
    
    public GameState CurrentGameState
    {
      get => currentGameState;
      set
      {
        if (currentGameState == value) return;
        currentGameState = value;
        OnGameStateChanged?.Invoke(currentGameState);
        Debug.Log($"Game state changed to: {currentGameState}");
      }
    }

    public void StartGame()
    {
      CurrentGameState = GameState.Game;
      SceneManager.GetInstance().LoadGameSceneAsync();
    }

    private IEnumerator LevelProgressCoroutine(float targetProgress, float duration)
    {
      const float startProgress = 0f;
      var elapsed = 0f;

      while (elapsed < duration)
      {
        elapsed += Time.deltaTime;
        var currentProgress = Mathf.Lerp(startProgress, targetProgress, elapsed / duration);
        OnLevelProgressChanged?.Invoke(currentProgress);
        yield return null;
      }

      OnLevelProgressChanged?.Invoke(targetProgress);
      CurrentGameState = GameState.GameWin;
    }

    public UnityEvent<float> OnLevelProgressChanged;
  }
}
