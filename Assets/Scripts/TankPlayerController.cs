using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public class TankPlayerController : MonoBehaviour
{
	#region Controllable config fields
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
	#endregion // Controllable config fields

	#region Input config fields
	public bool bLogAxisInput = false;
	#endregion // Input config fields

	#region Unity Game Object Messages
	public void Awake()
	{
		Debug.Log(MethodBase.GetCurrentMethod().Name);
		InitializeLinkToTank();
	}

	public void Start()
	{
		Debug.Log(MethodBase.GetCurrentMethod().Name);
	}

	public void FixedUpdate()
	{
		if(tank)
		{
			if(input.bFire)
			{
				tank.Fire(0);
			}

			if(input.bAltFire)
			{
				tank.Fire(1);
			}

			if( ! Mathf.Approximately(input.axisThrust, 0.0F) )
			{
				tank.Thrust(input.axisThrust);
			}

			if( ! Mathf.Approximately(input.axisRotate, 0.0F) )
			{
				tank.Rotate(input.axisRotate);
			}

			if( ! Mathf.Approximately(input.axisRotateGun, 0.0F) )
			{
				tank.RotateGun(input.axisRotateGun);
			}
		}
	}
	#endregion // Unity Game Object Messages

	#region Input actions
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
	#endregion Input actions

	struct InputState
	{
		public float axisThrust, axisRotate, axisRotateGun;
		public bool bFire;
		public bool bAltFire;
	};
       	InputState input;

	void InitializeLinkToTank()
	{
		const string PlayerTag = "Player";
		// WARNING!!! We must use GameObject. prefix when using FindGameObjectsWithTag, otherwise error!
		
		// WRONG: Game objects NEVER to be casted to components!!!
		//IEnumerable<ControllableTank> playerEntities = GameObject.FindGameObjectsWithTag(PlayerTag).OfType<ControllableTank>();
		IEnumerable<GameObject> playerEntities = GameObject.FindGameObjectsWithTag(PlayerTag).Where(o=>o.GetComponent<ControllableTank>() != null);
		// Checking that GameObject.FindGameObjectsWithTag never returns invalid objects (GameObject to-bool conversion is defined)
		Debug.Assert(playerEntities.All(o=>o));
		// C# Note: difference between between FirstOrDefault vs. SingleOrDefault
		// SingleOrDefault - exception if more than one element
		var tankGameObjectCandidate = playerEntities.FirstOrDefault(/*o=>o.GetComponent<ControllableTank>() != null*/);
		if(tankGameObjectCandidate)
		{
			tank = tankGameObjectCandidate.GetComponent<ControllableTank>();
			// Destroying extra player entities
			{
				IEnumerable<GameObject> extraPlayerEntities = playerEntities.Where( o => o != tankGameObjectCandidate );
				foreach(GameObject obj in extraPlayerEntities)
				{
					Debug.LogWarning($"Destroying extra player entity: name={obj.name}; type={obj.GetType()}");
					Destroy(obj);
				}
			}
		}

		

		if( ! tank )
		{
			Debug.LogWarning($"Failed to find game object with tag \"{PlayerTag}\" and of type {nameof(ControllableTank)}");

			if( ! templTank )
			{
				Debug.LogError($"Unable to spawn tank - {nameof(templTank)} is null");
				// @TODO: abort gameplay execution (throw critical exception?) 
			}
			else
			{
				Debug.Log($"Type of tank template object is {templTank.GetType()}");
				tank = Instantiate(templTank, transform.position, transform.rotation) as ControllableTank;
				if ( ! tank )
				{
					Debug.LogError($"Tank instantiation failed");
				}
			}
		}
		
		if( tank )
		{
			Debug.Log($"Now we use {tank.name} of class {tank.GetType()} as controllable tank");
		}
		else
		{
			Debug.LogError("Now we have NO tank object to control");
			return;
		}

		if( ! tank.CompareTag(PlayerTag) )
		{
			tank.tag = PlayerTag;
		}
	}
	ControllableTank tank;
}
