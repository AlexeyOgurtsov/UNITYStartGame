public interface IControllableEntity
{
	// Fire from weapon if can
	void Fire(int FireIndex);
	// Thrust (to be called each tick when thrusting is to be performed)
	void Thrust(float axisValue);
	// Rotate (to be called each tick when rotation is performed)
	void Rotate(float axisValue);
}
