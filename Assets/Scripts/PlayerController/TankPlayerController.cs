using UnityEngine;
using UnityEngine.InputSystem;

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

[RequireComponent(typeof(PlayerInput))]
public class TankPlayerController : MonoBehaviour
{
	const string PlayerTag = "Player";

	IDamageableGUIComponent DebugDamageableGUI;

	ControllableTank ControllableTank;
	IDamageableComponent Damageable;

	class MyInput
	{
		const string RotateAxisName = "Rotate";
		const string RotateGunAxisName = "RotateGun";
		const string ThrustAxisName = "Thrust";
		const string FireActionName = "Fire";
		const string AltFireActionName = "AltFire";

		PlayerInput playerInput;
		TankPlayerController ownerController;

		public float ThrustAxis, RotationAxis, TurretRotationAxis;
		public bool IsFiring;
		public bool IsFiringAlt;

		public MyInput(TankPlayerController ownerController)
		{
			Contract.Assert(ownerController);
			this.ownerController = ownerController;
		}

		bool ShouldLogAxisInput { get => ownerController.ShouldLogAxisInput; }

		public void InitializeAtUnityAwakeTime()
		{
			playerInput = ownerController.GetComponent<PlayerInput>();
			BindAllActionsToFunctions();
		}

		void BindAllActionsToFunctions()
		{
			BindPerformedHandlerToAction(RotateAxisName, InputAction_Rotate);
			BindPerformedHandlerToAction(RotateGunAxisName, InputAction_RotateGun);
			BindPerformedHandlerToAction(ThrustAxisName, InputAction_Thrust);

			BindPerformedHandlerToAction(FireActionName, InputAction_FireTurret);
			BindPerformedHandlerToAction(AltFireActionName, InputAction_FireAlt);
		}

		void BindPerformedHandlerToAction(string actionName, System.Action<InputAction.CallbackContext> performedHandler)
		{
			InputAction action = playerInput.actions[actionName];

			if(action != null)
			{
				action.performed += performedHandler;
			}
			else
			{
				Debug.LogError($"Unable to bind action performed handler: \"{actionName}\" is not defined");
			}
		}

		#region Input actions
		void InputAction_Rotate(InputAction.CallbackContext context) 
		{
			InputAction_FloatAxisChanged(context, RotateAxisName, out RotationAxis);
		}

		void InputAction_RotateGun(InputAction.CallbackContext context)
		{
			InputAction_FloatAxisChanged(context, RotateGunAxisName, out TurretRotationAxis);
		}

		void InputAction_Thrust(InputAction.CallbackContext context) 
		{
			InputAction_FloatAxisChanged(context, ThrustAxisName, out ThrustAxis);
		}

		void InputAction_FloatAxisChanged(InputAction.CallbackContext context, string axisName, out float axis)
		{
			axis = context.ReadValue<float>();
			if(ShouldLogAxisInput)
			{
				Debug.Log($"Value of axis \"{axisName}\" updated to {axis}");
			}
		}

		void InputAction_FireTurret(InputAction.CallbackContext context) 
		{
			InputAction_BoolAxisChanged(context, out IsFiring);
		}

		void InputAction_FireAlt(InputAction.CallbackContext context) 
		{
			InputAction_BoolAxisChanged(context, out IsFiringAlt);
		}

		void InputAction_BoolAxisChanged(InputAction.CallbackContext context, out bool axis) 
		{
			axis = context.ReadValue<float>() > 0;
		}
		#endregion // Input actions
	};
       	MyInput input;

	public bool ShouldLogAxisInput = false;

	// ControllableTank's script to init if there's no one found in the scene
	public ControllableTank TankTemplate;


	void Awake()
	{
		Debug.Log(MethodBase.GetCurrentMethod().Name);
		input = new MyInput(this);
		input.InitializeAtUnityAwakeTime();
		InitializeDebugGUI();
		ControllableTank = InstantiateOrKeepOnlySinglePlayableTank();
		LogControllableTankStatus();
	}

