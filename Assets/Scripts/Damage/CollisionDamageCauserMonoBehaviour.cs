using UnityEngine;

[AddComponentMenu("Damage/CollisionDamageCauser")]
public class CollisionDamageCauserMonoBehaviour : MonoBehaviour, IDamageCauserComponent
{
	const int DefaultDamageAmount = 10;

	public int DamageAmount = DefaultDamageAmount;
	public bool ShouldSelfDestructOnMakeDamage = true;

	public bool ShouldDamageInstigatorField;

	#region IDamageCauserComponent properties
	public DamageInstigator Instigator { get; set; }
	public bool ShouldDamageInstigator
	{
		get => ShouldDamageInstigatorField;
		set => ShouldDamageInstigatorField = value;
	}
	#endregion // IDamageCauserComponent properties

	void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log($"{nameof(CollisionDamageCauserMonoBehaviour)}: {nameof(OnCollisionEnter2D)}");

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
		if (Instigator.Pawn == gameObject && !ShouldDamageInstigator)
		{
			return false;
		}
		return true;
	}

	void DoMakeDamageOnGameObject(GameObject gameObject)
	{
		// @TODO: Spawn explosition fx
		DamageUtils.MakeDamageIfDamageableGameObject(gameObject, DamageToCause);

		if (ShouldSelfDestructOnMakeDamage)
		{
			Destroy(this.gameObject);
		}
	}

	Damage DamageToCause
	{
		get => new Damage(DamageAmount, Instigator);
	}
}
