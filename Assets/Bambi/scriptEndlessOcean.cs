using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptEndlessOcean : MonoBehaviour
{
	public const float maxViewDist = 100;

	public int chunkSize;
	public int chunksVisibleInViewDist;
	public GameObject chunkPfab;

	public Transform player;
	public static Vector2 playerPos;

	private Dictionary<Vector2, OceanChunk> chunks = new Dictionary<Vector2, OceanChunk>();
	List<OceanChunk> chunksVisibleLastUpdate = new List<OceanChunk>();

	// Start is called before the first frame update
	void Start()
    {
		//validation
		if (player == null)
			Debug.LogError("No player refrences assigned.", this);

		if (chunkPfab == null)
			Debug.LogError("No ocean prefab assigned.", this);
		
		chunkSize = scriptOceanManager.mapChunkSize - 1;
		chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);
    }

	private void Update()
	{
		playerPos = new Vector2(player.position.x, player.position.z);

		UpdateVisibleChunks();
	}

	void UpdateVisibleChunks()
	{
		//hide chunks that were visible
		foreach(var chunk in chunksVisibleLastUpdate)
		{
			chunk.SetVisible(false);
		}
		chunksVisibleLastUpdate.Clear();
		
		//create and update chunks
		int currentChunkCoordX = Mathf.RoundToInt(playerPos.x / chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt(playerPos.y / chunkSize);

		for (int yOffset = -chunksVisibleInViewDist; yOffset <= chunksVisibleInViewDist; yOffset++)
		{
			for (int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++)
			{
				Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

				if (chunks.ContainsKey(viewedChunkCoord))
				{
					var chunk = chunks[viewedChunkCoord];

					chunk.UpdateChunk();

					if (chunk.IsVisible())
						chunksVisibleLastUpdate.Add(chunk);
				}
				else
				{
					chunks.Add(viewedChunkCoord, new OceanChunk(viewedChunkCoord, chunkSize, chunkPfab, transform));
				}
			}
		}
	}

	public class OceanChunk
	{
		GameObject pfab;
		Vector2 pos;
		Bounds bounds;

		public OceanChunk(Vector2 coord, int size, GameObject chunkPfab, Transform parent)
		{
			pos = coord * size;
			Vector3 posV3 = new Vector3(pos.x, 0, pos.y);
			bounds = new Bounds(pos, Vector2.one * size);

			pfab = GameObject.Instantiate(chunkPfab);
			pfab.transform.position = posV3;
			pfab.transform.localScale = Vector3.one * size / 10f;
			pfab.transform.parent = parent;
			SetVisible(false);
		}

		public void UpdateChunk()
		{
			float playerDistFromEdge = Mathf.Sqrt(bounds.SqrDistance(playerPos));
			bool visible = playerDistFromEdge <= maxViewDist;
			SetVisible(visible);
		}

		public void SetVisible(bool visible)
		{
			pfab.SetActive(visible);
		}

		public bool IsVisible()
		{
			return pfab.activeSelf;
		}
	}
}
