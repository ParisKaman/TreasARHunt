﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class BasicDemoScript : MonoBehaviour
{

    internal enum AppState
    {
        DemoStepInit = 0,
        DemoStepPlaceKey,
        DemoStepPlaceShovel,
        DemoStepPlaceChest,
        DemoStepConfirmPlacement,
        DemoStepSearching,
        DemoStepFoundAllItems,
        DemoStepComplete
    }

    private AppState _currentAppState = AppState.DemoStepInit;

    AppState currentAppState
    {
        get
        {
            return _currentAppState;
        }
        set
        {
            if (_currentAppState != value)
                {
                    Debug.LogFormat("State from {0} to {1}", _currentAppState, value);
                    _currentAppState = value;
                    feedbackBox.text = stateParams[_currentAppState].StepMessage;
                }
        }
    }

    protected struct DemoStepParams
    {
        public string StepMessage {get; set;}
    }

    private readonly Dictionary<AppState, DemoStepParams> stateParams = new Dictionary<AppState, DemoStepParams>{
        { AppState.DemoStepInit, new DemoStepParams(){ StepMessage = "Demo start.  Arrr you ready?" }},
        { AppState.DemoStepPlaceKey, new DemoStepParams(){ StepMessage = "Tap to Place a key" }},
        { AppState.DemoStepPlaceShovel, new DemoStepParams(){ StepMessage = "Tap to Place a shovel" }},
        { AppState.DemoStepPlaceChest, new DemoStepParams(){ StepMessage = "Tap to Place a chest" }},
        { AppState.DemoStepConfirmPlacement, new DemoStepParams(){ StepMessage = "Placement Complete.  Switch user." }},
        { AppState.DemoStepSearching, new DemoStepParams(){ StepMessage = $"Locate the lost items." }},
        { AppState.DemoStepFoundAllItems, new DemoStepParams(){ StepMessage = "Chest unlocked!  Tap to open" }},
        { AppState.DemoStepComplete, new DemoStepParams(){ StepMessage = "Enjoy the loot matey!" }}
    };

    [SerializeField]
    private TMP_Text feedbackBox;
    [SerializeField]
    private GameObject confirmButton;
    [SerializeField]
    private GameObject inventory;

    [SerializeField]
    private GameObject keyPrefab;
    [SerializeField]
    private GameObject shovelPrefab;
    [SerializeField]
    private GameObject chestPrefab;

    private ARTapToPlace arTapToPlace;

    public bool searching = false;
    public int objectsFound = 0;
    public bool foundAllItems = false;
    public bool readyToOpenChest = false;

    // Start is called before the first frame update
    void Start()
    {
        arTapToPlace = FindObjectOfType<ARTapToPlace>();
    }

    // Update is called once per frame
    void Update()
    {
        feedbackBox.text = stateParams[currentAppState].StepMessage;
        if(searching)
        {
            feedbackBox.text += " " + (3 - objectsFound) + " Remain!";
        }
    }

    public void HideObjects()
    {
        arTapToPlace.HideAnchorObjects();
    }

    public void HideButton()
    {
        confirmButton.SetActive(false);
    }

    public void ShowButton()
    {
        confirmButton.SetActive(true);
    }

    public void ShowInventory()
    {
        inventory.SetActive(true);
    }

    public void HideInventory()
    {
        inventory.SetActive(false);
    }

    public void DisablePlaneColliders()
    {
        ARPlaneManager arPlaneManager = FindObjectOfType<ARPlaneManager>();
        arPlaneManager.planePrefab.GetComponent<MeshCollider>().enabled = false;
        foreach (var plane in arPlaneManager.trackables)
        {
            plane.gameObject.GetComponent<MeshCollider>().enabled = false;
        }
    }


    public void AdvanceDemo()
    {
        switch(currentAppState)
        {
            case AppState.DemoStepInit:
                HideButton();
                arTapToPlace.EnableIndicator(true);
                arTapToPlace.objectToPlace = keyPrefab;
                currentAppState = AppState.DemoStepPlaceKey;
                break;
            case AppState.DemoStepPlaceKey:
                arTapToPlace.objectToPlace = shovelPrefab;
                currentAppState = AppState.DemoStepPlaceShovel;
                break;
            case AppState.DemoStepPlaceShovel:
                arTapToPlace.objectToPlace = chestPrefab;
                currentAppState = AppState.DemoStepPlaceChest;
                break;
            case AppState.DemoStepPlaceChest:
                arTapToPlace.EnableIndicator(false);
                HideObjects();
                ShowButton();
                ChestManager chestManager = FindObjectOfType<ChestManager>();
                chestManager.placing = false;
                currentAppState = AppState.DemoStepConfirmPlacement;
                break;
            case AppState.DemoStepConfirmPlacement:
                HideButton();
                ShowInventory();
                searching = true;
                arTapToPlace.EnableSelect(true);
                InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
                inventoryManager.SearchBegins();
                currentAppState = AppState.DemoStepSearching;
                break;
            case AppState.DemoStepSearching:
                ShowButton();
                HideInventory();
                searching = false;
                arTapToPlace.EnableSelect(false);
                currentAppState = AppState.DemoStepFoundAllItems;
                break;
            case AppState.DemoStepFoundAllItems:
                HideButton();
                readyToOpenChest = true;
                DisablePlaneColliders();
                currentAppState = AppState.DemoStepComplete;
                break;
            case AppState.DemoStepComplete:
                break;
            default:
                Debug.Log("Error, got into a bad state " + currentAppState.ToString());
                break;
        }
    }

}
