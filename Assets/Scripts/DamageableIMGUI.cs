using UnityEngine;
using System.Diagnostics.Contracts;

public class DamageableIMGUI : MonoBehaviour
{
	public IDamageable Damageable;

	public Rect Area = new Rect(10, 10, 300, 100);

	void Awake()
	{
		Damageable = GetComponent<IDamageable>();
	}

	void OnGUI()
	{
		GUILayout.BeginArea(Area);
		GUILayout.BeginVertical();
		ShowComponentState();
		if(Damageable != null)
		{
			ShowHitsState();
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	void ShowComponentState()
	{
		if(Damageable != null)
		{
			GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
					GUILayout.Label("Class");
					GUILayout.Label(Damageable.GetType().ToString());
				GUILayout.EndHorizontal();
				if(Damageable is Behaviour behaviour)
				{
					GUILayout.BeginHorizontal();
						GUILayout.Label("isActiveAndEnabled");
						GUILayout.Label(behaviour.isActiveAndEnabled.ToString());
					GUILayout.EndHorizontal();
				}
			GUILayout.EndVertical();
		}
		else
		{
			GUILayout.Label("Damageable is null");
		}
	}

	void ShowHitsState()
	{
		string hitState = $"{Damageable.HitCount}/{Damageable.MaxHitCount}";
		GUILayout.Label(hitState);
	}
}
