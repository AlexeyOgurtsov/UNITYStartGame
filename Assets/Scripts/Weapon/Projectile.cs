using UnityEngine;

[AddComponentMenu("Weapon/Projectile")]
[DisallowMultipleComponent]
public class Projectile : MonoBehaviour
{
	public GameObject InstigatorPawn;

	void Start()
	{
		foreach(IDamageCauserComponent DamageCauser in GetComponents<IDamageCauserComponent>())
		{
			DamageCauser.InstigatorPawn = InstigatorPawn;
		}
	}

	// @TODO: Collision self-destruction (move from DamageCauser)
}
