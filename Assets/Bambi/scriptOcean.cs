using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptOcean : MonoBehaviour
{
	public List<GameObject> pfabsTrash = new List<GameObject>();
	
	public Bounds bounds;
	int trashQuantity;
	List<GameObject> chunkTrash = new List<GameObject>();

	private bool initialized = false;

	private void Start()
	{
		//Validation
		if (pfabsTrash.Count == 0)
			Debug.LogError("Missing trash prefab reference.", this);
	}

	public Bounds InitializeChunk(Transform parent, float size)
	{
		bounds = new Bounds(new Vector2(transform.position.x, transform.position.z), Vector2.one * size);

		transform.localScale = Vector3.one * size / 10f;
		transform.parent = parent;

		//SpawnTrash();

		initialized = true;

		return bounds;
	}

	public bool UpdateOceanVisibility()
	{
		bool isVisible = true;
		
		if (initialized)
		{
			//Remove collected trash from chunkTrash list
			//RemoveCollectedTrash();
			
			float playerDistFromEdge = bounds.SqrDistance(scriptOceanManager.playerPos);

			isVisible = playerDistFromEdge <= scriptOceanManager.Instance.maxViewDist;
			SetVisible(isVisible);
		}

		return isVisible;
	}

	/// <summary>
	/// If the player has scooped up pieces of trash in this chunk, we don't want those to disappear when the player moves too far away.
	/// </summary>
	//public void RemoveCollectedTrash()
	//{
	//	List<GameObject> trashToRemove = new List<GameObject>();

	//	foreach (GameObject trash in chunkTrash)
	//	{
	//		if (trash.GetComponent<scriptTrash>().isCollected)
	//		{
	//			trashToRemove.Add(trash);
	//		}
	//	}

	//	foreach(GameObject trash in trashToRemove)
	//	{
	//		chunkTrash.Remove(trash);
	//	}
	//}

	public void SetVisible(bool visible)
	{
		if (initialized && gameObject.activeSelf != visible)
		{
			gameObject.SetActive(visible);

			//foreach (var trash in chunkTrash)
			//{
			//	if (trash != null)
			//		trash.SetActive(visible);
			//}
		}
	}

	public bool IsVisible()
	{
		return gameObject.activeSelf;
	}
}
