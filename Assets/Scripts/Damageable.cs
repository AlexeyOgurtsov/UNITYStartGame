using UnityEngine;
using System.Reflection;

public class Damageable : MonoBehaviour, IDamageable
{
	public int initialMaxHits = 100;
	public int initialHits = 100;
	//public int hits = maxHits; // Compile error!!! We cannot initialize fields from fields

	public event System.EventHandler<HitCountChangedEventArgs> HitCountChanged;
	public event System.EventHandler<HitCountChangingEventArgs> HitCountChanging;

	public event System.EventHandler<MaxHitCountChangedEventArgs> MaxHitCountChanged;
	public event System.EventHandler<MaxHitCountChangingEventArgs> MaxHitCountChanging;

	public int HitCount 
	{
	       	get => damageState.hits;
	}
	public int MaxHitCount 
	{
	       	get => damageState.maxHits;
		set
		{
			var ChangingEventArgs = new MaxHitCountChangingEventArgs(value);
			OnMaxHitCountChanging(ChangingEventArgs);
			damageState.maxHits = ChangingEventArgs.NewMaxHits;
			var ChangedEventArgs = new MaxHitCountChangedEventArgs();
			OnMaxHitCountChanged(ChangedEventArgs);
		}
	}

	public void ResetHitCount(int NewHitCount)
	{
		var ChangingEventArgs = new HitCountChangingEventArgs(NewHitCount);
		OnHitCountChanging(ChangingEventArgs);
		damageState.hits = ChangingEventArgs.NewHits;
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
			// HINT: The only way we can call interface extensions methods from the class itself:
			//int oldHitCount = this.GetHitCount();
			int oldHitCount = HitCount;

			// NOTE: we cannot here update damage state by copying,
			// because public properties fire events;
			MaxHitCount = newDamageState.maxHits;
			SetHitCount(newDamageState.hits);

			int damageApplied = oldHitCount - newDamageState.hits;
			// @TODO: Update applied damage count (a chance to override)!
			if(this.IsTotallyDamaged())
			{
				// @TODO: We must broadcast event
				Destroy(gameObject);
			}
			return damageApplied;
		}
		else
		{
			return 0;
		}
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

	void Update()
	{
	}

	void Awake()
	{
		Debug.Log($"Damageable: {MethodBase.GetCurrentMethod().Name}; Sender={gameObject.name}");
		// WARN: At this point public field values are NOT yet AVAILABLE (i.e. initialized to ctor-initialized)!!!
	}

	void Start()
	{
		Debug.Log($"Damageable: {MethodBase.GetCurrentMethod().Name}; Sender={gameObject.name}");

		Debug.Log($"initialHits={initialHits}; initialMaxHits ={initialMaxHits}");
		// @TODO: when we enabling or disabling component dynamically, 
		// we must NOT reset the damage state!
		MaxHitCount = initialMaxHits;
		SetHitCount(initialHits);
	}
	DamageState damageState;
}
