// Base class of all controllable tanks (like Pawn in UE4)
//

using UnityEngine;

public class ControllableTank : MonoBehaviour, IControllableTank
{
	#region unity framework methods
	public void Awake()
	{
		Debug.Log($"{nameof(Awake)}; Sender=\"{name}\"");
		// damageable component: may be provided or may be not
		damageable = GetComponent<Damageable>();
	}

	public void Start()
	{
		Debug.Log($"{nameof(Start)}; Sender=\"{name}\"");
	}
	#endregion // unity framework methods

	#region IControllableTank
	public void RotateGun(float axisValue)
	{
		// @TODO
	}
	#endregion // IControllableTank

	#region IControllableEntity
	public void Fire(int FireIndex)
	{
		Debug.Log($"{nameof(Fire)}; Sender=\"{name}\", {nameof(FireIndex)}={FireIndex}");
	}

	public void Thrust(float axisValue)
	{
		// @TODO
	}

	public void Rotate(float axisValue)
	{
		// @TODO
	}
	#endregion //IControllableEntity

	#region IMyGameObject
	// Returns damageable interface instance, if supports damageable,
	// or nullptr if does NOT support it!
	public Damageable GetDamageable() => damageable;
	#endregion // IMyGameObject

	#region damageable
	Damageable damageable;
	#endregion // damageable
}

