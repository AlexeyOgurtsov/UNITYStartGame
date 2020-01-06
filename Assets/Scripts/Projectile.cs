using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
	public const float DEFAULT_SPEED = 1F;
	public const int DEFAULT_DAMAGE = 10;

	#region collision config fields
	public float projectileSpeed = DEFAULT_SPEED;
	#endregion // collision config fields

	#region damage config fields
	public int damage = DEFAULT_DAMAGE;
	#endregion // damage config fields

	#region unity functions
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

	void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log($"{nameof(Projectile)}: {nameof(OnCollisionEnter2D)}");

		// @TODO: Spawn explosition fx
		DamageableUtils.MakeDamage(collision.gameObject, damage, true);

		Destroy(this.gameObject);
	}
	#endregion // unity functions

	#region collision
	Rigidbody2D rb;
	#endregion // collision
}
