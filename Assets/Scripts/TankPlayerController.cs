using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Reflection;

public class TankPlayerController : MonoBehaviour
{
	public void Reset()
	{
		Debug.Log($"{MethodBase.GetCurrentMethod().Name}");
		// @TODO
	}

	public void Awake()
	{
		Debug.Log(MethodBase.GetCurrentMethod().Name);
	}

	public void Start()
	{
		Debug.Log(MethodBase.GetCurrentMethod().Name);
	}

	#region Input actions
	public void InputAction_RotateGun(InputAction.CallbackContext context)
	{
		float axisValue = context.ReadValue<float>();
		Debug.Log($"{MethodBase.GetCurrentMethod().Name}: {nameof(axisValue)}={axisValue}");
		// @TODO
	}
	public void InputAction_FireGun(InputAction.CallbackContext context)
	{
		Debug.Log(MethodBase.GetCurrentMethod().Name);
		// @TODO
	}
	public void InputAction_AltFireGun(InputAction.CallbackContext context)
	{
		Debug.Log(MethodBase.GetCurrentMethod().Name);
		// @TODO
	}
	public void InputAction_Rotate(InputAction.CallbackContext context)
	{
		float axisValue = context.ReadValue<float>();
		Debug.Log($"{MethodBase.GetCurrentMethod().Name}: {nameof(axisValue)}={axisValue}");
		// @TODO
	}
	public void InputAction_Thrust(InputAction.CallbackContext context)
	{
		float axisValue = context.ReadValue<float>();
		Debug.Log($"{MethodBase.GetCurrentMethod().Name}: {nameof(axisValue)}={axisValue}");
		// @TODO
	}
	#endregion Input actions
}
