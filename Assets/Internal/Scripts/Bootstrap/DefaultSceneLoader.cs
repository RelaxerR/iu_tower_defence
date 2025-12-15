#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Автоматически загружает начальный сценарий при запуске игры в редакторе
/// </summary>
[InitializeOnLoadAttribute]
public static class DefaultSceneLoader
{
  static DefaultSceneLoader()
  {
    EditorApplication.playModeStateChanged += LoadDefaultScene;
  }

  /// <summary>
  /// Обрабатывает изменение состояния режима воспроизведения и выполняет соответствующие действия
  /// </summary>
  /// <param name="state">Новое состояние режима воспроизведения</param>
  private static void LoadDefaultScene(PlayModeStateChange state)
  {
    switch (state)
    {
      case PlayModeStateChange.ExitingEditMode:
        // Сохраняем измененные сцены перед выходом из режима редактирования
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        break;
      
      case PlayModeStateChange.EnteredPlayMode:
        // Загружаем начальный сценарий (индекс 0) при входе в режим воспроизведения
        SceneManager.LoadScene(0);
        break;
      
      case PlayModeStateChange.EnteredEditMode:
      case PlayModeStateChange.ExitingPlayMode:
        // Ничего не делаем при переходе в режим редактирования или выходе из режима воспроизведения
        break;
      
      default:
        // Бросаем исключение при получении неожиданного значения состояния
        throw new ArgumentOutOfRangeException(nameof(state), state, null);
    }
  }
}
#endif
