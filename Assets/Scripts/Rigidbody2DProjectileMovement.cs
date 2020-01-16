using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Rigidbody2DProjectileMovement : MonoBehaviour
{
	Rigidbody2D rb;

	public const float DEFAULT_SPEED = 1F;

	public float projectileSpeed = DEFAULT_SPEED;

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		// @TODO Check: rigidbody mode dynamic
		// @TODO Check rigidbody continous collision detection
		// @TODO Gravity scale to ZERO
		
		// WARNING!!! Forward is Z axis!!!
		rb.velocity = transform.up * projectileSpeed;
		Debug.Log($"{nameof(Projectile)} spawned: velocity={rb.velocity}; {nameof(projectileSpeed)}={projectileSpeed}");
	}	
}
