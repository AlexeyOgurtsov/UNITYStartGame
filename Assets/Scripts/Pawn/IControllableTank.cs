public interface IControllableTank : IControllableEntity
{
	void RotateGun(float axisValue);
	void FireTurretIfCan();
	void FireAltIfCan();
}
