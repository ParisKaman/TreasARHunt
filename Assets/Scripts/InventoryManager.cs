using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private GameObject inventory;

    public GameObject item1;
    public GameObject item2;

    private Transform item1Location;
    private Transform item2Location;

    public bool keyFound = false;
    public bool shovelFound = false;

    private ChestManager chestManager;
    private BasicDemoScript demoScript;

    // Start is called before the first frame update
    void Start()
    {
        item1Location = item1.transform;
        item2Location = item2.transform;
        item1.SetActive(false);
        item2.SetActive(false);
        demoScript = FindObjectOfType<BasicDemoScript>();
    }

    public void SearchBegins()
    {
        chestManager = FindObjectOfType<ChestManager>();
    }

    public void KeyFound()
    {
        item1.SetActive(true);
    }

    public void ShovelFound()
    {
        item2.SetActive(true);
    }

    public void UseKey()
    {
        if(!chestManager.found || !chestManager.unearthed)
        {
            return;
        }

        demoScript.AdvanceDemo();
    }

    public void UseShovel()
    {
        if(!chestManager.found)
        {
            return;
        }

        chestManager.unearthed = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
