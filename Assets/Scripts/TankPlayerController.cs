using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public class TankPlayerController : MonoBehaviour
{
	const string PlayerTag = "Player";

	DamageableIMGUI debugDamageableGUI;

	ControllableTank tank;
	// damageable of the controlled tank 
	IDamageable damageable;

	struct InputState
	{
		public float axisThrust, axisRotate, axisRotateGun;
		public bool bFire;
		public bool bAltFire;
	};
       	InputState input;

	// Prefab of the tank, to be spawned if NO tank found in the scene
	// Must contain the ControllableTank script!
	//
	// @TODO Use the MonoScriptAttribute!
	// @TODO: RestrictToType
	// @TODO: RequireComponent
	// @TODO: ObjectField
	// @SEE: https://docs.unity3d.com/Manual/Attributes.html
	
	// ControllableTank's script to init if there's no one found in the scene
	public ControllableTank templTank;

	public bool bLogAxisInput = false;

	public void InputAction_RotateGun(InputAction.CallbackContext context)
	{
		float axisValue = context.ReadValue<float>();
		if(bLogAxisInput)
		{
			Debug.Log($"{MethodBase.GetCurrentMethod().Name}: {nameof(axisValue)}={axisValue}");
		}
		input.axisRotateGun = axisValue;
	}
	public void InputAction_FireGun(InputAction.CallbackContext context)
	{
		Debug.Log(MethodBase.GetCurrentMethod().Name);
		input.bFire = !input.bFire;
	}
	public void InputAction_AltFireGun(InputAction.CallbackContext context)
	{
		// WARNING!!! Unable to read bool value from button also!
		//bool bPressed = context.ReadValue<bool>();
		Debug.Log(MethodBase.GetCurrentMethod().Name);
		input.bAltFire = !input.bAltFire;
	}
	public void InputAction_Rotate(InputAction.CallbackContext context)
	{
		float axisValue = context.ReadValue<float>();
		if(bLogAxisInput)
		{
			Debug.Log($"{MethodBase.GetCurrentMethod().Name}: {nameof(axisValue)}={axisValue}");
		}
		input.axisRotate = axisValue;
	}
	public void InputAction_Thrust(InputAction.CallbackContext context)
	{
		float axisValue = context.ReadValue<float>();
		if(bLogAxisInput)
		{
			Debug.Log($"{MethodBase.GetCurrentMethod().Name}: {nameof(axisValue)}={axisValue}");
		}
		input.axisThrust = axisValue;
	}

	void Awake()
	{
		Debug.Log(MethodBase.GetCurrentMethod().Name);
		InitializeDebugGUI();
		InitializeLinkToTank();
	}

	void InitializeDebugGUI()
	{
		debugDamageableGUI = GetComponent<DamageableIMGUI>();
	}

	void InitializeLinkToTank()
	{
		GameObject tankGameObjectCandidate = KeepFirstPlayableTank();
		if (tankGameObjectCandidate)
		{
			tank = tankGameObjectCandidate.GetComponent<ControllableTank>();
		}
		else
		{
			tank = InstantiateTaggedPlayerTank();
		}
		
		LogControllableTankStatus();
	}

	GameObject KeepFirstPlayableTank()
	{
		IEnumerable<GameObject> playerEntities = FindPlayableTanks();
		GameObject tankGameObjectCandidate = playerEntities.FirstOrDefault();
		if (!tankGameObjectCandidate)
		{
			Debug.LogWarning($"Failed to find game object with tag \"{PlayerTag}\" and of type {nameof(ControllableTank)}");
		}
		DestroyAllGivenTanksExcept(playerEntities,tankGameObjectCandidate);
		return tankGameObjectCandidate;
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
		Debug.Log($"Type of tank template object is {templTank.GetType()}");
		ControllableTank t = Instantiate(templTank, transform.position, transform.rotation) as ControllableTank;
		if ( ! t )
		{
			Debug.LogError($"Tank instantiation failed");
			return null;
		}
		t.tag = PlayerTag;
		return t;
	}

	void LogControllableTankStatus()
	{
		if (tank)
		{
			Debug.Log($"Now we use {tank.name} of class {tank.GetType()} as controllable tank");
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
		damageable = tank ? tank.GetComponent<IDamageable>() : null;
		if(debugDamageableGUI)
		{
			debugDamageableGUI.Damageable = damageable;
		}
	}

	void FixedUpdate()
	{
		if(tank)
		{
			if (input.bFire)
			{
				tank.FireTurretIfCan();
			}

			if (input.bAltFire)
			{
				tank.FireAltIfCan();
			}

			if (!Mathf.Approximately(input.axisThrust, 0.0F) )
			{
				tank.Thrust(input.axisThrust);
			}

			if (!Mathf.Approximately(input.axisRotate, 0.0F) )
			{
				tank.Rotate(input.axisRotate);
			}

			if (!Mathf.Approximately(input.axisRotateGun, 0.0F) )
			{
				tank.RotateGun(input.axisRotateGun);
			}
		}
	}
}
