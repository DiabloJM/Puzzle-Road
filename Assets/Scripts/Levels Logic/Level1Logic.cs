using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Logic : MonoBehaviour
{
    //Variables publicas
    [Header("Jugador y Nivel")]
    public GameObject player;
    public int level; //Al nivel actual se le resta 1 para que este correcto en los arreglos de PlayerPrefs

    [Header("Canvas de Victoria y Derrota")]
    public GameObject victoryCanvas;
    public GameObject defeatCanvas;

    [Header("GameObjects a Cambiar")]
    public GameObject horizontalBar;
    public GameObject ballon;
    public SpriteRenderer semaphore;

    [Header("Sprite de Semaforo")]
    public Sprite redSemaphore;

    //Variables privadas
    private bool scissorsUsed = false, barUsed = false; //Para saber si los objetos fueron usados
    private bool wellUsedS = false, wellUsedB = false;  //Para saber si los objetos fueron usados correctamente
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
        if(wellUsedB && wellUsedS) //Si ambos objetos fueron bien utilizados
        {
            StartCoroutine(LaunchPlayer());
        }
        else if(scissorsUsed && barUsed) //Si ambos objetos fueron mal usados
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

    //Funcion cuando se use objeto en el semaforo
    public void SemaphoreOption()
    {
        if(InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Tijeras")
        {
            scissorsUsed = true;
            wellUsedS = true;
            semaphore.sprite = redSemaphore; //Cambiar semaforo de verde a rojo
        }
        else
        {
            barUsed = true;
        }

        //Quitar objeto del inventario
        InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] = "";
        InventoryController.instance.images[InventoryController.instance.itemInUse].gameObject.SetActive(false);

        Analisis();
    }

    //Funcion cuando se use objeto en el globo
    public void BallonOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Tijeras")
        {
            scissorsUsed = true;
        }
        else
        {
            barUsed = true;
            wellUsedB = true;
            horizontalBar.gameObject.SetActive(true); //Poner la barra sobre el globo
        }

        //Quitar objeto del inventario
        InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] = "";
        InventoryController.instance.images[InventoryController.instance.itemInUse].gameObject.SetActive(false);

        Analisis();
    }

    //Funcion para actualizar variables del PlayerPrefs después de ganar
    private void ChangePlayerPrefs(int stars)
    {
        if(playableLevels < 2)
        {
            PlayerPrefs.SetInt("playableLevels", 2);
        }
        else if (PlayerPrefs.GetInt("playableLevels") < (level + 2))
        {
            PlayerPrefs.SetInt("playableLevels", playableLevels + 1);
        }

        if(levelStars < stars)
        {
            PlayerPrefs.SetInt("levelStars" + level, stars);
        }
    }

    //Corutina para lanzar al jugador por los aires y ganar
    IEnumerator LaunchPlayer()
    {
        player.gameObject.transform.position = new Vector3(-1.59f, -1.47f, 0.0f);
        yield return new WaitForSeconds(0.5f);
        SoundController.instance.sfxVolumes[2].Play();
        yield return new WaitForSeconds(0.4f);
        ballon.gameObject.SetActive(false);
        player.gameObject.transform.position = new Vector3(-0.52f, -0.81f, 0.0f);
        horizontalBar.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -45.0f);
        yield return new WaitForSeconds(0.5f);
        horizontalBar.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
        player.gameObject.transform.position = new Vector3(0.74f, -0.32f, 0.0f);
        yield return new WaitForSeconds(0.5f);
        horizontalBar.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -180.0f);
        player.gameObject.transform.position = new Vector3(1.63f, -2.25f, 0.0f);
        yield return new WaitForSeconds(1.5f);
        gameObject.GetComponent<AudioSource>().Stop();
        victoryCanvas.gameObject.SetActive(true);
        victoryCanvas.GetComponent<AudioSource>().Play();
        ChangePlayerPrefs(3);
    }
}
