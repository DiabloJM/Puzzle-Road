using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    //Variables publicas
    public static InventoryController instance;
    public int itemInUse;  //Para saber que objeto del inventario se esta utilizando

    [Header("RawImages del Inventario")]
    public RawImage[] images;

    [Header("Canvas de Posibles Lugares")]
    public GameObject optionsCanvas;

    [HideInInspector]
    public string[] inventoryObjects = { "", "", "", "" };

    private void Awake()
    {
        MakeSingleton();
    }

    //Funcion para agregar un objeto al inventario
    public void AddObject(Texture texture)
    {
        string item = texture.name;

        for(int i=0;i <4;i++)
        {
            if(inventoryObjects[i] == "" && item != "")
            {
                inventoryObjects[i] = item;
                images[i].texture = texture;
                images[i].gameObject.SetActive(true);
                item = "";
            }
        }

    }

    //Usar objeto del inventario
    public void UseObject(int num)
    {
        if(inventoryObjects[num - 1] != "")
        {
            optionsCanvas.gameObject.SetActive(true);
            itemInUse = num - 1;
        }
    }

    //Destruye el item de la pantalla después de tocarlo y añadirlo al inventario
    public void DestroyItem(GameObject item)
    {
        Destroy(item);
    }

    //Funcion para hacer el Singleton
    void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
}
