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
    private void Start()
    {
		initialY = transform.localPosition.y;

		//Offset the bob so trash isn't bobbing uniformly... creepy @-@
		offset = Random.Range(0f, 6.5f);

        initialY = transform.localPosition.y;
        DoTheBob();
    }

    //Update is called once per frame
    void Update()
    {
        DoTheBob();
	}

    private void DoTheBob ()
    {
        float y = initialY + ((Mathf.Sin(Time.time * speed + offset)) * range);
		transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
    }
}
