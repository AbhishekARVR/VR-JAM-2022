using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptOcean : MonoBehaviour
{
	public GameObject pfabTrash;
	
	Bounds bounds;
	int trashQuantity;
	int trashMin = 10;
	List<GameObject> chunkTrash = new List<GameObject>();


	//Coroutines
	private IEnumerator spawnTrashRoutine;

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

		SpawnTrash(scriptOceanManager.maxTrashQuantity);
	}

	public void UpdateChunk()
	{
		float playerDistFromEdge = Mathf.Sqrt(bounds.SqrDistance(scriptOceanManager.playerPos));
		bool visible = playerDistFromEdge <= scriptOceanManager.maxViewDist;
		SetVisible(visible);
	}

	public void SpawnTrash(int trashMax)
	{
		//spawnTrashRoutine = SpawnTrashRoutine(trashMax);
		//StartCoroutine(spawnTrashRoutine);

		trashQuantity = Random.Range(trashMin, trashMax);

		for (int i = 0; i < trashQuantity; i++)
		{
			var randX = Random.Range(bounds.min.x, bounds.max.x);
			var randY = Random.Range(bounds.min.y, bounds.max.y);

			var trash = Instantiate(pfabTrash, new Vector3(randX, .35f, randY), Quaternion.identity);
			trash.transform.parent = transform;

			chunkTrash.Add(trash);
		}
	}

	public void SetVisible(bool visible)
	{
		if (gameObject.activeSelf != visible)
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

	//Coroutine
	private IEnumerator SpawnTrashRoutine(int trashMax)
	{
		//trashQuantity = Random.Range(trashMin, trashMax);

		//for (int i = 0; i < trashQuantity; i++)
		//{
			yield return new WaitForSeconds(.1f);

		//	var randX = Random.Range(bounds.min.x, bounds.max.x);
		//	var randY = Random.Range(bounds.min.y, bounds.max.y);

		//	var trash = Instantiate(pfabTrash, new Vector3(randX, .35f, randY), Quaternion.identity);
		//	trash.transform.parent = transform;

		//	chunkTrash.Add(trash);
		//}

		//SetVisible(false);

		spawnTrashRoutine = null;
	}
}
