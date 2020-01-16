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
		// WARNING!!! "Forward is" Z axis (up vector) in 2D!!!
		rb.velocity = transform.up * projectileSpeed;
		Debug.Log($"{nameof(Rigidbody2DProjectileMovement)} spawned: velocity={rb.velocity}; {nameof(projectileSpeed)}={projectileSpeed}");
	}	
}
