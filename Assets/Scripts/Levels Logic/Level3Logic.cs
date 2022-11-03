using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Logic : MonoBehaviour
{
    //Variables publicas
    [Header("Jugador y Nivel")]
    public GameObject player;
    public int level; //Al nivel actual se le resta 1 para que este correcto en los arreglos de PlayerPrefs

    [Header("Canvas de Victoria y Derrota")]
    public GameObject victoryCanvas;
    public GameObject defeatCanvas;

    [Header("GameObject a cambiar")]
    public GameObject playerUmbrella;

    //Variables privadas
    private bool umbrellaUsed = false, stopUsed = false, ropeUsed = false; //Para saber si los objetos fueron usados
    private bool wellUsedU = false, wellUsedR = false;  //Para saber si los objetos fueron usados correctamente
    private int playableLevels; //Total de niveles que el jugador ya puede jugar
    private int levelStars; //Estrellas conseguidas en este nivel

    private void Awake()
    {
        playableLevels = PlayerPrefs.GetInt("playableLevels");
        levelStars = PlayerPrefs.GetInt("levelStars" + level);
    }

    //Funcion para ver si se cumplen las condiciones de victoria o derrota
    private void Analisis()
    {
        if (wellUsedR && wellUsedU) //Si la cuerda y el paraguas fueron usados correctamente
        {
            StartCoroutine(UseRope());
        }
        else if (umbrellaUsed && stopUsed && ropeUsed) //Si todos los objetos fueron usados 
        {
            gameObject.GetComponent<AudioSource>().Stop();
            defeatCanvas.gameObject.SetActive(true);
            defeatCanvas.GetComponent<AudioSource>().Play();
        }
        else
        {
            //Todo continua
        }
    }

    //Funcion cuando un objeto es utilizo en el anuncio publicitario
    public void ScaffoldOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Paraguas")
        {
            umbrellaUsed = true;
            wellUsedU = true;
            StartCoroutine(CrossStreet());
        }
        else if(InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Cuerda")
        {
            ropeUsed = true;
        }
        else
        {
            stopUsed = true;
        }

        //Quitar objeto del inventario
        InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] = "";
        InventoryController.instance.images[InventoryController.instance.itemInUse].gameObject.SetActive(false);

        Analisis();
    }

    //Funcion cuando se use objeto en el arbol
    public void HookOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Cuerda" && wellUsedU)
        {
            ropeUsed = true;
            wellUsedR = true;
        }
        else if(InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Cuerda" && !wellUsedU)
        {
            ropeUsed = true;
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Alto")
        {
            stopUsed = true;
        }
        else
        {
            umbrellaUsed = true;
        }

        //Quitar objeto del inventario
        InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] = "";
        InventoryController.instance.images[InventoryController.instance.itemInUse].gameObject.SetActive(false);

        Analisis();
    }

    //Funcion cuando se use un objeto en la calle
    public void StreetOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Paraguas")
        {
            umbrellaUsed = true;
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Cuerda")
        {
            ropeUsed = true;
        }
        else
        {
            stopUsed = true;
        }

        //Quitar objeto del inventario
        InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] = "";
        InventoryController.instance.images[InventoryController.instance.itemInUse].gameObject.SetActive(false);

        Analisis();
    }

    //Funcion para actualizar variables del PlayerPrefs después de ganar
    private void ChangePlayerPrefs(int stars)
    {
        if (playableLevels < 2)
        {
            PlayerPrefs.SetInt("playableLevels", 2);
        }
        else if (PlayerPrefs.GetInt("playableLevels") < (level + 2))
        {
            PlayerPrefs.SetInt("playableLevels", playableLevels + 1);
        }

        if (levelStars < stars)
        {
            PlayerPrefs.SetInt("levelStars" + level, stars);
        }
    }
    
    //Corutina para cruzar la calle después de usar el paraguas
    IEnumerator CrossStreet()
    {
        playerUmbrella.gameObject.SetActive(true);
        player.transform.position = new Vector3(-1.27f, -0.93f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.83f, -0.93f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.45f, -0.93f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.24f, -0.93f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.05f, -0.93f, 0f);
        yield return new WaitForSeconds(1.0f);
        playerUmbrella.gameObject.SetActive(false);
    }

    //Corutina para balancearse con la cuerda
    IEnumerator UseRope()
    {
        player.transform.position = new Vector3(0.05f, -0.15f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.49f, -0.53f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.99f, -0.88f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.45f, -0.36f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.74f, -0.97f, 0f);
        yield return new WaitForSeconds(1.5f);
        gameObject.GetComponent<AudioSource>().Stop();
        victoryCanvas.gameObject.SetActive(true);
        victoryCanvas.GetComponent<AudioSource>().Play();
        ChangePlayerPrefs(3);
    }
}
