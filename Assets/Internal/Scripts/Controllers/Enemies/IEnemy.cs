using UnityEngine;
using Internal.Scripts.Controllers.Buildings;

namespace Internal.Scripts.Controllers.Enemies
{
  public interface IEnemy
  {
    public EnemySettings GetSettings();
    public void Spawn(Vector3 spawnPosition, Transform parent);
    public void Attack(IBuilding building);
    public void TakeDamage(float damage);
    public void Die();
  }
}
