using UnityEngine;
using Internal.Scripts.Controllers.Buildings;

namespace Internal.Scripts.Controllers.Enemies
{
  public interface IEnemy
  {
    public EnemySettings GetSettings();
    public void Attack();
    public void TakeDamage(float damage);
    public void Die();
    public void Move();
    public void SetTargetCastle(CastleController castle);
  }
}
