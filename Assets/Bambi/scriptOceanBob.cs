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
    private void Start()
    {
        initialY = transform.localPosition.y;
        DoTheBob();

    }



    //Update is called once per frame
    void Update()
    {
        //Do the bob
        DoTheBob();
		
	}

    private void DoTheBob ()
    {
        float y = initialY + (Mathf.Sin(Time.time * speed) * range);
        transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
    }
}
