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

	DamageableIMGUI debugDamageableGUI;

	ControllableTank controllableTank;
	// damageable of the controlled tank 
	IDamageable damageable;

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

	// Prefab of the tank, to be spawned if NO tank found in the scene
	// Must contain the ControllableTank script!
	//
	// @TODO Use the MonoScriptAttribute!
	// @TODO: RestrictToType
	// @TODO: RequireComponent
	// @TODO: ObjectField
	// @SEE: https://docs.unity3d.com/Manual/Attributes.html
	
	// ControllableTank's script to init if there's no one found in the scene
	public ControllableTank TankTemplate;


	void Awake()
	{
		Debug.Log(MethodBase.GetCurrentMethod().Name);
		input = new MyInput(this);
		input.InitializeAtUnityAwakeTime();
		InitializeDebugGUI();
		controllableTank = InstantiateOrKeepOnlySinglePlayableTank();
		LogControllableTankStatus();
	}

	void InitializeDebugGUI()
	{
		debugDamageableGUI = GetComponent<DamageableIMGUI>();
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
		if (controllableTank)
		{
			Debug.Log($"Now we use {controllableTank.name} of class {controllableTank.GetType()} as controllable tank");
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
		damageable = controllableTank ? controllableTank.GetComponent<IDamageable>() : null;
		if(debugDamageableGUI)
		{
			debugDamageableGUI.Damageable = damageable;
		}
	}

	void FixedUpdate()
	{
		if(controllableTank)
		{
			if (input.IsFiring)
			{
				controllableTank.FireTurretIfCan();
			}

			if (input.IsFiringAlt)
			{
				controllableTank.FireAltIfCan();
			}

			if (!Mathf.Approximately(input.ThrustAxis, 0.0F) )
			{
				controllableTank.Thrust(input.ThrustAxis);
			}

			if (!Mathf.Approximately(input.RotationAxis, 0.0F) )
			{
				controllableTank.Rotate(input.RotationAxis);
			}

			if (!Mathf.Approximately(input.TurretRotationAxis, 0.0F) )
			{
				controllableTank.RotateGun(input.TurretRotationAxis);
			}
		}
	}
}
