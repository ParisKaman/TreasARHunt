using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorVisibility : MonoBehaviour
{

    private bool objectFound = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        BasicDemoScript demoScript = FindObjectOfType<BasicDemoScript>();
        float distanceToWaypoint = Vector3.Distance(this.gameObject.transform.position,Camera.main.transform.position);
        if(distanceToWaypoint < 1 && demoScript.searching && !objectFound)
        {
            objectFound = true;
            Animator[] animators = this.gameObject.GetComponentsInChildren<Animator>();
            foreach (Animator a in animators)
            {
                a.enabled = true;
            }
            var meshRenderers = this.gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer m in meshRenderers)
            {
                m.enabled = true;
            }
            //this.gameObject.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().enabled = true;
            FindObjectOfType<BasicDemoScript>().objectsFound++;
        }
    }
}
