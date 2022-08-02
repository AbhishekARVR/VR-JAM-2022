using UnityEngine;

public class scriptPrefabManager : MonoBehaviour
{
	// Assign the prefab in the inspector
	public GameObject OceanPrefab;
	public GameObject TrashPrefab;
	//Singleton
	private static scriptPrefabManager m_Instance = null;
	public static scriptPrefabManager Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = (scriptPrefabManager)FindObjectOfType(typeof(scriptPrefabManager));
			}
			return m_Instance;
		}
	}
}