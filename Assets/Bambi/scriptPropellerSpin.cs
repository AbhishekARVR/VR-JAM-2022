using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptPropellerSpin : MonoBehaviour
{
	public float speed;
	public Vector3 spinDirection;

    // Update is called once per frame
    void Update()
    {
		transform.Rotate(spinDirection * Time.deltaTime * speed);
	}
}
