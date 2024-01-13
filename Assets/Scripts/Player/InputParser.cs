using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


public class InputParser : MonoBehaviour
{
	[Header("Character Input Values")]
	public Vector2 move;
	public Vector2 look;
	public bool jump;
	public bool sprint;

	[Header("Movement Settings")]
	public bool analogMovement;

	[Header("Mouse Cursor Settings")]
	public bool cursorLocked = true;
	public bool cursorInputForLook = true;

	PlayerInput _playerInput;

	public bool IsCurrentDeviceMouse
    {
        get
        {
            return _playerInput.currentControlScheme == "KeyboardMouse";
        }
    }


	private void Awake() {
		
		_playerInput = GetComponent<PlayerInput>();
		Cursor.lockState = CursorLockMode.Locked;

	}
	public void OnMove(InputAction.CallbackContext value)
	{
		MoveInput(value.ReadValue<Vector2>());
	}

	public void OnLook(InputAction.CallbackContext  value)
	{
		if(cursorInputForLook)
		{
			LookInput(value.ReadValue<Vector2>());
		}
	}

	public void OnJump(InputAction.CallbackContext  value)
	{
		JumpInput(value.performed);
	}

	public void OnSprint(InputAction.CallbackContext value)
	{
		SprintInput(value.phase.IsInProgress());
	}


	public void MoveInput(Vector2 newMoveDirection)
	{
		move = newMoveDirection;
	} 

	public void LookInput(Vector2 newLookDirection)
	{
		look = newLookDirection;
	}

	public void JumpInput(bool newJumpState)
	{
		jump = newJumpState;
	}

	public void SprintInput(bool newSprintState)
	{
		sprint = newSprintState;
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		SetCursorState(cursorLocked);
	}

	private void SetCursorState(bool newState)
	{
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}
}
