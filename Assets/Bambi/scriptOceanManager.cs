using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptOceanManager : MonoBehaviour
{
	static public scriptOceanManager Instance;

	//[Range(0, 6)]
	//public int levelOfDetail;

	public float maxViewDist = 10000;
	public int mapChunkSize = 50;
	public int maxTrashQuantity = 5;
	public int minTrashQuantity = 1;
	private int chunksVisibleInViewDist;
	public GameObject chunkPfab;

	public Transform player;
	public static Vector2 playerPos;

	private Dictionary<Vector2, GameObject> chunks = new Dictionary<Vector2, GameObject>();
	List<scriptOcean> chunksVisibleLastUpdate = new List<scriptOcean>();

	// Start is called before the first frame update
	void Start()
    {
		//validation
		if (Instance == null)
			Instance = this;
		else
			Debug.LogError("An ocean manager already exists in the scene!", Instance);

		if (player == null)
			Debug.LogError("No player refrence assigned.", this);

		if (chunkPfab == null)
			Debug.LogError("No ocean prefab assigned.", this);

		chunksVisibleInViewDist = Mathf.RoundToInt(Mathf.Sqrt(maxViewDist) / mapChunkSize);
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
		int currentChunkCoordX = Mathf.RoundToInt(playerPos.x / mapChunkSize);
		int currentChunkCoordY = Mathf.RoundToInt(playerPos.y / mapChunkSize);

		for (int yOffset = -chunksVisibleInViewDist; yOffset <= chunksVisibleInViewDist; yOffset++)
		{
			for (int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++)
			{
				Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

				if (chunks.ContainsKey(viewedChunkCoord))
				{
					var chunk = chunks[viewedChunkCoord].GetComponent<scriptOcean>();

					chunk.UpdateChunk();

					if (chunk.IsVisible())
						chunksVisibleLastUpdate.Add(chunk.GetComponent<scriptOcean>());
				}
				else
				{
					Vector2 pos = viewedChunkCoord * mapChunkSize;

					//Make sure prefab is active before instantiation
					scriptPrefabManager.Instance.OceanPrefab.SetActive(true);

					var newChunk = Instantiate(scriptPrefabManager.Instance.OceanPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
					newChunk.GetComponent<scriptOcean>().InitializeChunk(transform, mapChunkSize);

					chunks.Add(viewedChunkCoord, newChunk);
				}
			}
		}
	}
}
