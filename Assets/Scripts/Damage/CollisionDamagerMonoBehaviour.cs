using UnityEngine;

[AddComponentMenu("Damage/CollisionDamager")]
public class CollisionDamagerComponentMonoBehaviour : MonoBehaviour, IDamageCauserComponent
{
	const int DefaultDamage = 10;

	public int damage = DefaultDamage;
	public bool ShouldSelfDestructOnMakeDamage = true;

	public GameObject InstigatorPawnField;
	public bool ShouldDamageInstigatorField;

	#region IDamageCauserComponent properties
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
	#endregion // IDamageCauserComponent properties

	void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log($"{nameof(CollisionDamagerComponentMonoBehaviour)}: {nameof(OnCollisionEnter2D)}");

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
