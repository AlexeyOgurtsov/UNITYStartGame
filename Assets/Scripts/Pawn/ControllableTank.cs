// Base class of all controllable tanks (like Pawn in UE4)
//

using UnityEngine;

[System.Serializable]
public class ControllableTankMovement
{
	ControllableTank tank;
	Rigidbody2D rb;

	public float ThrustImpulse = 10000;
	public ForceMode2D ThrustMode2D = ForceMode2D.Impulse;
	public float RotationDegsPerSec = 10;

	public ControllableTankMovement(ControllableTank tank)
	{
		this.tank = tank;
		rb = tank.GetComponent<Rigidbody2D>();
	}

	public void Thrust(float axisValue)
	{
		Vector2 thrustForceParameter = axisValue * ThrustImpulse * tank.transform.up * Time.fixedDeltaTime;
		rb.AddForce(thrustForceParameter, ThrustMode2D);
	}

	public void Rotate(float axisValue)
	{
		rb.MoveRotation(rb.rotation + RotationDegsPerSec * axisValue * Time.fixedDeltaTime);
	}
}

[RequireComponent(typeof(Rigidbody2D))]
public class ControllableTank : MonoBehaviour, IControllableTank
{
	const float RotationSpeedDegs = 10.0F;

	public enum FireIndex
	{
		Turret,
		AltFire
	};

	public ControllableTankMovement movement;

	IDamageableComponent damageable;
	ProjectileShooter turret;

	public IDamageableComponent GetDamageable() => damageable;

	public void RotateGun(float axisValue)
	{
		if(turret)
		{
			turret.transform.Rotate(0, 0, axisValue * Time.fixedDeltaTime * RotationSpeedDegs);
		}
	}

	public void FireIfCan(int fireIndexValue)
	{
		Debug.Log($"{nameof(FireIfCan)}; Sender=\"{name}\", {nameof(fireIndexValue)}={fireIndexValue}");
		var fireIndex = (FireIndex)fireIndexValue;
		switch(fireIndex)
		{
			case FireIndex.Turret:
			{
				FireTurretIfCan();
				break;
			}

			case FireIndex.AltFire:
			{
				FireAltIfCan();
				break;
			}

			default:
			{
				throw new System.InvalidOperationException($"Unsupported fire index {nameof(fireIndexValue)}={fireIndexValue}; {nameof(fireIndex)}={fireIndex}");
			}
		}
	}

	public void FireTurretIfCan()
	{
		if(turret)
		{
			turret.FireIfCan();
		}
	}
	public void FireAltIfCan()
	{
		Debug.LogWarning($"{nameof(FireIndex.AltFire)} is not yet implemented");
		// @TODO
	}

	public void Thrust(float axisValue)
	{
		movement.Thrust(axisValue);
	}

	public void Rotate(float axisValue)
	{
		movement.Rotate(axisValue);
	}

	public void WhenPossessedBy(TankPlayerController newController)
	{
		turret?.WhenPossessedBy(new DamageInstigator(newController, gameObject));
	}

	protected void Awake()
	{
		Debug.Log($"{nameof(Awake)}; Sender=\"{name}\"");
		movement = new ControllableTankMovement(this);
		FindAndInitializeTurretAtUnityAwakeTime();
		LinkToDamageableComponent();
	}

	void FindAndInitializeTurretAtUnityAwakeTime()
	{
		turret = GetComponentInChildren<ProjectileShooter>();
		if(turret == null)
		{
			Debug.LogWarning($"Unable to find attached turret script");
			return;
		}
	}

	void LinkToDamageableComponent()
	{
		// damageable component: may be provided or may be not
		damageable = GetComponent<IDamageableComponent>();
	}
}

