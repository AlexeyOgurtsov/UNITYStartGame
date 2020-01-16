using UnityEngine;

public class CollisionDamager : MonoBehaviour, IDamageCauser
{
	public const int DEFAULT_DAMAGE = 10;

	public int damage = DEFAULT_DAMAGE;
	public bool ShouldSelfDestructOnMakeDamage = true;

	public GameObject InstigatorPawnField;
	public bool ShouldDamageInstigatorField;

	#region IDamageCauser properties
	public GameObject InstigatorPawn
	{
		get => InstigatorPawnField;
		set => InstigatorPawnField = value;
	}
	public bool ShouldDamageInstigator
	{
		get => ShouldDamageInstigatorField;
		set => ShouldDamageInstigatorField = value;
	}
	#endregion // IDamageCauser properties

	void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log($"{nameof(CollisionDamager)}: {nameof(OnCollisionEnter2D)}");

		if(collision.gameObject != null)
		{
			if (ShouldDamageGameObject(collision.gameObject))
			{
				DoMakeDamageOnGameObject(collision.gameObject);
			}
		}
	}

	bool ShouldDamageGameObject(GameObject gameObject)
	{
		if (InstigatorPawn == gameObject && !ShouldDamageInstigator)
		{
			return false;
		}
		return true;
	}

	void DoMakeDamageOnGameObject(GameObject gameObject)
	{
		// @TODO: Spawn explosition fx
		DamageableUtils.MakeDamage(gameObject, damage, true);

		if (ShouldSelfDestructOnMakeDamage)
		{
			Destroy(this.gameObject);
		}
	}
}
