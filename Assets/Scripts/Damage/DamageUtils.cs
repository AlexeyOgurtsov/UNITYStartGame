using UnityEngine;
using System.Diagnostics.Contracts;

public static class DamageUtils
{
	public static int MakeDamageIfDamageableGameObject(GameObject gameObject, Damage damage, LogMode logMode = LogMode.Enable)
	{
		// if failed to find the main script, try to find the damageable component directly
		// (some objects may contain NO main script, but do contain the damageable component)
		IDamageableComponent damageable = gameObject.GetComponent<IDamageableComponent>();
		if(damageable != null)
		{
			return damageable.MakeDamage(damage);
		}
		else
		{
			LogUtils.LogWarningIf(logMode, "{gameObject.name} does NOT contain damageable component");
		}

		return 0;
	}

	public static int MakeDamageIfDamageableComponent(Object component, Damage damage, LogMode logMode = LogMode.Enable)
	{
		Contract.Assert(component != null);
		if(component is IDamageableComponent damageable)
		{
			return damageable.MakeDamage(damage);
		}
		else
		{
			if(component is Object obj)
			{
				LogUtils.LogWarningIf(logMode, $"object {obj.name} does NOT support {nameof(IDamageableComponent)} interface!");
			}
		}
		return 0;
	}

	public static void SetInstigatorForDamageSubsystemComponents(GameObject gameObject, DamageInstigator instigator)
	{
		foreach(IDamageSubsystemComponent component in gameObject.GetComponents<IDamageSubsystemComponent>())
		{
			component.Instigator = instigator;
		}
	}
}
