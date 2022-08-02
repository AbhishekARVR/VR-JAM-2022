using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simply moves forward, at a given speed
/// </summary>
public class scriptAutoMove : MonoBehaviour
{
	public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		transform.Translate(transform.forward * speed * Time.deltaTime);
	}
}
