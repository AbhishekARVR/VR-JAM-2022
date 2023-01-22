using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptRadio : MonoBehaviour
{
	public bool randomPlay = true;
	private AudioSource aSrc;

    void Awake()
    {
		aSrc = GetComponent<AudioSource>();

		if (aSrc == null)
			Debug.LogError("No audio source found on radio.", this);

		//Play music at a random point in the clip so approaching the barge doesn't sound the same every time.
		if (randomPlay)
			aSrc.time = Random.Range(0f, aSrc.clip.length);
    }
}
