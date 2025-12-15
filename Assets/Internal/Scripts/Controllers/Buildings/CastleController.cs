using System;
using Internal.Scripts.Bootstrap;
using UnityEngine;
using UnityEngine.Events;

namespace Internal.Scripts.Controllers.Buildings
{
  public class CastleController : MonoBehaviour
  {
    private static CastleController _instance;
    public static CastleController GetInstance()
    {
      _instance ??= FindFirstObjectByType<CastleController>();
      return _instance;
    }
    
    public UnityEvent<float> OnHealthChanged = new UnityEvent<float>();
    
    private float currentHealth = 1000;

    private void Start()
    {
      currentHealth = GameSettingsManager.GetInstance().Settings.PlayerSettings.CastleMaxHealth;
      OnHealthChanged.Invoke(currentHealth);
    }

    public void TakeDamage(float damageAmount)
    {
      currentHealth -= damageAmount;
      OnHealthChanged.Invoke(currentHealth);
      // Debug.Log($"Castle took {damageAmount} damage, current health: {currentHealth}");
      if (!(currentHealth <= 0)) return;
      
      // Debug.Log("Castle has been destroyed!");
      GameManager.GetInstance().CurrentGameState = GameManager.GameState.GameLoose;
      Destroy(gameObject);
    }
  }
}
