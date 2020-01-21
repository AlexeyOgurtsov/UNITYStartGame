using UnityEngine;

public enum LogMode
{
	Disable,
	Enable
}

public static class LogUtils
{
	public static void LogIf(LogMode logMode, string message)
	{
		LogIf(logMode != LogMode.Disable, message);
	}

	public static void LogIf(bool shouldLog, string message)
	{
		if(shouldLog)
		{
			Debug.Log(message);
		}
	}

	public static void LogWarningIf(LogMode logMode, string message)
	{
		LogWarningIf(logMode != LogMode.Disable, message);
	}

	public static void LogWarningIf(bool shouldLog, string message)
	{
		if(shouldLog)
		{
			Debug.LogWarning(message);
		}
	}

	public static void LogErrorIf(LogMode logMode, string message)
	{
		LogErrorIf(logMode != LogMode.Disable, message);
	}

	public static void LogErrorIf(bool shouldLog, string message)
	{
		if(shouldLog)
		{
			Debug.LogError(message);
		}
	}
}
