// All my game object scripts must implement this interface!

using UnityEngine;
using System.Diagnostics.Contracts;

public interface IMyGameObject
{
	// Returns damageable component, if supports damageable,
	// or nullptr if does NOT support it!
	Damageable GetDamageable();
};

public static class MyGameObjectUtils
{
	public static bool IsDamageable(this IMyGameObject obj)
	{
		return obj.GetDamageable() != null;
	}

	public static Damageable GetDamageableChecked(this IMyGameObject obj)
	{
		Damageable damageable = obj.GetDamageable();
		Contract.Assert(damageable != null, $"Using {nameof(GetDamageableChecked)} requires that damageable is included");
		return damageable;
	}

	// @TODO: Provide some functions for area damages etc.
	// (GOOD EXERCISE for Raycasts and spatial queries!)
}
