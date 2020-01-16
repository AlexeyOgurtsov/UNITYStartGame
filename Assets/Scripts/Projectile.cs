using UnityEngine;

public class Projectile : MonoBehaviour
{
	public const int DEFAULT_DAMAGE = 10;

	public int damage = DEFAULT_DAMAGE;

	void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log($"{nameof(Projectile)}: {nameof(OnCollisionEnter2D)}");

		// @TODO: Spawn explosition fx
		DamageableUtils.MakeDamage(collision.gameObject, damage, true);

		Destroy(this.gameObject);
	}
}
