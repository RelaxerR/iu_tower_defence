#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoadAttribute]
public static class DefaultSceneLoader
{
  static DefaultSceneLoader(){
    EditorApplication.playModeStateChanged += LoadDefaultScene;
  }

  private static void LoadDefaultScene(PlayModeStateChange state)
  {
    switch (state)
    {
      case PlayModeStateChange.ExitingEditMode:
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ();
        break;
      case PlayModeStateChange.EnteredPlayMode:
        SceneManager.LoadScene (0);
        break;
      case PlayModeStateChange.EnteredEditMode:
      case PlayModeStateChange.ExitingPlayMode:
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(state), state, null);
    }

  }
}
#endif
