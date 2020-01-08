using UnityEngine;
using System.Diagnostics.Contracts;

public struct DamageState
{
	public int hits;
	public int maxHits;

	public DamageState(int hits, int maxHits)
	{
		// WARN! hits DO may be negative
		Contract.Assert(maxHits > 0);
		this.hits = hits;
		this.maxHits = maxHits;
	}

	public DamageState(int maxHits)
	: this(maxHits, maxHits)
	{
	}
};

public enum EDamageSetMode
{
	Normal, // Damage DO may be corrected on the gameplay side
	Reset, // Hits points are set as-is	
}

public class HitCountChangedEventArgs : System.EventArgs {};
public class HitCountChangingEventArgs : System.EventArgs
{
	public int NewHits { get; set; }

	public HitCountChangingEventArgs(int newHits)
	{
		this.NewHits = newHits;
	}
};

public class MaxHitCountChangedEventArgs : System.EventArgs {};
public class MaxHitCountChangingEventArgs : System.EventArgs 
{
	public int NewMaxHits { get; set; }

	public MaxHitCountChangingEventArgs(int newMaxHits)
	{
		this.NewMaxHits = newMaxHits;
	}
};

// Interface for damageable scripts
public interface IDamageable
{
	event System.EventHandler<HitCountChangedEventArgs> HitCountChanged;
	event System.EventHandler<HitCountChangingEventArgs> HitCountChanging;

	event System.EventHandler<MaxHitCountChangedEventArgs> MaxHitCountChanged;
	event System.EventHandler<MaxHitCountChangingEventArgs> MaxHitCountChanging;

	int HitCount { get; }
	int MaxHitCount { get; set; }

	// - Will automatically call Destroy;
	// - Will honor max hits;
	// - Everything ResetHitCount does;
	// @RETURNS: hit count really set
	int SetHitCount(int NewHitCount);

	// - Will fire events;
	void ResetHitCount(int NewHitCount);

	// Current state of damages;
	// When set, always set EXACTLY
	// WARNING!!! To make reset ever in inactive state, should pass true for bEverIfInactive
	// returns: count of damage REALLY received
	int SetDamageState(DamageState newDamageState, EDamageSetMode mode = EDamageSetMode.Normal, bool bEverIfInactive = false);
	DamageState GetDamageState();
	// @TODO: Add events (on damaged etc.)
}

public static class DamageableExtensions
{
	// @TODO: Can we implement extension properties?
	// We can NOT!

	// returns: amount of damage really received
	//public static int SetHitCount(this IDamageable damageable, int newHitCount)
	//{
	//	// @TODO: Change: use props instead!
	//	DamageState damageState = damageable.GetDamageState();
	//	damageState.hits = newHitCount;
	//	// WARNING!!! We can NOT invoke events from extension methods!
	//	//damageable.HitCountChanged?.Invoke(damageable, new HitCountChangedEventArgs());
	//	return damageable.SetDamageState(damageState);
///
	//}
	public static void SetMaxHitCount(this IDamageable damageable, int newMaxHitCount)
	{
		// @TODO: Change: use props instead!
		DamageState damageState = damageable.GetDamageState();
		damageState.maxHits = newMaxHitCount;
		damageable.SetDamageState(damageState);
	}
	public static int MakeDamage(this IDamageable damageable, int amount)
	{
		return damageable.SetHitCount(damageable.HitCount - amount);
	}
	public static int GetMaxHits(this IDamageable damageable)
	{
		return damageable.GetDamageState().maxHits;
	}
	public static bool AreHitsOver(this IDamageable damageable)
	{
		return damageable.HitCount > damageable.MaxHitCount;
	}
	public static bool AreHitsMaximum(this IDamageable damageable)
	{
		return damageable.HitCount == damageable.MaxHitCount;
	}
	// Checks whether the given hit count is at zero or below zero
	public static bool IsTotallyDamaged(this IDamageable damageable)
	{
		return damageable.HitCount <= 0;
	}
}
public static class DamageableUtils
{
	public static int MakeDamage(GameObject gameObject, int damage, bool bLogOnFailure = false)
	{
		// OK: We DO may safely search component by interfaces!
		Component mainScript = gameObject.GetComponent<IMyGameObject>() as Component;
		if(mainScript == null)
		{
			// if failed to find the main script, try to find the damageable component directly
			// (some objects may contain NO main script, but do contain the damageable component)
			IDamageable damageable = gameObject.GetComponent<IDamageable>();
			if(damageable != null)
			{
				return damageable.MakeDamage(damage);
			}
			else
			{
				if(bLogOnFailure)
				{
					Debug.LogWarning("{gameObject.name} does NOT contain damageable component");
				}
			}
		}
		else
		{
			return MakeDamageForMainScript(mainScript, damage, bLogOnFailure);
		}
		return 0;
	}

	// This function must be called on the main game object script (i.e. script that may support IMyGameObject)
	public static int MakeDamageForMainScript(Object gameObject, int damage, bool bLogOnFailure = false)
	{
		Contract.Assert(gameObject != null);
		if(gameObject is IMyGameObject myobj)
		{
			IDamageable damageable = myobj.GetDamageable();
			if(damageable != null)
			{
				return damageable.MakeDamage(damage);
			}
			else
			{
				if(bLogOnFailure)
				{
					Debug.LogWarning($"{nameof(GameObject)} {gameObject.name} is NOT damageable ( GetDamageable() returned null )!");
				}
			}
		}
		else
		{
			if(bLogOnFailure)
			{
				Debug.LogWarning($"{nameof(GameObject)} {gameObject.name} is NOT instance of the {nameof(IMyGameObject)} interface!");
			}
		}
		return 0;
	}
	public static int MakeDamageForDamageableObject(Object component, int damage, bool bLogOnFailure = false)
	{
		Contract.Assert(component != null);
		if(component is IDamageable damageable)
		{
			return damageable.MakeDamage(damage);
		}
		else
		{
			if(bLogOnFailure)
			{
				if(component is Object obj)
				{
					Debug.LogWarning($"object {obj.name} does NOT support {nameof(IDamageable)} interface!");
				}
			}
		}
		return 0;
	}
}