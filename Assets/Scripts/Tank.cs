// Defines behaviour of the tank (aka pawn like in UE4)
//

using UnityEngine;

public class Tank : ControllableTank
{
	public new void Awake()
	{
		Debug.Log($"nameof(Awake); Sender=\"{name}\"");
		base.Awake();
	}

	public new void Start()
	{
		Debug.Log($"nameof(Start); Sender=\"{name}\"");
		base.Start();
	}
}

