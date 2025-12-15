using UnityEngine;

namespace Internal.Scripts.Controllers.Enemies
{
  /// <summary>
  /// Настройки врага, содержащие параметры для создания и поведения врага
  /// </summary>
  [CreateAssetMenu(fileName = "Enemy Settings", menuName = "Settings/Enemy Settings", order = 3)]
  public class EnemySettings : ScriptableObject
  {
    /// <summary>
    /// Количество урона, наносимого врагом при атаке
    /// </summary>
    [Tooltip("Количество урона, наносимого врагом при атаке")]
    public float DamageAmount;

    /// <summary>
    /// Максимальное здоровье врага
    /// </summary>
    [Tooltip("Максимальное здоровье врага")]
    public float MaxHealth;
  }
}
