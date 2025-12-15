using UnityEngine;
using Internal.Scripts.Controllers.Enemies;

namespace Internal.Scripts.Controllers.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float speed = 10f;           // Скорость движения снаряда
        [SerializeField] private int damage = 10;             // Урон, наносимый врагу
        [SerializeField] private float maxLifetime = 5f;      // Максимальное время жизни (на случай, если не попал)

        private Transform target = null;                      // Цель, к которой движется снаряд
        private bool hasHit = false;                          // Флаг, чтобы избежать повторного нанесения урона

        private float lifeTimer = 0f;                         // Таймер жизни

        // Метод для установки цели
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void Update()
        {
            lifeTimer += Time.deltaTime;

            if (lifeTimer >= maxLifetime)
            {
                Destroy(gameObject);
                return;
            }

            if (target == null)
            {
                // Цель пропала, уничтожаем снаряд
                Destroy(gameObject);
                return;
            }

            // Двигаем снаряд в сторону цели
            Vector3 direction = (target.position - transform.position + new Vector3(0, 1f, 0)).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Поворачиваем снаряд в сторону движения (по желанию, для визуального эффекта)
            transform.forward = direction;
        }

        // Вызывается при столкновении с врагом
        private void OnTriggerEnter(Collider other)
        {
            if (hasHit) return; // Уже попали, выходим

            // Проверяем, является ли объект врагом
            IEnemy enemy = other.GetComponent<IEnemy>();
            if (enemy != null)
            {
                // Наносим урон врагу (предполагается, что IEnemy имеет метод TakeDamage)
                enemy.TakeDamage(damage);
                Debug.Log($"[Projectile] Hit {other.name} for {damage} damage.", this);

                hasHit = true;
                // Уничтожаем снаряд после попадания
                Destroy(gameObject);
            }
        }

        // Если вы используете 2D физику, замените OnTriggerEnter на OnTriggerStay2D или OnTriggerEnter2D
        // private void OnTriggerEnter2D(Collider2D other) { ... }
    }
}