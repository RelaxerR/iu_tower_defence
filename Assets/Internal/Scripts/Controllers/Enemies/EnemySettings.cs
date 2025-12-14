using UnityEngine;

namespace Internal.Scripts.Controllers.Enemies
{
  [CreateAssetMenu(fileName = "Enemy Settings", menuName = "Settings", order = 3)]
  public class EnemySettings : ScriptableObject
  {
    public GameObject prefab;
    public float DamageAmount;
    public float Speed;
    public float MaxHealth;
  }
}
