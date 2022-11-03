using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level4Logic : MonoBehaviour
{
    //Variables publicas
    [Header("Jugador y Nivel")]
    public GameObject player;
    public int level;

    [Header("Canvas de Victoria y Derrota")]
    public GameObject victoryCanvas;
    public GameObject defeatCanvas;

    [Header("GameObjects a Cambiar")]
    public GameObject playerStickRun;
    public GameObject playerStickBalance;
    public GameObject puddle;
    public GameObject arrowRope;

    [Header("Tercer estrella y Sprite de Estrella")]
    public RawImage thirdStar;
    public Texture goldenStar;

    //Variables privadas
    private bool stickUsed = false, waterUsed = false, ropeUsed = false; //Para saber si los objetos fueron usados
    private bool waterInFloor = false, waterInCrossbow = false;  //Para saber en cual de las dos opciones validas fue usada la botella de agua
    private bool stickOnStreet = false, stickOnRope = false; //Para saber en cual de las dos opciones validas fue usada el palo
    private bool wellUsedR = false; //Para saber si la cuerda fue usada en el arbol
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
        if (waterInFloor && stickOnStreet && wellUsedR) //Las condiciones para la victoria de 2 estrellas
        {
            gameObject.GetComponent<AudioSource>().Stop();
            victoryCanvas.gameObject.SetActive(true);
            victoryCanvas.GetComponent<AudioSource>().Play();
            ChangePlayerPrefs(2);
        }
        else if (waterInCrossbow && stickOnRope) //Las condiciones para la victoria de 3 estrellas
        {
            thirdStar.texture = goldenStar;
            gameObject.GetComponent<AudioSource>().Stop();
            victoryCanvas.gameObject.SetActive(true);
            victoryCanvas.GetComponent<AudioSource>().Play();
            ChangePlayerPrefs(3);
        }
        else if(stickUsed && waterUsed && ropeUsed) //Si todos los objetos fueron usados
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

    //Funcion cuando un objeto es utilizo en la ballesta
    public void CrossbowOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Agua")
        {
            waterUsed = true;
            waterInCrossbow = true;
            arrowRope.gameObject.SetActive(true);
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Cuerda")
        {
            ropeUsed = true;
        }
        else
        {
            stickUsed = true;
        }

        //Quitar objeto del inventario
        InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] = "";
        InventoryController.instance.images[InventoryController.instance.itemInUse].gameObject.SetActive(false);

        Analisis();
    }

    //Funcion cuando se use objeto en la opcion que esta cerca del jugador
    public void NearPlayerOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Agua")
        {
            waterUsed = true;
            waterInFloor = true;
            puddle.gameObject.SetActive(true);
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Cuerda")
        {
            ropeUsed = true;
        }
        else
        {
            stickUsed = true;
        }

        //Quitar objeto del inventario
        InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] = "";
        InventoryController.instance.images[InventoryController.instance.itemInUse].gameObject.SetActive(false);

        Analisis();
    }

    //Funcion cuando se use un objeto en la calle
    public void StreetOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Palo" && waterInFloor)
        {
            stickUsed = true;
            stickOnStreet = true;
            playerStickRun.gameObject.SetActive(true);
            //Llamar a la corutina para saltar con el palo
            StartCoroutine(JumpWithStick());
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Palo")
        {
            stickUsed = true;
            Analisis();
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Agua")
        {
            waterUsed = true;
            Analisis();
        }
        else
        {
            ropeUsed = true;
            Analisis();
        }

        //Quitar objeto del inventario
        InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] = "";
        InventoryController.instance.images[InventoryController.instance.itemInUse].gameObject.SetActive(false);
    }

    //Funcion para cuando se use un objeto en el arbol
    public void TreeOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Cuerda" && waterInFloor && stickOnStreet)
        {
            ropeUsed = true;
            wellUsedR = true;
            //Llamar a la corutina para balancearse en la cuerda en el arbol
            StartCoroutine(RopeOnTree());
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Cuerda")
        {
            ropeUsed = true;
            Analisis();
        }
        else if(InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Palo" && waterInCrossbow)
        {
            stickUsed = true;
            stickOnRope = true;
            playerStickBalance.gameObject.SetActive(true);
            //Llamar a la corutina para cruzar la cuerda con el palo
            StartCoroutine(BalanceInRope());
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Palo")
        {
            stickUsed = true;
            Analisis();
        }
        else
        {
            waterUsed = true;
            Analisis();
        }

        //Quitar objeto del inventario
        InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] = "";
        InventoryController.instance.images[InventoryController.instance.itemInUse].gameObject.SetActive(false);
    }

    //Funcion para cuando un objeto es usado en el semaforo
    //En este nivel ningun objeto hace nada en el semaforo, por lo que es una opcion inutil
    public void SemaphoreOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Cuerda")
        {
            ropeUsed = true;
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Palo")
        {
            stickUsed = true;
        }
        else
        {
            waterUsed = true;
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

    //Corutina para saltar a mitad de la calle con ayuda del agua en el piso y el palo
    IEnumerator JumpWithStick()
    {
        player.transform.position = new Vector3(-1.27f, -2.27f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.98f, -1.95f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.48f, -1.47f, 0f);
        playerStickRun.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.01f, -1.6f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.23f, -2.27f, 0f);
        yield return new WaitForSeconds(1.0f);
        Analisis();
    }

    //Corutina para balancearse en la cuerda con ayuda del palo
    IEnumerator BalanceInRope()
    {
        player.transform.position = new Vector3(-0.43f, 0.36f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.06f, 0.23f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.28f, 0.12f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.7f, -0.01f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.2f, -0.22f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.65f, -0.4f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.79f, -1.42f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.79f, -1.95f, 0f);
        playerStickBalance.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.79f, -2.27f, 0f);
        yield return new WaitForSeconds(1.0f);
        Analisis();
    }

    //Corutina para usar la cuerda en el arbol
    IEnumerator RopeOnTree()
    {
        player.transform.position = new Vector3(0.23f, -1.98f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.23f, -1.58f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.57f, -1.74f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.97f, -2f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.5f, -1.63f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.79f, -1.74f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.79f, -2f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.79f, -2.27f, 0f);
        yield return new WaitForSeconds(1.0f);
        Analisis();
    }
}
