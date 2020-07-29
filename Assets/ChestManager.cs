using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    [SerializeField]
    private GameObject chestPrefab;
    [SerializeField]
    private GameObject dirtPrefab;

    public bool dirtShowing = false;
    public bool chestShowing = true;

    public bool unearthed = false;
    public bool placing = true;
    public bool found = false;

    // Start is called before the first frame update
    void Start()
    {
        //dirtShowing = dirtPrefab.transform.GetChild(0).GetComponent<MeshRenderer>().enabled;
    }

    public void HideChest()
    {
        if(!chestShowing)
        {
            return;
        }

        Animator[] animators = chestPrefab.gameObject.GetComponentsInChildren<Animator>();
        foreach (Animator a in animators)
        {
            a.enabled = false;
        }
        var renderers = chestPrefab.gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }
        chestShowing = false;
    }

    public void ShowChest()
    {
        if(chestShowing)
        {
            return;
        }
        Debug.Log("Showing Chest");
        Animator[] animators = chestPrefab.gameObject.GetComponentsInChildren<Animator>();
        foreach (Animator a in animators)
        {
            a.enabled = true;
        }
        var renderers = chestPrefab.gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = true;
        }
        chestShowing = true;
    }

    public void HideDirt()
    {
        if(!dirtShowing)
        {
            return;
        }
        Debug.Log("Hiding Dirt");
        var renderers = dirtPrefab.gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }
        dirtShowing = false;
    }

    public void ShowDirt()
    {
        if(dirtShowing)
        {
            return;
        }
        Debug.Log("Showing Dirt");
        var renderers = dirtPrefab.gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = true;
        }
        dirtShowing = true;
    }

    // Update is called once per frame
    void Update()
    {
        //if placing, disable dirt so we are just showing chest
        if(placing)
        {
            Debug.Log("Placing");
            HideDirt();
        }
        else if (!found)
        {
            //Debug.Log("Hidden");
            HideDirt();
            HideChest();
        }
        else
        {

            ShowDirt();
            if(!unearthed)
            {
                Debug.Log("Just Dirt");
                HideChest();
            }
            else
            {
                Debug.Log("Unearthed");
                HideDirt();
                ShowChest();
            }
        }
    }
}
