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
	[Tooltip("How many chunks does the player have to explore before finding something interesting.")]
	public int pointOfInterestSpawnRate;
	[Tooltip("Offset the POIs so we don't spawn near the starting barge.")]
	public int pointOfInterestOffset;
	[Tooltip("How many barges have to spawn before a rig appears.")]
	public int rigMultiplier;

	public GameObject oceanPfab;
	public GameObject rigPfab;
	public GameObject bargePfab;
	public List<GameObject> trashPfabs = new List<GameObject>();

	public Transform player;
	public static Vector2 playerPos;

	private int chunksVisibleInViewDist;
	private Dictionary<Vector2, OceanChunk> chunks = new Dictionary<Vector2, OceanChunk>();
	List<OceanChunk> chunksVisible = new List<OceanChunk>();

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

		var playerObj = GameObject.FindGameObjectWithTag("Player");
		if (playerObj == null)
			Debug.LogError("No player object found.", this);

		if (oceanPfab == null)
			Debug.LogError("No ocean prefab assigned.", this);

		if (rigPfab == null)
			Debug.LogError("No rig prefab assigned.", this);

		if (bargePfab == null)
			Debug.LogError("No barge prefab assigned.", this);

		if (pointOfInterestSpawnRate == 0)
			Debug.LogError("POI spawn rate cannot be 0.", this);

		if (rigMultiplier == 0)
			Debug.LogError("Rig multiplier cannot be 0.", this);

		//initialize values
		player = playerObj.transform;
		chunksVisibleInViewDist = Mathf.RoundToInt(Mathf.Sqrt(maxViewDist) / mapChunkSize);
	}

	private void Update()
	{
		playerPos = new Vector2(player.position.x, player.position.z);

		UpdateChunks();
	}

	void UpdateChunks()
	{
		int currentChunkCoordX = Mathf.RoundToInt(playerPos.x / mapChunkSize);
		int currentChunkCoordY = Mathf.RoundToInt(playerPos.y / mapChunkSize);

		//Create any new chunks we discovered
		for (int yOffset = -chunksVisibleInViewDist; yOffset <= chunksVisibleInViewDist; yOffset++)
		{
			for (int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++)
			{
				Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

				if (chunks.TryGetValue(viewedChunkCoord, out OceanChunk chunk))
				{
					if (!chunksVisible.Contains(chunk)) //then the chunk was previously initialized, hidden, and is now coming back into view.
						chunksVisible.Add(chunk);
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

					chunksVisible.Add(newChunk);
				}
			}
		}

		//Update and clean chunks
		List<OceanChunk> chunksToRemove = new List<OceanChunk>();

		foreach(OceanChunk chunk in chunksVisible)
		{
			var ocean = chunk.ocean.GetComponent<scriptOcean>();

			//Check if we need to hide the chunk
			bool isVisible = ocean.UpdateOceanVisibility();
			UpdateObjectVisibility(isVisible, chunk.objects);

			if (isVisible) //then do object activities
			{
				//bob the trash
				if (chunk.type == ChunkType.Trash)
					BobTrash(chunk.objects);
			}
			else
			{
				chunksToRemove.Add(chunk);
			}
		}

		//Remove chunks no longer visible
		foreach(var chunk in chunksToRemove)
		{
			chunksVisible.Remove(chunk);
		}
	}

	public (ChunkType type, List<GameObject> objs) SpawnChunkObjects(Bounds bounds)
	{
		//check if we should spawn a rig
		if (chunks.Count > 0 && chunks.Count % (pointOfInterestSpawnRate * rigMultiplier + pointOfInterestOffset) == 0)
			return (ChunkType.Rig, new List<GameObject>()
			{
				Instantiate(rigPfab, new Vector3(bounds.center.x, 0, bounds.center.y), Quaternion.Euler(0, Random.Range(0, 359), 0), transform)
			});

		//check if we should spawn a barge
		if (chunks.Count > 0 && chunks.Count % pointOfInterestSpawnRate + pointOfInterestOffset == 0)
			return (ChunkType.Barge, new List<GameObject>()
			{
				Instantiate(bargePfab, new Vector3(bounds.center.x, 0, bounds.center.y), Quaternion.Euler(0, Random.Range(0, 359), 0), transform)
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

			var trash = Instantiate(pfabTrash, new Vector3(randX, .35f, randY), Quaternion.Euler(randDir, randDir, randDir), transform);

			chunkTrash.Add(trash);
		}

		return chunkTrash;
	}

	public void UpdateObjectVisibility(bool isVisible, List<GameObject> objs)
	{
		//No need to loop through objects if our visibility hasn't changed.
		if (objs.Count == 0 || objs[0].activeSelf == isVisible)
			return;

		foreach (GameObject obj in objs)
		{
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
