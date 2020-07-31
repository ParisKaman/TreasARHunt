using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;


public class ARTapToPlace : MonoBehaviour
{
    private ARRaycastManager aRRaycastManager;
    private ARAnchorManager aRAnchorManager;
    public List<ARAnchor> aRAnchors;
    public Pose placementPose;
    volatile public bool isPlacementValid;
    public GameObject placementIndicator;
    public GameObject objectToPlace;
    public List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public GameObject indicator;
    public Pose lastPosePlaced;
    public GameObject spawnedObject;
    public bool inCreateAnchor;

    static RaycastHit[] s_RaycastResults = new RaycastHit[10];

    volatile public bool tryingToUpdate;
    public bool inUpdateMethod;
    public bool validAtIndicatorStart;
    private bool movingAnchor;
    private bool selectMode = false;

    [SerializeField]
    private GameObject confirmButton;

    // Start is called before the first frame update
    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        aRAnchorManager = FindObjectOfType<ARAnchorManager>();
        indicator = Instantiate(placementIndicator);
        aRAnchors = new List<ARAnchor>();
        isPlacementValid = false;
        tryingToUpdate = false;
        inCreateAnchor = false;
        movingAnchor = false;
        selectMode = false;
    }

    // Update is called once per frame
    void Update()
    {
        inUpdateMethod = false;
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (Input.touchCount == 1)
        {
            //if touching UI element, ignore touch
            if (eventSystem == null || !eventSystem.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                if (isPlacementValid && inCreateAnchor && Input.GetTouch(0).phase != TouchPhase.Ended)
                {
                      PlaceObject();
                }
                if(selectMode)
                {
                    AttemptItemSelect();
                }
                if (Input.GetTouch(0).phase == TouchPhase.Began && !inCreateAnchor)
                {
                }
            }
        }
    }

    public void AttemptItemSelect()
    {
        Debug.Log("Attempting select");
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit raycastHit;
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();

        //make a hitMask so that the raycast only hits objects in the layer items
        int hitMask = 1 << 9;
        if(Physics.Raycast(ray, out raycastHit, Mathf.Infinity, hitMask))
        {
            GameObject objectHit = raycastHit.transform.gameObject;
            GameObject objectContainer = objectHit.transform.parent.gameObject;
            string containerName = objectContainer.transform.name;
            inventoryManager.PickupItem(objectContainer);

        }
    }

    public void ShowConfirmButton()
    {
        confirmButton.SetActive(true);
    }

    public void HideConfirmButton()
    {
        confirmButton.SetActive(false);
    }

    public void CreateAnchor()
    {
        ARAnchor anchorToAdd = aRAnchorManager.AddAnchor(lastPosePlaced);
        spawnedObject.transform.parent = anchorToAdd.transform;
        spawnedObject = null;
        aRAnchors.Add(anchorToAdd);
        HideConfirmButton();
    }

    public void HideAnchorObjects()
    {
        foreach (ARAnchor anchor in aRAnchors)
        {
            Animator[] animators = anchor.gameObject.GetComponentsInChildren<Animator>();
            foreach (Animator a in animators)
            {
                a.enabled = false;
            }
            Renderer[] renderers = anchor.gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                r.enabled = false;
            }
        }
    }


    private void CleaupSpawnedObject()
    {
        if(spawnedObject != null)
        {
            Destroy(spawnedObject);
        }
    }

    public void EnableIndicator(bool enabled)
    {
      inCreateAnchor = enabled;
    }

    public void EnableSelect(bool enabled)
    {
      selectMode = enabled;
    }

    private void PlaceObject()
    {
        if(spawnedObject == null)
        {
          spawnedObject = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
          ShowConfirmButton();
        }
        else
        {
          spawnedObject.transform.position = placementPose.position;
          spawnedObject.transform.rotation = placementPose.rotation;
          movingAnchor = true;
        }

        lastPosePlaced = placementPose;
    }

    private void UpdatePlacementIndicator()
    {
        inUpdateMethod = true;

        if (isPlacementValid && inCreateAnchor)
        {
            tryingToUpdate = true;
            indicator.SetActive(true);
            indicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            tryingToUpdate = false;
            indicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var screenRay = Camera.main.ScreenPointToRay(screenCenter);
        int hitMask = 1 << 10;
        var raycastHits = Physics.RaycastNonAlloc(screenRay, s_RaycastResults, Mathf.Infinity, hitMask);
        isPlacementValid = (s_RaycastResults[0].collider != null);

        if (isPlacementValid)
        {
            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            Quaternion hitRotation = Quaternion.LookRotation(cameraBearing);
            Pose hitPose = new Pose(s_RaycastResults[0].point, hitRotation);
            placementPose = hitPose;
        }
    }
}
