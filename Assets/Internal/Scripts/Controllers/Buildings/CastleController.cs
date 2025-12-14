using UnityEngine;

namespace Internal.Scripts.Controllers.Buildings
{
  public class CastleController : MonoBehaviour
  {
    private float currentHealth = 1000;
    
    public void TakeDamage(float damageAmount)
    {
      currentHealth -= damageAmount;
      Debug.Log($"Castle took {damageAmount} damage, current health: {currentHealth}");
      if (currentHealth <= 0)
      {
        Debug.Log("Castle has been destroyed!");
        // Add destruction logic here
      }
    }
    public Vector3 GetPosition()
    {
      return transform.position;
    }
  }
}
