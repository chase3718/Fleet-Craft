using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;

public class DockUIManager : MonoBehaviour
{
    Ship ship;
    BuildManager buildManager;
    UIDocument doc;
    Button deleteBtn;
    Button hullBtn;
    Button weaponBtn;
    Button engineBtn;
    Button saveBtn;
    Button exitBtn;
    Button selectBtn;
    ListView partList;
    Dictionary<string, List<ShipPart>> partLists = new Dictionary<string, List<ShipPart>>();

    void Awake()
    {
        ship = FindObjectOfType<Ship>();
        buildManager = FindObjectOfType<BuildManager>();
        doc = GetComponent<UIDocument>();
        InitPartLists();
        partList = doc.rootVisualElement.Q<ListView>("PartList");
        partList.style.display = DisplayStyle.None;
        deleteBtn = doc.rootVisualElement.Q<Button>("RemoveBtn");
        hullBtn = doc.rootVisualElement.Q<Button>("HullBtn");
        weaponBtn = doc.rootVisualElement.Q<Button>("WeaponBtn");
        engineBtn = doc.rootVisualElement.Q<Button>("EngineBtn");
        saveBtn = doc.rootVisualElement.Q<Button>("SaveBtn");
        exitBtn = doc.rootVisualElement.Q<Button>("ExitBtn");
        selectBtn = doc.rootVisualElement.Q<Button>("SelectBtn");
        deleteBtn.clicked += () =>
        {
            buildManager.dockMode = DockMode.Delete;
            TogglePartList(null); //Hide the part list because you're no longer in building mode
        };
        hullBtn.clicked += () => TogglePartList("Hull");
        weaponBtn.clicked += () => TogglePartList("Weapon");
        engineBtn.clicked += () => TogglePartList("Engine");
        selectBtn.clicked += () => {
            buildManager.dockMode = DockMode.Select;
            TogglePartList(null); //Hide the part list because you're no longer in building mode
        };
        saveBtn.clicked += () => {
            ship.Save();
            SceneManager.LoadScene( "BoatTest" );
            };
        exitBtn.clicked += () => {
            ship.Save();
            Application.Quit();            
        };

        deleteBtn.style.width = deleteBtn.style.height;
        hullBtn.style.width = hullBtn.style.height;
        weaponBtn.style.width = weaponBtn.style.height;
        saveBtn.style.width = saveBtn.style.height;
        exitBtn.style.width = exitBtn.style.height;
        selectBtn.style.width = selectBtn.style.height;

    }

    void InitPartLists()
    {
        partLists.Add("Hull", new List<ShipPart>());
        partLists.Add("Engine", new List<ShipPart>());
        partLists.Add("Weapon", new List<ShipPart>());
        partLists.Add("Aircraft", new List<ShipPart>());
        partLists.Add("Decoration", new List<ShipPart>());

        GameObject[] allPrimitives = Resources.LoadAll<GameObject>("Prefabs/ShipParts/");
        foreach (GameObject prim in allPrimitives)
        {
            ShipPart part = prim.GetComponent<ShipPart>();

            partLists[part.category.ToString()].Add(part);
        }

    }

    private Boolean listOpen = false;
    void TogglePartList(string category = null)
    {
        if(category == null){
            partList.style.display = DisplayStyle.None;
            partList.Clear();
            listOpen = false;
        }
        else if(listOpen){ //closes if open
            TogglePartList(null);
        } else
        {
            
            buildManager.dockMode = DockMode.Build;   
            partList.Clear();
            partList.itemsSource = partLists[category];
            partList.makeItem = () => new Button();
            partList.bindItem = (e, i) =>
            {
                (e as Button).text = (partList.itemsSource[i] as ShipPart).alias;
                (e as Button).clicked += () =>
                {
                    buildManager.SetGhostBlock(partList.itemsSource[i] as ShipPart);
                    buildManager.dockMode = DockMode.Build;
                };
            };
            partList.itemsChosen += items => Debug.Log(items);
            partList.selectionChanged += items => Debug.Log(items);
            partList.style.display = DisplayStyle.Flex;
            listOpen = true;
            return;
        }

    }
}
