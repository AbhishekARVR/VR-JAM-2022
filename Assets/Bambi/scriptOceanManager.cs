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
	public float oceanPlane; //the y axis plane on which trash should float.
	public int maxTrashQuantity = 5;
	public int minTrashQuantity = 1;
	private int chunksVisibleInViewDist;
	public GameObject chunkPfab;
	public List<GameObject> trashPfabs = new List<GameObject>();

	public Transform player;
	public static Vector2 playerPos;

	private Dictionary<Vector2, OceanChunk> chunks = new Dictionary<Vector2, OceanChunk>();
	List<scriptOcean> chunksVisibleLastUpdate = new List<scriptOcean>();

	public enum ChunkType
	{
		Trash,
		Barge,
		Rig
	}

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

		UpdateChunks();
	}

	void UpdateChunks()
	{
		//hide chunks that were visible
		foreach (var chunk in chunksVisibleLastUpdate)
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

				if (chunks.TryGetValue(viewedChunkCoord, out OceanChunk chunk))
				{
					chunk.lastViewedChunkCoord = viewedChunkCoord;

					var ocean = chunk.ocean.GetComponent<scriptOcean>();

					//Check if we need to hide the chunk
					bool isVisible = ocean.UpdateOceanVisibility();
					UpdateTrashVisibility(isVisible, chunk.trashObjs);

					if (isVisible)
					{
						//bob the trash
						BobTrash(chunk.trashObjs);

						//add to visible list
						chunksVisibleLastUpdate.Add(ocean.GetComponent<scriptOcean>());
					}
				}
				else
				{
					Vector2 pos = viewedChunkCoord * mapChunkSize;

					//Make sure prefab is active before instantiation
					scriptPrefabManager.Instance.OceanPrefab.SetActive(true);

					var ocean = Instantiate(scriptPrefabManager.Instance.OceanPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
					var oceanScript = ocean.GetComponent<scriptOcean>();

					var trash = SpawnTrash(oceanScript.InitializeChunk(transform, mapChunkSize));

					var newChunk = new OceanChunk { type = ChunkType.Trash, lastViewedChunkCoord = viewedChunkCoord, ocean = ocean, trashObjs = trash };

					chunks.Add(viewedChunkCoord, newChunk);
				}
			}
		}
	}

	public List<GameObject> SpawnTrash(Bounds bounds)
	{
		List<GameObject> chunkTrash = new List<GameObject>();
		
		var trashQuantity = Random.Range(scriptOceanManager.Instance.minTrashQuantity, scriptOceanManager.Instance.maxTrashQuantity);

		while (chunkTrash.Count < trashQuantity)
		{
			var randX = Random.Range(bounds.min.x, bounds.max.x);
			var randY = Random.Range(bounds.min.y, bounds.max.y);

			var randDir = Random.Range(0f, 360f);

			var randomIndexForTrash = Random.Range(0, trashPfabs.Count);
			var pfabTrash = trashPfabs[randomIndexForTrash];
			pfabTrash.SetActive(true); //make sure pfab is active before spawning.

			//Debug.Log("trash==" + randomIndexForTrash);

			var trash = Instantiate(pfabTrash, new Vector3(randX, .35f, randY), Quaternion.Euler(randDir, randDir, randDir));
			trash.transform.parent = transform;

			chunkTrash.Add(trash);
		}

		return chunkTrash;
	}

	public void UpdateTrashVisibility(bool isVisible, List<GameObject> trashObjs)
	{
		foreach(GameObject trash in trashObjs)
		{
			//No need to loop through if everyone is already set to the correct value.
			if (trash.activeSelf == isVisible)
				break;

			trash.SetActive(isVisible);
		}
	}

	public void BobTrash(List<GameObject> trashObjs)
	{
		foreach(GameObject trash in trashObjs)
		{
			var bobScript = trash.GetComponent<scriptOceanBob>();

			bobScript.DoTheBob();
		}
	}

	/// <summary>
	/// Once a piece of trash has been collected, we don't need to worry about visability anymore.
	/// </summary>
	/// <param name="key"></param>
	/// <param name="trashToRemove"></param>
	public void RemoveCollectedTrash(Vector3 pos, GameObject trashToRemove)
	{
		int keyX = Mathf.RoundToInt(pos.x / mapChunkSize);
		int keyY = Mathf.RoundToInt(pos.z / Instance.mapChunkSize);
		Vector2 key = new Vector2(keyX, keyY);

		if (chunks.TryGetValue(key, out OceanChunk ocean))
		{
			ocean.trashObjs.Remove(trashToRemove);
		}
	}

	public class OceanChunk
	{
		public ChunkType type;

		public Vector2 lastViewedChunkCoord;

		public GameObject ocean;

		public List<GameObject> trashObjs;
	}
}
