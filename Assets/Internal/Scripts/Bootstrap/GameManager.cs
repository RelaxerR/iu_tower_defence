using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace Internal.Scripts.Bootstrap
{
  /// <summary>
  /// Менеджер управления состоянием игры и прогрессом уровня
  /// </summary>
  public class GameManager : MonoBehaviour
  {
    [CanBeNull]
    private static GameManager Instance;

    /// <summary>
    /// Возвращает экземпляр менеджера игры, создавая его при необходимости
    /// </summary>
    /// <returns>Единственный экземпляр GameManager в сцене</returns>
    public static GameManager GetInstance()
    {
      Instance ??= FindFirstObjectByType<GameManager>();
      return Instance;
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
    /// Вызывается при старте компонента, инициализирует корутину прогресса уровня
    /// </summary>
    private void Start()
    {
      var levelMaxValue = GameSettingsManager.GetInstance().Settings.LevelMaxValue;
      var levelDuration = GameSettingsManager.GetInstance().Settings.LevelDurationSeconds;
      StartCoroutine(LevelProgressCoroutine(levelMaxValue, levelDuration));
    }

    /// <summary>
    /// Перечисление возможных состояний игры
    /// </summary>
    public enum GameState
    {
      /// <summary>
      /// Меню игры
      /// </summary>
      Menu,
      
      /// <summary>
      /// Игровой процесс
      /// </summary>
      Game,
      
      /// <summary>
      /// Пауза в игре
      /// </summary>
      Paused,
      
      /// <summary>
      /// Победа игрока
      /// </summary>
      GameWin,
      
      /// <summary>
      /// Поражение игрока
      /// </summary>
      GameLoose
    }
    
    /// <summary>
    /// Событие, вызываемое при изменении состояния игры
    /// </summary>
    public UnityEvent<GameState> OnGameStateChanged;
    
    private GameState currentGameState = GameState.Menu;
    
    /// <summary>
    /// Текущее состояние игры
    /// </summary>
    public GameState CurrentGameState
    {
      set
      {
        if (currentGameState == value) return;
        currentGameState = value;
        OnGameStateChanged?.Invoke(currentGameState);
        
        // Загружаем сцену Bootstrap при достижении победы или поражения
        if (currentGameState != GameState.GameWin && currentGameState != GameState.GameLoose)
          return;
        var win = currentGameState == GameState.GameWin;
        
        var level = PlayerPrefs.HasKey("Level") ? PlayerPrefs.GetInt("Level") : 1;
        if (win) level++;
        PlayerPrefs.SetInt("Level", level);
        
        PlayerPrefs.SetString("GameResult", win ? "Win" : "Loose");
        OnGameResult?.Invoke(win);
        // Загружаем сцену Bootstrap после небольшой задержки для отображения результата
        StartCoroutine(LoadBootstrapSceneAfterDelay());
      }
    }

    /// <summary>
    /// Событие, вызываемое при достижении результата игры (победа или поражение)
    /// </summary>
    public UnityEvent<bool> OnGameResult;

    /// <summary>
    /// Запускает игровой процесс
    /// </summary>
    public void StartGame()
    {
      CurrentGameState = GameState.Game;
      SceneManager.GetInstance().LoadGameSceneAsync();
    }

    /// <summary>
    /// Корутина, управляющая прогрессом уровня со временем
    /// </summary>
    /// <param name="targetProgress">Целевое значение прогресса</param>
    /// <param name="duration">Продолжительность прогресса в секундах</param>
    /// <returns>IEnumerator для корутины</returns>
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

      // По истечении времени завершаем игру победой
      OnLevelProgressChanged?.Invoke(targetProgress);
      CurrentGameState = GameState.GameWin;
    }

    /// <summary>
    /// Загружает сцену Bootstrap с небольшой задержкой
    /// </summary>
    /// <returns>IEnumerator для корутины</returns>
    private IEnumerator LoadBootstrapSceneAfterDelay()
    {
      // Небольшая задержка, чтобы игрок успел увидеть результат
      yield return new WaitForSeconds(1.5f);
      
      // Загружаем сцену Bootstrap
      SceneManager.GetInstance().LoadScene("GameResult");
    }

    /// <summary>
    /// Событие, вызываемое при изменении прогресса уровня
    /// </summary>
    public UnityEvent<float> OnLevelProgressChanged;
  }
}