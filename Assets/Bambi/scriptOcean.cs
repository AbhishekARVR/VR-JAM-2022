using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptOcean : MonoBehaviour
{
	public List<GameObject> pfabsTrash = new List<GameObject>();
	
	Bounds bounds;
	int trashQuantity;
	List<GameObject> chunkTrash = new List<GameObject>();

	private bool initialized = false;

	private void Start()
	{
		//Validation
		if (pfabsTrash.Count == 0)
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
			//Remove collected trash from chunkTrash list
			RemoveCollectedTrash();
			
			float playerDistFromEdge = bounds.SqrDistance(scriptOceanManager.playerPos);

			bool visible = playerDistFromEdge <= scriptOceanManager.Instance.maxViewDist;
			SetVisible(visible);
		}
	}

	/// <summary>
	/// If the player has scooped up pieces of trash in this chunk, we don't want those to disappear when the player moves too far away.
	/// </summary>
	public void RemoveCollectedTrash()
	{
		List<GameObject> trashToRemove = new List<GameObject>();

		foreach (GameObject trash in chunkTrash)
		{
			if (trash.GetComponent<scriptTrash>().isCollected)
			{
				trashToRemove.Add(trash);
			}
		}

		foreach(GameObject trash in trashToRemove)
		{
			chunkTrash.Remove(trash);
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
			//scriptPrefabManager.Instance.TrashPrefab.SetActive(true);
			//var test = scriptPrefabManager.Instance.TrashPrefab.GetComponent<scriptOceanBob>();
			//test.enabled = true;

			var randomIndexForTrash = Random.Range(0, pfabsTrash.Count);
			var pfabTrash = pfabsTrash[randomIndexForTrash];
			Debug.Log("trash==" + randomIndexForTrash);
			var trash = Instantiate(pfabTrash, new Vector3(randX, .35f, randY), Quaternion.Euler(randDir, randDir, randDir));
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
