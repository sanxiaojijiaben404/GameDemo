using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TriggerSceneSwitch : MonoBehaviour
{
    public int index; 
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            GetComponent<SceneSwitch>().LoadGameScene(index);
           
        }
    }
}
