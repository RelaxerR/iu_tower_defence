using Internal.Scripts.Bootstrap;
using Internal.Scripts.Bootstrap.Settings;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Internal.Scripts.Controllers.Player
{
  /// <summary>
  /// Класс, управляющий движением игрока с использованием джойстика и ввода
  /// </summary>
  [RequireComponent(typeof(Rigidbody))]
  [RequireComponent(typeof(PlayerInput))]
  public class MovementController : MonoBehaviour
  {
    #region Поля

    [SerializeField] 
    private VariableJoystick joystick;
    
    private static PlayerSettings settings
    {
      get => GameSettingsManager.GetInstance().Settings.PlayerSettings;
    }
    
    private Vector2 moveInput;

    #endregion

    #region Методы ввода

    /// <summary>
    /// Обрабатывает событие движения от системы ввода
    /// </summary>
    /// <param name="context">Контекст обратного вызова действия ввода</param>
    public void OnMove(InputAction.CallbackContext context)
    {
      moveInput = context.ReadValue<Vector2>();
    }

    #endregion

    #region Методы Unity

    /// <summary>
    /// Вызывается каждый кадр, обновляет позицию игрока на основе ввода
    /// </summary>
    private void Update()
    {
      var moveVector = new Vector3(moveInput.x, 0, moveInput.y);
      transform.Translate(moveVector * (Time.deltaTime * settings.MoveSpeed), Space.World);
      
      var direction = Vector3.forward * joystick.Vertical + Vector3.right * joystick.Horizontal;
      transform.Translate(direction * (Time.deltaTime * settings.MoveSpeed), Space.World);
    }

    #endregion
  }
}
