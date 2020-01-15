using UnityEngine;
using System.Reflection;
using System.Diagnostics.Contracts;

// @TODO BUGs
// No projectile visible

// @TODO
// Show muzzle
// Projectile property validation
// Field grouping
// Consider event support

public class SimpleWeapon : MonoBehaviour
{
	public const float DEFAULT_MIN_SECONDS_BETWEEN_SHOTS = 1;
	public const int DEFAULT_SHOT_COUNT = 10;

	public GameObject ProjectileTemplate;
	public float MinSecondsBetweenShots = DEFAULT_MIN_SECONDS_BETWEEN_SHOTS;
	public int ShotCount = DEFAULT_SHOT_COUNT;
	public bool IsShotCountLimited = true;
	public Vector2 MuzzleOffset; 

	public bool bLogGameplayFailsInsideCanFire = false;

	public float MinAllowedNextShotTime
	{
		get;
		private set;
	} = 0;

	public void FireIfCan()
	{
		if (CanFireNow())
		{
			DoFire();
		}
	}

	public bool CanFireNow()
	{
		float currentTime = Time.time;
		if (!CanFireDueToNextShotTime(currentTime))
		{
			LogFireDueToNextShotTime_IfLoggingItEnabled(currentTime);
			return false;
		}
		if (!CanFireDueToShotCount())
		{
			LogFireDueToShotCount_IfLoggingItEnabled();
			return false;
		}
		return true;
	}

	bool CanFireDueToNextShotTime(float currentTime) => currentTime >= MinAllowedNextShotTime;
	void LogFireDueToNextShotTime_IfLoggingItEnabled(float currentTime)
	{
		if(bLogGameplayFailsInsideCanFire)
		{
			Debug.Log($"Gameplay prohibits fire due to next shot time: {nameof(currentTime)}={currentTime}; {nameof(MinAllowedNextShotTime)}={MinAllowedNextShotTime}");
		}
	}

	bool CanFireDueToShotCount() => !IsShotCountLimited || ShotCount > 0;
	void LogFireDueToShotCount_IfLoggingItEnabled()
	{
		if(bLogGameplayFailsInsideCanFire)
		{
			Debug.Log($"Gameplay prohibits fire due to shot count: {nameof(IsShotCountLimited)}={IsShotCountLimited}; {nameof(ShotCount)}={ShotCount}");
		}
	}

	void DoFire()
	{		
		if (!Instantiate(ProjectileTemplate, ComputeMuzzleWorldPosition3D(), transform.rotation))
		{
			Debug.LogError($"Failed to instantiate projectile of class \"{ProjectileTemplate?.GetType()}\"");
			return;
		}

		MinAllowedNextShotTime = Time.time + MinSecondsBetweenShots;
		DecreaseShotCountIfShould();
	}

	Vector3 ComputeMuzzleWorldPosition3D()
	{
		return transform.position + (Vector3)MuzzleOffset;
	}

	void DecreaseShotCountIfShould()
	{
		if (IsShotCountLimited)
		{
			Contract.Assert(ShotCount > 0, $"This branch should only be executed if we have at least one shot");
			ShotCount--;
		}
	}
}
