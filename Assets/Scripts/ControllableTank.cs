// Base class of all controllable tanks (like Pawn in UE4)
//

using UnityEngine;

public class ControllableTank : MonoBehaviour, IControllableTank
{
	const float TANK_ROTATION_SPEED_DEGS = 10.0F;

	public enum FireIndex
	{
		Turret
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

			default:
			{
				throw new System.InvalidOperationException($"Unsupported fire index {nameof(fireIndexValue)}={fireIndexValue}; {nameof(fireIndex)}={fireIndex}");
			}
		}
	}

	void FireTurretIfCan()
	{
		if(turret)
		{
			turret.FireIfCan();
		}
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
		// @TODO: Try to find by tag!
		turret = GetComponentInChildren<TankTurret>();
		if(turret == null)
		{
			Debug.LogWarning($"Unable to find attached turret script");
		}
		// damageable component: may be provided or may be not
		damageable = GetComponent<IDamageable>();
	}

	protected void Start()
	{
		Debug.Log($"{nameof(Start)}; Sender=\"{name}\"");
	}
}

