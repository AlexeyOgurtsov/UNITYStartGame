using UnityEngine;
using System.Diagnostics.Contracts;

public struct DamageState
{
	public int Hits;
	public int MaxHits;

	public DamageState(int MaxHits)
	: this(MaxHits, MaxHits)
	{
	}

	public DamageState(int hits, int MaxHits)
	{
		// WARN! hits DO may be negative
		Contract.Assert(MaxHits > 0);
		this.Hits = hits;
		this.MaxHits = MaxHits;
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
	public HitCountChangingEventArgs(int newHits)
	{
		this.NewHits = newHits;
	}

	public int NewHits { get; set; }
};

public class MaxHitCountChangedEventArgs : System.EventArgs {};
public class MaxHitCountChangingEventArgs : System.EventArgs 
{
	public MaxHitCountChangingEventArgs(int newMaxHits)
	{
		this.NewMaxHits = newMaxHits;
	}

	public int NewMaxHits { get; set; }
};

// Interface for damageable scripts
public interface IDamageableComponent : IDamageSubsystemComponent
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
	// returns: count of damage REALLY received
	int SetDamageStateIfActiveAndEnabled(DamageState newDamageState, EDamageSetMode mode = EDamageSetMode.Normal);
	int SetDamageStateEverIfInactive(DamageState newDamageState, EDamageSetMode mode = EDamageSetMode.Normal);
	DamageState GetDamageState();
	// @TODO: Add events (on damaged etc.)
}

public static class DamageableExtensions
{
	public static void SetMaxHitCount(this IDamageableComponent damageable, int newMaxHitCount)
	{
		DamageState damageState = damageable.GetDamageState();
		damageState.MaxHits = newMaxHitCount;
		damageable.SetDamageState(damageState);
	}
	public static int MakeDamage(this IDamageableComponent damageable, Damage damage)
	{
		return damageable.SetHitCount(damageable.HitCount - damage.Amount);
	}

	public static int GetMaxHits(this IDamageableComponent damageable) => damageable.GetDamageState().MaxHits;
	public static bool AreHitsOver(this IDamageableComponent damageable) => damageable.HitCount > damageable.MaxHitCount;
	public static bool AreHitsMaximum(this IDamageableComponent damageable) => damageable.HitCount == damageable.MaxHitCount;
	// Checks whether the given hit count is at zero or below zero
	public static bool IsTotallyDamaged(this IDamageableComponent damageable) => damageable.HitCount <= 0;
}
