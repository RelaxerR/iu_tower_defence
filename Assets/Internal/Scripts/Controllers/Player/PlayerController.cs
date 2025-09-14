using System;
using System.Linq.Expressions;
using Internal.Scripts.Bootstrap;
using Internal.Scripts.Bootstrap.Settings;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Internal.Scripts.Controllers.Player
{
  [RequireComponent(typeof(Rigidbody))]
  [RequireComponent(typeof(PlayerInput))]
  public class PlayerController : MonoBehaviour
  {
    [SerializeField]
    private VariableJoystick joystick;
    
    private static PlayerSettings settings
    {
      get => GameSettingsManager.GetInstance().Settings.PlayerSettings;
    }
    
    private Vector2 moveInput;

    public void OnMove(InputAction.CallbackContext context)
    {
      moveInput = context.ReadValue<Vector2>();
    }
    
    public void OnAttack(InputAction.CallbackContext context)
    {
      if (context.performed)
      {
        Debug.Log("Attack");
      }
    }

    private void Update()
    {
      var moveVector = new Vector3(moveInput.x, 0, moveInput.y);
      transform.Translate(moveVector * (Time.deltaTime * settings.MoveSpeed));
      
      var direction = Vector3.forward * joystick.Vertical + Vector3.right * joystick.Horizontal;
      transform.Translate(direction * (Time.deltaTime * settings.MoveSpeed));
    }
  }
}
