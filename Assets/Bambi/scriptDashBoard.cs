using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scriptDashBoard : MonoBehaviour
{
	public Text txtTrashCount;

	// Start is called before the first frame update
	void Start()
    {
		//validation
		if (txtTrashCount == null)
			Debug.LogError("Missing trash count text reference.", this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void updateTrashCount(int trashCount)
	{
		txtTrashCount.text = $"Trash: {trashCount}";
	}
}
