using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour


{

    [SerializeField] Transform pausePos;
    [SerializeField] GameObject StartCanvas;


    // Update is called once per frame
    void Update()
    {
        StartCanvas.transform.position = pausePos.transform.position;
    }

    public void StartTheGame()
    {
        gameObject.SetActive(false);
    }
}
