using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Logic : MonoBehaviour
{
    //Variables publicas
    [Header("Jugador y Nivel")]
    public GameObject player;
    public int level; //Al nivel actual se le resta 1 para que este correcto en los arreglos de PlayerPrefs

    [Header("Canvas de Victoria y Derrota")]
    public GameObject victoryCanvas;
    public GameObject defeatCanvas;

    [Header("GameObjects a Cambiar")]
    public GameObject rope;
    public GameObject tree;
    public GameObject fire;
    public SpriteRenderer advertisement;

    [Header("Sprite de Anuncio")]
    public Sprite blankAdvertisement;

    //Variables privadas
    private bool lighterUsed = false, chainsawUsed = false, ropeUsed = false; //Para saber si los objetos fueron usados
    private bool wellUsedL = false, wellUsedC = false, wellUsedR = false;  //Para saber si los objetos fueron usados correctamente
    private bool isTreeFallen = false; //Saber si el arbol fue cortado
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
        if (wellUsedR && wellUsedC && wellUsedL) //Si la cuerda fue usada correctamente cuando el arbol esta caido
        {
            StartCoroutine(ClimbTree());
        }
        else if (chainsawUsed && lighterUsed && ropeUsed) //Si todos los objetos fueron mal usados
        {
            gameObject.GetComponent<AudioSource>().Stop();
            defeatCanvas.gameObject.SetActive(true);
            defeatCanvas.GetComponent<AudioSource>().Play();
        }
        else if(chainsawUsed && lighterUsed && !wellUsedC && !wellUsedL) //Si usan mal la sierra y el encendedor y no tienen acceso a la cuerda
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
    public void AdvertisementOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "encendedor")
        {
            lighterUsed = true;
            wellUsedL = true;
            rope.gameObject.SetActive(true);
            SoundController.instance.sfxVolumes[2].Play();
            StartCoroutine(BurnAdvertisement());
        }
        else
        {
            chainsawUsed = true;
        }

        //Quitar objeto del inventario
        InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] = "";
        InventoryController.instance.images[InventoryController.instance.itemInUse].gameObject.SetActive(false);

        Analisis();
    }

    //Funcion cuando se use objeto en el arbol
    public void TreeOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Sierra")
        {
            chainsawUsed = true;
            wellUsedC = true;
            isTreeFallen = true;
            SoundController.instance.sfxVolumes[3].Play();
            StartCoroutine(CutDownTree());
        }
        else if(InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "encendedor")
        {
            lighterUsed = true;
        }
        else
        {
            ropeUsed = true;
        }

        //Quitar objeto del inventario
        InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] = "";
        InventoryController.instance.images[InventoryController.instance.itemInUse].gameObject.SetActive(false);

        Analisis();
    }

    //Funcion para cuando se use un objeto en la calle
    public void StreetOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "gancho" && isTreeFallen)
        {
            ropeUsed = true;
            wellUsedR = true;
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "encendedor")
        {
            lighterUsed = true;
        }
        else
        {
            chainsawUsed = true;
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

    //Corutina para talar el arbol
    IEnumerator CutDownTree()
    {
        yield return new WaitForSeconds(1.0f);
        tree.gameObject.transform.rotation = Quaternion.Euler(0, 0, 18);
        tree.gameObject.transform.position = new Vector3(1.404f, -1.58f, 0.0f);
        yield return new WaitForSeconds(0.3f);
        SoundController.instance.sfxVolumes[4].Play();
        tree.gameObject.transform.rotation = Quaternion.Euler(0, 0, 36);
        tree.gameObject.transform.position = new Vector3(0.928f, -1.66f, 0.0f);
        yield return new WaitForSeconds(1.0f);
        tree.gameObject.transform.rotation = Quaternion.Euler(0, 0, 54);
        tree.gameObject.transform.position = new Vector3(0.452f, -1.74f, 0.0f);
        yield return new WaitForSeconds(0.3f);
        tree.gameObject.transform.rotation = Quaternion.Euler(0, 0, 72);
        tree.gameObject.transform.position = new Vector3(-0.024f, -1.82f, 0.0f);
        yield return new WaitForSeconds(0.3f);
        tree.gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
        tree.gameObject.transform.position = new Vector3(-0.5f, -1.9f, 0.0f);
    }

    //Corutina para prenderle fuego al anuncio
    IEnumerator BurnAdvertisement()
    {
        fire.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        fire.gameObject.SetActive(false);
        advertisement.gameObject.GetComponent<SpriteRenderer>().sprite = blankAdvertisement;
        yield return new WaitForSeconds(3.0f);
        rope.gameObject.SetActive(true);
    }
    //Corutina para lanzar al jugador por los aires y ganar
    IEnumerator ClimbTree()
    {
        player.gameObject.transform.position = new Vector3(-0.94f, -0.55f, 0.0f);
        yield return new WaitForSeconds(0.5f);
        player.gameObject.transform.position = new Vector3(-0.39f, -0.78f, 0.0f);
        yield return new WaitForSeconds(0.5f);
        player.gameObject.transform.position = new Vector3(0.18f, -0.78f, 0.0f);
        yield return new WaitForSeconds(0.5f);
        player.gameObject.transform.position = new Vector3(0.7f, -0.78f, 0.0f);
        yield return new WaitForSeconds(0.5f);
        player.gameObject.transform.position = new Vector3(1.26f, -0.78f, 0.0f);
        yield return new WaitForSeconds(0.5f);
        player.gameObject.transform.position = new Vector3(1.56f, -1.3f, 0.0f);
        yield return new WaitForSeconds(0.5f);
        player.gameObject.transform.position = new Vector3(1.69f, -2.22f, 0.0f);
        yield return new WaitForSeconds(1.5f);
        gameObject.GetComponent<AudioSource>().Stop();
        victoryCanvas.gameObject.SetActive(true);
        victoryCanvas.GetComponent<AudioSource>().Play();
        ChangePlayerPrefs(3);
    }
}
