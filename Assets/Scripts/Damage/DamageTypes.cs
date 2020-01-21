using UnityEngine;

public struct DamageInstigator
{
	public TankPlayerController PlayerController;
	public GameObject Pawn;

	public DamageInstigator(TankPlayerController playerController, GameObject pawn)
	{
		this.PlayerController = playerController;
		this.Pawn = pawn;
	}
};

public struct Damage
{
	public int Amount;
	public DamageInstigator Instigator;

	public Damage(int amount, DamageInstigator instigator)
	{
		this.Amount = amount;
		this.Instigator = instigator;
	}
};

