// Defines behaviour of the tank (aka pawn like in UE4)
//

using UnityEngine;

public class Tank : MonoBehaviour, IControllableTank
{
	public void Awake()
	{
		Debug.Log($"nameof(Awake); Sender=\"{name}\"");
	}

	public void Start()
	{
		Debug.Log($"nameof(Start); Sender=\"{name}\"");
	}

	#region IControllableEntity
	public void Fire(int FireIndex)
	{
		Debug.Log($"{nameof(Fire)}; Sender=\"{name}\", {nameof(FireIndex)}={FireIndex}");
	}

	public void Thrust(float axisValue)
	{
		Debug.Log($"{nameof(Thrust)}; Sender=\"{name}\", {nameof(axisValue)}={axisValue}");
	}

	public void Rotate(float axisValue)
	{
		Debug.Log($"{nameof(Rotate)}; Sender=\"{name}\", {nameof(axisValue)}={axisValue}");
	}
	#endregion //IControllableEntity
}

