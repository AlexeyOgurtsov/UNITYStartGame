using UnityEngine;

public interface IDamageCauser
{
	GameObject InstigatorPawn { get; set; }
	bool ShouldDamageInstigator { get; set; }
}
