using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptOceanBob : MonoBehaviour
{
	public float range;
	public float speed;
	public float offset;

	private float initialY;

    //Start is called before the first frame update
    void Start()
    {
		initialY = transform.localPosition.y;

		//Offset the bob so trash isn't bobbing uniformly... creepy @-@
		offset = Random.Range(0f, 6.5f);
    }

    //Update is called once per frame
    void Update()
    {
		//Do the bob
		float y = initialY + ((Mathf.Sin(Time.time * speed + offset)) * range);
		//float y = initialY + (Mathf.Sin(Time.time * speed) * range);
		transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
	}

	void DoTheBob()
	{

	}
}
