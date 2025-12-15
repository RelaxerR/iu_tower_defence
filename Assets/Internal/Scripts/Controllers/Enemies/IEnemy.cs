using Internal.Scripts.Controllers.Buildings;

namespace Internal.Scripts.Controllers.Enemies
{
  /// <summary>
  /// Интерфейс для врагов, определяющий основные методы поведения врага
  /// </summary>
  public interface IEnemy
  {
    /// <summary>
    /// Возвращает настройки врага
    /// </summary>
    /// <returns>Настройки врага</returns>
    EnemySettings GetSettings();

    /// <summary>
    /// Атакует цель (обычно замок)
    /// </summary>
    void Attack();

    /// <summary>
    /// Применяет урон к врагу
    /// </summary>
    /// <param name="damage">Количество урона</param>
    void TakeDamage(float damage);

    /// <summary>
    /// Уничтожает врага
    /// </summary>
    void Die();

    /// <summary>
    /// Устанавливает замок в качестве цели для врага
    /// </summary>
    /// <param name="castle">Замок, который станет целью</param>
    void SetTargetCastle(CastleController castle);
  }
}
