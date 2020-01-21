using UnityEngine;

[AddComponentMenu("Damage/DamageFacade")]
[DisallowMultipleComponent]
public class DamageFacadeMonoBehaviour : MonoBehaviour, IDamageFacadeComponent
{
	public IDamageableComponent Damageable { get; private set; }
	public IDamageCauserComponent[] DamageCausers { get; private set; }

	public void SetInstigatorForAllComponents(DamageInstigator instigator)
	{
		if(Damageable != null)
		{
			Damageable.Instigator = instigator;
		}
		foreach(IDamageCauserComponent causer in DamageCausers)
		{
			causer.Instigator = instigator;
		}
	}

	void Awake()
	{
		Damageable = GetComponent<IDamageableComponent>();
		DamageCausers = GetComponents<IDamageCauserComponent>();
	}
}
