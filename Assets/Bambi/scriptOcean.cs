using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptOcean : MonoBehaviour
{
	public List<GameObject> pfabsTrash = new List<GameObject>();
	
	public Bounds bounds;

	private bool initialized = false;

	private void Start()
	{
		//Validation
		if (pfabsTrash.Count == 0)
			Debug.LogError("Missing trash prefab references.", this);
	}

	public Bounds InitializeChunk(Transform parent, float size)
	{
		bounds = new Bounds(new Vector2(transform.position.x, transform.position.z), Vector2.one * size);

		transform.localScale = Vector3.one * size / 10f;
		transform.parent = parent;

		initialized = true;

		return bounds;
	}

	public bool UpdateOceanVisibility()
	{
		bool isVisible = true;
		
		if (initialized)
		{
			float playerDistFromEdge = bounds.SqrDistance(scriptOceanManager.playerPos);

			isVisible = playerDistFromEdge <= scriptOceanManager.Instance.maxViewDist;
			SetVisible(isVisible);
		}

		return isVisible;
	}

	public void SetVisible(bool visible)
	{
		if (initialized && gameObject.activeSelf != visible)
		{
			gameObject.SetActive(visible);
		}
	}

	public bool IsVisible()
	{
		return gameObject.activeSelf;
	}
}
