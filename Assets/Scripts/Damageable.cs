using UnityEngine;
using System.Reflection;

public class Damageable : MonoBehaviour, IDamageable
{
	#region IDamageable interface
	public int SetDamageState(DamageState newDamageState, EDamageSetMode mode)
	{
		// @TODO: Can we call interface extension method here?
		//int oldHitCount = GetHitCount();
		int oldHitCount = this.GetHitCount();
		damageState = newDamageState;
		int damageApplied = oldHitCount - newDamageState.hits;
		// @TODO: We must broadcast event
		// @TODO: Update applied damage count (a chance to override)!
		if(this.IsTotallyDamaged())
		{
			// @TODO: We must broadcast event
			Destroy(gameObject);
		}
		return damageApplied;
	}
	public DamageState GetDamageState() => damageState;
	#endregion // IDamageable interface

	void Update()
	{
	}

	void Awake()
	{
		Debug.Log($"Damageable: {MethodBase.GetCurrentMethod().Name}; Sender={gameObject.name}");
	}

	void Start()
	{
		Debug.Log($"Damageable: {MethodBase.GetCurrentMethod().Name}; Sender={gameObject.name}");
	}

	DamageState damageState;
}
