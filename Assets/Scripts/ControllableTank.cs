// Base class of all controllable tanks (like Pawn in UE4)
//

using UnityEngine;

public class ControllableTank : MonoBehaviour, IControllableTank
{
	public void Awake()
	{
		Debug.Log($"{nameof(Awake)}; Sender=\"{name}\"");
	}

	public void Start()
	{
		Debug.Log($"{nameof(Start)}; Sender=\"{name}\"");
	}

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

	#region IControllableTank
	public void RotateGun(float axisValue)
	{
		// @TODO
	}
	#endregion // IControllableTank
}

