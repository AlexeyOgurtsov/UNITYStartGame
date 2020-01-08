// Base class of all controllable tanks (like Pawn in UE4)
//

using UnityEngine;

public class ControllableTank : MonoBehaviour, IControllableTank
{
	const float TANK_ROTATION_SPEED_DEGS = 10.0F;

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

	public void Fire(int FireIndex)
	{
		Debug.Log($"{nameof(Fire)}; Sender=\"{name}\", {nameof(FireIndex)}={FireIndex}");
		if(FireIndex == 0)
		{
			if(turret)
			{
				turret.Fire();
			}
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