	void InitializeDebugGUI()
	{
		DebugDamageableGUI = GetComponent<IDamageableGUIComponent>();
	}

	ControllableTank InstantiateOrKeepOnlySinglePlayableTank()
	{
		GameObject tankCandidate = DestructAllPlayableTanksExceptFirst();
		if (tankCandidate)
		{
			return tankCandidate.GetComponent<ControllableTank>();
		}
		else
		{
			return InstantiateTaggedPlayerTank();
		}
	}

	GameObject DestructAllPlayableTanksExceptFirst()
	{
		IEnumerable<GameObject> playerEntities = FindPlayableTanks();
		GameObject chosenTank = playerEntities.FirstOrDefault();
		if (!chosenTank)
		{
			Debug.LogWarning($"Failed to find game object with tag \"{PlayerTag}\" and of type {nameof(ControllableTank)}");
		}
		DestroyAllGivenTanksExcept(playerEntities,chosenTank);
		return chosenTank;
	}

	GameObject[] FindPlayableTanks()
	{
		return GameObject.FindGameObjectsWithTag(PlayerTag).Where(o=>o.GetComponent<ControllableTank>() != null).ToArray();
	}

	void DestroyAllGivenTanksExcept(IEnumerable<GameObject> playerEntities, GameObject exceptTank)
	{
		IEnumerable<GameObject> extraPlayerEntities = playerEntities.Where( o => o != exceptTank );
		foreach(GameObject obj in extraPlayerEntities)
		{
			Debug.LogWarning($"Destroying extra player entity: name={obj.name}; type={obj.GetType()}");
			Destroy(obj);
		}
	}

	ControllableTank InstantiateTaggedPlayerTank()
	{
		Debug.Log($"Type of tank template object is {TankTemplate.GetType()}");
		ControllableTank instantiatedTank = Instantiate(TankTemplate, transform.position, transform.rotation) as ControllableTank;
		if (!instantiatedTank)
		{
			Debug.LogError($"Tank instantiation failed");
			return null;
		}
		instantiatedTank.tag = PlayerTag;
		return instantiatedTank;
	}

	void LogControllableTankStatus()
	{
		if (ControllableTank)
		{
			Debug.Log($"Now we use {ControllableTank.name} of class {ControllableTank.GetType()} as controllable tank");
		}
		else
		{
			Debug.LogError("Now we have NO tank object to control");
			return;
		}
	}

	void Start()
	{
		Debug.Log(MethodBase.GetCurrentMethod().Name);
		InitializeOnPossess();
	}

	// To be called right after the possessed tank is changed
	void InitializeOnPossess()
	{
		InitializeDamageableOnPossess();
		if(ControllableTank)
		{
			ControllableTank.WhenPossessedBy(this);
		}
	}

	void InitializeDamageableOnPossess()
	{
		Damageable = ControllableTank ? ControllableTank.GetComponent<IDamageableComponent>() : null;
		if(DebugDamageableGUI != null)
		{
			DebugDamageableGUI.Damageable = Damageable;
		}
	}

	void FixedUpdate()
	{
		if(ControllableTank)
		{
			if (input.IsFiring)
			{
				ControllableTank.FireTurretIfCan();
			}

			if (input.IsFiringAlt)
			{
				ControllableTank.FireAltIfCan();
			}

			if (!Mathf.Approximately(input.ThrustAxis, 0.0F) )
			{
				ControllableTank.Thrust(input.ThrustAxis);
			}

			if (!Mathf.Approximately(input.RotationAxis, 0.0F) )
			{
				ControllableTank.Rotate(input.RotationAxis);
			}

			if (!Mathf.Approximately(input.TurretRotationAxis, 0.0F) )
			{
				ControllableTank.RotateGun(input.TurretRotationAxis);
			}
		}
	}
}
