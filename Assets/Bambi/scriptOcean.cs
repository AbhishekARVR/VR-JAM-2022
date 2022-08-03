using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptOcean : MonoBehaviour
{
	public GameObject pfabTrash;
	
	Bounds bounds;
	int trashQuantity;
	List<GameObject> chunkTrash = new List<GameObject>();

	private bool initialized = false;

	private void Start()
	{
		//Validation
		if (pfabTrash == null)
			Debug.LogError("Missing trash prefab reference.", this);
	}

	public void InitializeChunk(Transform parent, float size)
	{
		bounds = new Bounds(new Vector2(transform.position.x, transform.position.z), Vector2.one * size);

		transform.localScale = Vector3.one * size / 10f;
		transform.parent = parent;

		SpawnTrash();
	}

	public void UpdateChunk()
	{
		if (initialized)
		{
			float playerDistFromEdge = bounds.SqrDistance(scriptOceanManager.playerPos);

			bool visible = playerDistFromEdge <= scriptOceanManager.Instance.maxViewDist;
			SetVisible(visible);
		}
	}

	public void SpawnTrash()
	{
		trashQuantity = Random.Range(scriptOceanManager.Instance.minTrashQuantity, scriptOceanManager.Instance.maxTrashQuantity);

		while (chunkTrash.Count < trashQuantity)
		{
			var randX = Random.Range(bounds.min.x, bounds.max.x);
			var randY = Random.Range(bounds.min.y, bounds.max.y);

			var randDir = Random.Range(0f, 360f);

			//Make sure the prefab is active before instantiating
			scriptPrefabManager.Instance.TrashPrefab.SetActive(true);
			var test = scriptPrefabManager.Instance.TrashPrefab.GetComponent<scriptOceanBob>();
			test.enabled = true;

			var trash = Instantiate(scriptPrefabManager.Instance.TrashPrefab, new Vector3(randX, .35f, randY), Quaternion.Euler(randDir, randDir, randDir));
			trash.transform.parent = transform;

			chunkTrash.Add(trash);
		}

		initialized = true;
	}

	public void SetVisible(bool visible)
	{
		if (initialized && gameObject.activeSelf != visible)
		{
			gameObject.SetActive(visible);

			foreach (var trash in chunkTrash)
			{
				if (trash != null)
					trash.SetActive(visible);
			}
		}
	}

	public bool IsVisible()
	{
		return gameObject.activeSelf;
	}
}
