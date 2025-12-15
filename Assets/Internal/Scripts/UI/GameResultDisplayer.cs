using System;
using System.Collections;
using Internal.Scripts.Bootstrap;
using TMPro;
using UnityEngine;
// ReSharper disable HeapView.DelegateAllocation

namespace Internal.Scripts.UI
{
  /// <summary>
  /// Класс, отображающий результат игры (победа/поражение) в текстовом поле
  /// </summary>
  public class GameResultDisplayer : MonoBehaviour
  {
    #region Поля

    [SerializeField]
    private TMP_Text _resultText;

    #endregion

    #region Unity

    private void Start()
    {
      var result = false;
      if (!PlayerPrefs.HasKey("GameResult")) result = false;
      
      var gameRes = PlayerPrefs.GetString("GameResult");
      result = gameRes == "Win";
      OnGameResultChanged(result);
      StartCoroutine(LoadBootstrapSceneAfterDelay());
    }

    #endregion

    #region Методы обработки событий

    /// <summary>
    /// Обрабатывает изменение результата игры
    /// </summary>
    /// <param name="win">true - победа, false - поражение</param>
    private void OnGameResultChanged(bool win)
    {
      _resultText.text = win switch
      {
        true => "Победа",
        false => "Поражение",
      };
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
      SceneManager.GetInstance().LoadScene("Bootstrap");
    }
    
    #endregion
  }
}
