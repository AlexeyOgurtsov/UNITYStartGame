using UnityEngine;
using System.Reflection;

[AddComponentMenu("Damage/Damageable")]
[DisallowMultipleComponent]
public class DamageableMonoBehaviour : MonoBehaviour, IDamageableComponent
{
	DamageState damageState;

	public event System.EventHandler<HitCountChangedEventArgs> HitCountChanged;
	public event System.EventHandler<HitCountChangingEventArgs> HitCountChanging;

	public event System.EventHandler<MaxHitCountChangedEventArgs> MaxHitCountChanged;
	public event System.EventHandler<MaxHitCountChangingEventArgs> MaxHitCountChanging;

	public DamageInstigator Instigator { get; set; }

	public int HitCount => damageState.Hits;
	public int MaxHitCount 
	{
	       	get => damageState.MaxHits;
		set
		{
			var ChangingEventArgs = new MaxHitCountChangingEventArgs(value);
			OnMaxHitCountChanging(ChangingEventArgs);
			damageState.MaxHits = ChangingEventArgs.NewMaxHits;
			var ChangedEventArgs = new MaxHitCountChangedEventArgs();
			OnMaxHitCountChanged(ChangedEventArgs);
		}
	}

	public int initialMaxHits = 100;
	public int initialHits = 100;

	public void ResetHitCount(int NewHitCount)
	{
		var ChangingEventArgs = new HitCountChangingEventArgs(NewHitCount);
		OnHitCountChanging(ChangingEventArgs);
		damageState.Hits = ChangingEventArgs.NewHits;
		var ChangedEventArgs = new HitCountChangedEventArgs();
		OnHitCountChanged(ChangedEventArgs);
	}

	public int SetHitCount(int NewHitCount)
	{
		// @TODO: honor max hits here
		ResetHitCount(NewHitCount);
		if(this.IsTotallyDamaged())
		{
			// @TODO: We must broadcast event
			Destroy(gameObject);
		}
		return NewHitCount;
	}

	public int SetDamageState(DamageState newDamageState, EDamageSetMode mode, bool bEverIfInactive = false)
	{
		if(bEverIfInactive || isActiveAndEnabled)
		{
			return SetDamageStateEverIfInactive(newDamageState, mode);
		}
		else
		{
			return 0;
		}
	}
	public int SetDamageStateIfActiveAndEnabled(DamageState newDamageState, EDamageSetMode mode = EDamageSetMode.Normal)
	{
		if(isActiveAndEnabled)
		{
			return SetDamageStateEverIfInactive(newDamageState, mode);
		}
		else
		{
			return 0;
		}
	}
	public int SetDamageStateEverIfInactive(DamageState newDamageState, EDamageSetMode mode = EDamageSetMode.Normal)
	{
		// HINT: The only way we can call interface extensions methods from the class itself:
		//int oldHitCount = this.GetHitCount();
		int oldHitCount = HitCount;

		// NOTE: we cannot here update damage state by copying,
		// because public properties fire events;
		MaxHitCount = newDamageState.MaxHits;
		SetHitCount(newDamageState.Hits);

		int damageApplied = oldHitCount - newDamageState.Hits;
		// @TODO: Update applied damage count (a chance to override)!
		if(this.IsTotallyDamaged())
		{
			// @TODO: We must broadcast event
			Destroy(gameObject);
		}
		return damageApplied;
	}

	public DamageState GetDamageState() => damageState;

	protected virtual void OnHitCountChanging(HitCountChangingEventArgs e)
	{
		HitCountChanging?.Invoke(this, e);
	}

	protected virtual void OnHitCountChanged(HitCountChangedEventArgs e)
	{
		HitCountChanged?.Invoke(this, e);
	}

	protected virtual void OnMaxHitCountChanging(MaxHitCountChangingEventArgs e)
	{
		MaxHitCountChanging?.Invoke(this, e);
	}

	protected virtual void OnMaxHitCountChanged(MaxHitCountChangedEventArgs e)
	{
		MaxHitCountChanged?.Invoke(this, e);
	}

	void Start()
	{
		Debug.Log($"Damageable: {MethodBase.GetCurrentMethod().Name}; Sender={gameObject.name}");
		Debug.Log($"initialHits={initialHits}; initialMaxHits ={initialMaxHits}");
		MaxHitCount = initialMaxHits;
		SetHitCount(initialHits);
	}
}
