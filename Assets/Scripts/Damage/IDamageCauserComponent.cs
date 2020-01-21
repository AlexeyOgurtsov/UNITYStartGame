using UnityEngine;

public interface IDamageCauserComponent : IDamageSubsystemComponent
{
	bool ShouldDamageInstigator { get; set; }
}
