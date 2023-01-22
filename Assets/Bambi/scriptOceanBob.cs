using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptOceanBob : MonoBehaviour
{
	public float range;
	public float speed;

	private float offset;

    //Start is called before the first frame update
    private void Start()
    {
		//Offset the bob so trash isn't bobbing uniformly... creepy @-@
		offset = Random.Range(0f, 6.5f);

		//DoTheBob();
	}

    void FixedUpdate()
    {
		DoTheBob();
	}

    public void DoTheBob()
    {
        float y = scriptOceanManager.Instance.oceanPlane + ((Mathf.Sin(Time.time * speed + offset)) * range);
		transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
