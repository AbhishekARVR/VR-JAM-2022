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
	[Tooltip("The y axis plane on which trash should float.")]
	public float oceanPlane;
	[Tooltip("The maximum number of trash a chunk can contain.")]
	public int maxTrashQuantity = 5;
	[Tooltip("The minimum number of trash a chunk can contain.")]
	public int minTrashQuantity = 1;
	[Tooltip("How many chunks does the player have to explore before finding a rig.")]
	public int rigSpawnRate;
	[Tooltip("How many chunks does the player have to explore before finding a barge.")]
	public int bargeSpawnRate;

	public GameObject oceanPfab;
	public GameObject rigPfab;
	public GameObject bargePfab;
	public List<GameObject> trashPfabs = new List<GameObject>();

	public Transform player;
	public static Vector2 playerPos;

	private int chunksVisibleInViewDist;
	private Dictionary<Vector2, OceanChunk> chunks = new Dictionary<Vector2, OceanChunk>();
	List<scriptOcean> chunksVisibleLastUpdate = new List<scriptOcean>();

	public enum ChunkType
	{
		Trash,
		Barge,
		Rig
	}

	void Start()
    {
		//validation
		if (Instance == null)
			Instance = this;
		else
			Debug.LogError("An ocean manager already exists in the scene!", Instance);

		if (player == null)
			Debug.LogError("No player refrence assigned.", this);

		if (oceanPfab == null)
			Debug.LogError("No ocean prefab assigned.", this);

		if (rigPfab == null)
			Debug.LogError("No rig prefab assigned.", this);

		if (bargePfab == null)
			Debug.LogError("No barge prefab assigned.", this);

		if (rigSpawnRate == 0)
			Debug.LogError("Rig spawn rate cannot be 0.", this);

		if (bargeSpawnRate == 0)
			Debug.LogError("Barge spawn rate cannot be 0.", this);

		//initialize values
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
					var ocean = chunk.ocean.GetComponent<scriptOcean>();

					//Check if we need to hide the chunk
					bool isVisible = ocean.UpdateOceanVisibility();
					UpdateObjectVisibility(isVisible, chunk.objects);

					if (isVisible)
					{
						//bob the trash
						if (chunk.type == ChunkType.Trash)
							BobTrash(chunk.objects);

						//add to visible list
						chunksVisibleLastUpdate.Add(ocean.GetComponent<scriptOcean>());
					}
				}
				else
				{
					Vector2 pos = viewedChunkCoord * mapChunkSize;

					//Make sure prefab is active before instantiation
					oceanPfab.SetActive(true);

					var ocean = Instantiate(oceanPfab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
					var oceanScript = ocean.GetComponent<scriptOcean>();

					var chunkObjs = SpawnChunkObjects(oceanScript.InitializeChunk(transform, mapChunkSize));

					var newChunk = new OceanChunk { type = chunkObjs.type, ocean = ocean, objects = chunkObjs.objs };

					chunks.Add(viewedChunkCoord, newChunk);
				}
			}
		}
	}

	public (ChunkType type, List<GameObject> objs) SpawnChunkObjects(Bounds bounds)
	{
		//check if we should spawn a rig
		if (chunks.Count % rigSpawnRate == 0)
			return (ChunkType.Rig, new List<GameObject>()
			{
				Instantiate(rigPfab, new Vector3(bounds.center.x, 0, bounds.center.y), Quaternion.identity)
			});

		//check if we should spawn a barge
		if (chunks.Count % bargeSpawnRate == 0)
			return (ChunkType.Barge, new List<GameObject>()
			{
				Instantiate(bargePfab, new Vector3(bounds.center.x, 0, bounds.center.y), Quaternion.identity)
			});

		//else spawn trash
		return (ChunkType.Trash, SpawnTrash(bounds));
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

	public void UpdateObjectVisibility(bool isVisible, List<GameObject> objs)
	{
		foreach(GameObject obj in objs)
		{
			//No need to loop through if everyone is already set to the correct value.
			if (obj.activeSelf == isVisible)
				break;

			obj.SetActive(isVisible);
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
			if (ocean.type == ChunkType.Trash)
				ocean.objects.Remove(trashToRemove);
			else
				Debug.LogWarning("Tried to remove trash from a non trash chunk.", trashToRemove);
		}
		else
			Debug.LogError("Could not find the specified chunk.", this);
	}

	public class OceanChunk
	{
		public ChunkType type;
		
		public GameObject ocean;

		public List<GameObject> objects;
	}
}
