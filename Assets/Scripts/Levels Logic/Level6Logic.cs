using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level6Logic : MonoBehaviour
{
    //Variables publicas
    [Header("Jugador y Nivel")]
    public GameObject player;
    public int level;

    [Header("Canvas de Victoria y Derrota")]
    public GameObject victoryCanvas;
    public GameObject defeatCanvas;

    [Header("GameObjects a Cambiar")]
    public GameObject playerRocket; //El lanza cohetes del jugador
    public GameObject playerUmbrella;   //El paraguas del jugador
    public GameObject playerStick;  //El palo del jugador
    public GameObject ladder;   //Escalera en el andamio
    public GameObject ScaffoldSound; //Sonido de la grua hibraulica
    public GameObject explotionLayette; //Explosion en la canastilla del andamio
    public GameObject explotionStart;  //Explosion en la zona del inicio
    public GameObject explotionStreet;  //Explosion en la calle
    public SpriteRenderer staffoldBase; //Base del andamio
    public SpriteRenderer staffoldLayette;  //Canastilla del andamio

    [Header("Sprites")]
    public Sprite staffoldMedium;   //Andamio a la mitad
    public Sprite staffoldMaximum;  //Andamio al maximo
    public Sprite layetteMedium;    //Canastilla a la mitad
    public Sprite layetteMaximum;   //Canastilla al maximo

    [Header("Segunda/Tercer estrella y Sprite de Estrella")]
    public RawImage secondStar;
    public RawImage thirdStar;
    public Texture goldenStar;

    //Variables privadas
    private bool ladderUsed = false, umbrellaUsed = false, rocketUsed = false, stickUsed = false, ropeUsed = false; //Para saber si los objetos fueron usados
    private bool wellUsedL = false, wellUsedR = false, wellUsedS = false; //Para saber si los objetos fueron bien usados
    private bool rocketOnLayette = false, rocketOnStreet = false, rocketOnStart = false; //Para saber en cual de las zonas correctas fue usada el lanza cohetes
    private bool umbrellaOnLayette = false, umbrellaOnSemaphore = false; //Para saber en cual de las zonas correctas fue usada el paraguas
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
        if (wellUsedL && rocketOnLayette && wellUsedS && umbrellaOnSemaphore) //Las condiciones para la victoria de 1 estrella
        {
            gameObject.GetComponent<AudioSource>().Stop();
            victoryCanvas.gameObject.SetActive(true);
            victoryCanvas.GetComponent<AudioSource>().Play();
            ChangePlayerPrefs(1);
        }
        else if (wellUsedL && umbrellaOnLayette && (rocketOnStreet || wellUsedR)) //Las condiciones para la victoria de 2 estrellas
        {
            secondStar.texture = goldenStar;
            gameObject.GetComponent<AudioSource>().Stop();
            victoryCanvas.gameObject.SetActive(true);
            victoryCanvas.GetComponent<AudioSource>().Play();
            ChangePlayerPrefs(2);
        }
        else if (rocketOnStart && wellUsedR) //Las condiciones para la victoria de 3 estrellas
        {
            secondStar.texture = goldenStar;
            thirdStar.texture = goldenStar;
            gameObject.GetComponent<AudioSource>().Stop();
            victoryCanvas.gameObject.SetActive(true);
            victoryCanvas.GetComponent<AudioSource>().Play();
            ChangePlayerPrefs(3);
        }
        else if (ladderUsed && umbrellaUsed && rocketUsed && stickUsed && ropeUsed) //Si todos los objetos fueron usados
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

    //Funcion cuando un objeto es utilizo en el andamio base
    public void StaffoldOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Escalera")
        {
            ladderUsed = true;
            wellUsedL = true;
            ladder.gameObject.SetActive(true);
            SoundController.instance.sfxVolumes[2].Play();
            //Llamar a la funcion para subir al andamio
            StartCoroutine(ScaffoldUp());
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Rocket_Launcher")
        {
            rocketUsed = true;
            rocketOnStart = true;
            playerRocket.gameObject.SetActive(true);
            explotionStart.gameObject.SetActive(true);
            //Llamar a la funcion para salir volando a la mitad de la calle
            StartCoroutine(LaunchToStreet());
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Paraguas")
        {
            umbrellaUsed = true;
            Analisis();
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Palo")
        {
            stickUsed = true;
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

    //Funcion cuando se use objeto en la canastilla del andamio
    public void LayetteOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Rocket_Launcher")
        {
            rocketUsed = true;

            if (wellUsedL)
            {
                rocketOnLayette = true;
                playerRocket.gameObject.SetActive(true);
                explotionLayette.gameObject.SetActive(true);
                //Llamar a la corutina para salir volando hacia las luces
                StartCoroutine(LaunchToLights());
                
            }
            else
            {
                Analisis();
            }
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Escalera")
        {
            ladderUsed = true;
            Analisis();
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Paraguas")
        {
            umbrellaUsed = true;

            if(wellUsedL)
            {
                umbrellaOnLayette = true;
                playerUmbrella.gameObject.SetActive(true);
                //Llamar a la corutina para bajar de la canastilla con el paraguas
                StartCoroutine(FallToStreet());
            }
            Analisis();
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Palo")
        {
            stickUsed = true;
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

    //Funcion cuando se use un objeto en la calle
    public void StreetOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Rocket_Launcher")
        {
            rocketUsed = true;

            if (wellUsedL && umbrellaOnLayette)
            {
                rocketOnStreet = true;
                playerRocket.gameObject.SetActive(true);
                explotionStreet.gameObject.SetActive(true);
                //Llamar a la corutina para salir volando al final de la calle
                StartCoroutine(LaunchToEnd());
            }
            else
            {
                Analisis();
            }
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Escalera")
        {
            ladderUsed = true;
            Analisis();
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Paraguas")
        {
            umbrellaUsed = true;
            Analisis();
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Palo")
        {
            stickUsed = true;
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

    //Funcion para cuando se use un objeto en el semaforo peatonal
    public void RopeOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Palo")
        {
            stickUsed = true;

            if (wellUsedL && rocketOnLayette)
            {
                wellUsedS = true;
                playerStick.gameObject.SetActive(true);
                //Llamar a la corutina para balancearse hasta el otro lado
                StartCoroutine(BalanceRope());
            }
            else
            {
                Analisis();
            }
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Escalera")
        {
            ladderUsed = true;
            Analisis();
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Paraguas")
        {
            umbrellaUsed = true;
            Analisis();
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Rocket_Launcher")
        {
            rocketUsed = true;
            Analisis();
        }
        else
        {
            ropeUsed = true;

            if ((wellUsedL && umbrellaOnLayette) || rocketOnStart)
            {
                wellUsedR = true;
                //Llamar a la corutina para balancearse en las luces
                StartCoroutine(ThrowRope());
            }
            else
            {
                Analisis();
            }
        }

        //Quitar objeto del inventario
        InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] = "";
        InventoryController.instance.images[InventoryController.instance.itemInUse].gameObject.SetActive(false);
    }

    //Funcion para cuando se usa un objeto en el semaforo
    public void SemaphoreOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Paraguas")
        {
            umbrellaUsed = true;

            if (wellUsedL && rocketOnLayette && wellUsedS)
            {
                umbrellaOnSemaphore = true;
                playerUmbrella.gameObject.SetActive(true);
                //Llamar a la corutina para bajar del semaforo con el paraguas
                StartCoroutine(FallToEnd());

            }
            else
            {
                Analisis();
            }
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Escalera")
        {
            ladderUsed = true;
            Analisis();
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Rocket_Launcher")
        {
            rocketUsed = true;
            Analisis();
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Palo")
        {
            stickUsed = true;
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

    //Funcion para actualizar variables del PlayerPrefs después de ganar
    private void ChangePlayerPrefs(int stars)
    {
        if (levelStars < stars)
        {
            PlayerPrefs.SetInt("levelStars" + level, stars);
        }
    }

    //Corutina para subir el andamio junto con el jugador
    IEnumerator ScaffoldUp()
    {
        player.transform.position = new Vector3(-2.39f, -0.84f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-2.26f, -0.33f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-2.15f, 0.07f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-2.01f, 0.58f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-1.77f, 0.96f, 0f);
        ladder.gameObject.SetActive(false);
        staffoldLayette.sortingOrder = 4;
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-1.47f, 0.83f, 0f);
        yield return new WaitForSeconds(0.5f);
        ScaffoldSound.gameObject.SetActive(true);
        player.transform.position = new Vector3(-1.37f, 0.38f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-1.37f, 1.25f, 0f);
        staffoldBase.sprite = staffoldMedium;
        staffoldLayette.sprite = layetteMedium;
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-1.37f, 2.19f, 0f);
        staffoldBase.sprite = staffoldMaximum;
        staffoldLayette.sprite = layetteMaximum;
        yield return new WaitForSeconds(1.0f);
        Analisis();
    }

    //Corutina para ser lanzado a la mitad de la calle
    IEnumerator LaunchToStreet()
    {
        player.transform.position = new Vector3(-1.31f, -1.57f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.81f, -0.58f, 0f);
        yield return new WaitForSeconds(0.5f);
        explotionStart.gameObject.SetActive(false);
        player.transform.position = new Vector3(-0.23f, -0.81f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.05f, -1.28f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.36f, -2.27f, 0f);
        playerRocket.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        Analisis();
    }

    //Corutina para ser lanzado a las luces
    IEnumerator LaunchToLights()
    {
        player.transform.position = new Vector3(-1.02f, 2.58f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.63f, 2.81f, 0f);
        yield return new WaitForSeconds(0.5f);
        explotionLayette.gameObject.SetActive(false);
        player.transform.position = new Vector3(-0.32f, 2.89f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.03f, 2.67f, 0f);
        yield return new WaitForSeconds(0.5f);
        playerRocket.gameObject.SetActive(false);
        player.transform.position = new Vector3(0.4f, 2.47f, 0f);
        yield return new WaitForSeconds(1.0f);
        Analisis();
    }

    //Corutina para ser lanzado al final de la calle
    IEnumerator LaunchToEnd()
    {
        player.transform.position = new Vector3(0.67f, -1.5f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.07f, -0.89f, 0f);
        yield return new WaitForSeconds(0.5f);
        explotionStreet.gameObject.SetActive(false);
        player.transform.position = new Vector3(1.63f, -1.11f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(2.0f, -2.27f, 0f);
        playerRocket.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        Analisis();
    }

    //Corutina para bajar en paracaidas a la mitad de la calle
    IEnumerator FallToStreet()
    {
        player.transform.position = new Vector3(-0.8f, 2.17f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.53f, 1.78f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.18f, 1.23f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.09f, 0.92f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.17f, 0.14f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.22f, -1.13f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.22f, -2.27f, 0f);
        playerUmbrella.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        Analisis();
    }

    //Corutina para bajar en paracaidas al final de la calle
    IEnumerator FallToEnd()
    {
        player.transform.position = new Vector3(2.0f, 1.73f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(2.0f, 0.89f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(2.0f, 0.25f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(2.0f, -0.91f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(2.0f, -1.7f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(2.0f, -2.27f, 0f);
        playerUmbrella.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        Analisis();
    }

    //Corutina para balancearse en la cuerda hasta el semaforo
    IEnumerator BalanceRope()
    {
        player.transform.position = new Vector3(0.78f, 2.47f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.06f, 2.44f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.29f, 2.44f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.51f, 2.44f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.73f, 2.44f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(2.0f, 2.49f, 0f);
        playerStick.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        Analisis();
    }

    //Corutina para tirar cuerda en las luces y balancearse hasta el final
    IEnumerator ThrowRope()
    {
        player.transform.position = new Vector3(-0.03f, -1.79f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.29f, -1.35f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.68f, -0.76f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.14f, -0.91f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.25f, -0.7f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.71f, -0.19f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.1f, 0.14f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.49f, -0.09f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.83f, -0.55f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(2.0f, -1.06f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(2.0f, -2.27f, 0f);
        yield return new WaitForSeconds(1.0f);
        Analisis();
    }
}
