// Base class of all controllable tanks (like Pawn in UE4)
//

using UnityEngine;

public class ControllableTank : MonoBehaviour, IControllableTank
{
	const float RotationSpeedDegs = 10.0F;

	public enum FireIndex
	{
		Turret,
		AltFire
	};

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
		// @TODO
	}

	public void Rotate(float axisValue)
	{
		// @TODO
	}

	public void WhenPossessedBy(TankPlayerController newController)
	{
		turret?.WhenPossessedBy(new DamageInstigator(newController, gameObject));
	}

	protected void Awake()
	{
		Debug.Log($"{nameof(Awake)}; Sender=\"{name}\"");
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

