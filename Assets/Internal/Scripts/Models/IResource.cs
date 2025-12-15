namespace Internal.Scripts.Models
{
  /// <summary>
  /// Интерфейс для ресурсов, определяющий основные методы взаимодействия с ресурсами
  /// </summary>
  public interface IResource
  {
    /// <summary>
    /// Возвращает идентификатор ресурса
    /// </summary>
    string Id { get; }
    
    /// <summary>
    /// Возвращает или устанавливает количество ресурса
    /// </summary>
    int Amount { get; set; }
    
    /// <summary>
    /// Собирает ресурс, уменьшая его количество
    /// </summary>
    void Collect();
    
    /// <summary>
    /// Вызывается при выборе ресурса
    /// </summary>
    void OnSelected();
    
    /// <summary>
    /// Вызывается при отмене выбора ресурса
    /// </summary>
    void OnDeselected();
    
    /// <summary>
    /// Уничтожает объект ресурса
    /// </summary>
    void DestroySelf();
  }
}
