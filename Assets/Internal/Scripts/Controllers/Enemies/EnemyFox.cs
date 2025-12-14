using Internal.Scripts.Controllers.Buildings;
using UnityEngine;

namespace Internal.Scripts.Controllers.Enemies
{
  public class EnemyFox : MonoBehaviour, IEnemy
  {
    private EnemySettings _settings;
    public EnemySettings GetSettings() => _settings;
    
    private float _health;

    public void Spawn(Vector3 spawnPosition, Transform parent)
    {
      Instantiate(_settings.prefab, spawnPosition, Quaternion.identity, parent);
    }
    public void Attack(IBuilding building)
    {
      building.TakeDamage(_settings.DamageAmount);
    }
    public void TakeDamage(float damage)
    {
      _health -= damage;
      if (_health <= 0)
      {
        Die();
      }
    }
    public void Die()
    {
      Destroy(gameObject);
    }
  }
}
