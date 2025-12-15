using UnityEngine;
using Internal.Scripts.Controllers.Enemies;

namespace Internal.Scripts.Controllers.Projectiles
{
  /// <summary>
  /// Класс снаряда, управляющий движением снаряда к цели и нанесением урона при попадании
  /// </summary>
  public class Projectile : MonoBehaviour
  {
    #region Поля настроек

    [Header("Настройки снаряда")]
    [SerializeField] 
    private float speed = 10f;           // Скорость движения снаряда
    
    [SerializeField] 
    private int damage = 10;             // Урон, наносимый врагу
    
    [SerializeField] 
    private float maxLifetime = 5f;      // Максимальное время жизни (на случай, если не попал)

    #endregion

    #region Поля состояния

    private Transform target = null;     // Цель, к которой движется снаряд
    private bool hasHit = false;         // Флаг, чтобы избежать повторного нанесения урона
    private float lifeTimer = 0f;        // Таймер жизни

    #endregion

    #region Методы управления

    /// <summary>
    /// Устанавливает цель для снаряда
    /// </summary>
    /// <param name="newTarget">Новая цель для снаряда</param>
    public void SetTarget(Transform newTarget)
    {
      target = newTarget;
    }

    #endregion

    #region Методы Unity

    /// <summary>
    /// Обновляет состояние снаряда каждый кадр
    /// </summary>
    private void Update()
    {
      lifeTimer += Time.deltaTime;

      if (lifeTimer >= maxLifetime)
      {
        Destroy(gameObject);
        return;
      }

      if (!target)
      {
        // Цель пропала, уничтожаем снаряд
        Destroy(gameObject);
        return;
      }

      // Двигаем снаряд в сторону цели
      var direction = (target.position - transform.position + new Vector3(0, 1f, 0)).normalized;
      transform.position += direction * (speed * Time.deltaTime);

      // Поворачиваем снаряд в сторону движения (по желанию, для визуального эффекта)
      transform.forward = direction;
    }

    /// <summary>
    /// Вызывается при столкновении с врагом
    /// </summary>
    /// <param name="other">Объект, с которым произошло столкновение</param>
    private void OnTriggerEnter(Collider other)
    {
      if (hasHit) return; // Уже попали, выходим

      // Проверяем, является ли объект врагом
      var enemy = other.GetComponent<IEnemy>();
      if (enemy == null)
        return;
      // Наносим урон врагу (предполагается, что IEnemy имеет метод TakeDamage)
      enemy.TakeDamage(damage);

      hasHit = true;
      // Уничтожаем снаряд после попадания
      Destroy(gameObject);
    }

    #endregion
  }
}