using Internal.Scripts.Bootstrap.Settings;
using TMPro;
using UnityEngine;

namespace Internal.Scripts.UI
{
  /// <summary>
  /// Класс, отображающий количество ресурса в текстовом поле
  /// </summary>
  [RequireComponent(typeof(TMP_Text))]
  public class ResourceDisplayer : MonoBehaviour
  {
    #region Поля

    [SerializeField]
    private ResourceSettings _resourceSettings;
    
    private TMP_Text _text;

    #endregion

    #region Методы Unity

    /// <summary>
    /// Вызывается при старте компонента, получает компонент текста
    /// </summary>
    private void Start()
    {
      _text = GetComponent<TMP_Text>();
    }

    #endregion

    #region Методы обновления

    /// <summary>
    /// Обрабатывает изменение количества ресурса
    /// </summary>
    /// <param name="resourceId">Идентификатор ресурса</param>
    /// <param name="newAmount">Новое количество ресурса</param>
    public void OnResourceAmountChanged(string resourceId, int newAmount)
    {
      if (resourceId != _resourceSettings.Id) return;
      if (_text.text != $"{newAmount}") // Избегаем ненужных обновлений
        _text.text = $"{newAmount}";
    }

    #endregion
  }
}
