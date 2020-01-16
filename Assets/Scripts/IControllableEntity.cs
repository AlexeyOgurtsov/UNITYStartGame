// Like pawn in UE4
public interface IControllableEntity : IMyGameObject
{
	// Fire (to be called each time when firing is to be performed)
	void FireIfCan(int fireIndex);
	// Thrust (to be called each tick when thrusting is to be performed)
	void Thrust(float axisValue);
	// Rotate (to be called each tick when rotation is performed)
	void Rotate(float axisValue);
}
