using UnityEngine;

public class Projectile : MonoBehaviour
{
	public GameObject InstigatorPawn;

	void Start()
	{
		foreach(IDamageCauser DamageCauser in GetComponents<IDamageCauser>())
		{
			DamageCauser.InstigatorPawn = InstigatorPawn;
		}
	}

	// @TODO: Collision self-destruction (move from DamageCauser)
}
