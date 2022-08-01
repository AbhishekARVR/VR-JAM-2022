using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptOceanBob : MonoBehaviour
{
	public float range;
	public float speed;
	public float tolerance;

	private float initialY;

    //Start is called before the first frame update
    void Start()
    {
		initialY = transform.localPosition.y;
    }

    //Update is called once per frame
    void Update()
    {
		//Do the bob
		float y = initialY + (Mathf.Sin(Time.time * speed) * range);
		transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
	}
}
