using UnityEngine;

public interface IDamageCauserComponent
{
	GameObject InstigatorPawn { get; set; }
	bool ShouldDamageInstigator { get; set; }
}
