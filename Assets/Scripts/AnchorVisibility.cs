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
            demoScript.objectsFound++;
            Debug.Log(this.gameObject.transform.GetChild(0).name);
            if(this.gameObject.transform.GetChild(0).name.Contains("ChestContainer"))
            {
                Debug.Log("Found Chest Container");
                ChestManager chestManager = this.gameObject.transform.GetChild(0).GetComponent<ChestManager>();
                Debug.Log("Chest Manager");
                chestManager.found = true;
                Debug.Log("chestmanager found");
                return;
            }

            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
            if(this.gameObject.transform.GetChild(0).name.Contains("KeyContainer"))
            {
                inventoryManager.KeyFound();
            }
            else if (this.gameObject.transform.GetChild(0).name.Contains("ShovelContainer"))
            {
                inventoryManager.ShovelFound();
            }

            Animator[] animators = this.gameObject.GetComponentsInChildren<Animator>();
            foreach (Animator a in animators)
            {
                a.enabled = true;
            }
            var renderers = this.gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                r.enabled = true;
            }
        }
    }
}
