using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectPlayer : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Boat"))
        {
            //fades out sailing music, so you can hear the spatial scav radio audio source
            AudioManager.Instance.FadeScavIn();
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Boat"))
        {
            //fades in sailing music as you leave spatial scav radio audio source
            AudioManager.Instance.FadeScavOut();
        }
        
    }
}
