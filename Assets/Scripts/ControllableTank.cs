// Base class of all controllable tanks (like Pawn in UE4)
//

using UnityEngine;

public class ControllableTank : MonoBehaviour, IControllableTank
{
	const float TANK_ROTATION_SPEED_DEGS = 10.0F;

	public enum FireIndex
	{
		Turret,
		AltFire
	};

	IDamageable damageable;
	TankTurret turret;

	public IDamageable GetDamageable() => damageable;

	public void RotateGun(float axisValue)
	{
		if(turret)
		{
			turret.transform.Rotate(0, 0, axisValue * Time.deltaTime * TANK_ROTATION_SPEED_DEGS);
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

	protected void Awake()
	{
		Debug.Log($"{nameof(Awake)}; Sender=\"{name}\"");
		LinkToTurret();
		LinkToDamageableComponent();
	}

	void LinkToTurret()
	{
		turret = GetComponentInChildren<TankTurret>();
		if(turret == null)
		{
			Debug.LogWarning($"Unable to find attached turret script");
		}
	}

	void LinkToDamageableComponent()
	{
		// damageable component: may be provided or may be not
		damageable = GetComponent<IDamageable>();
	}

	protected void Start()
	{
		Debug.Log($"{nameof(Start)}; Sender=\"{name}\"");
	}
}

