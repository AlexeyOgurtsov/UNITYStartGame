using UnityEngine;
using System.Diagnostics.Contracts;

[AddComponentMenu("Damage/DamageableIMGUI")]
public class DamageableIMGUIMonoBehaviour : MonoBehaviour, IDamageableGUIComponent
{
	public Rect Area = new Rect(10, 10, 300, 100);

	public IDamageableComponent Damageable { get; set; }

	void Awake()
	{
		Damageable = GetComponent<IDamageableComponent>();
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
