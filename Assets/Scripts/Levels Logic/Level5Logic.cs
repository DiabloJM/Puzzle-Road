using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level5Logic : MonoBehaviour
{
    //Variables publicas
    [Header("Jugador y Nivel")]
    public GameObject player;
    public int level;

    [Header("Canvas de Victoria y Derrota")]
    public GameObject victoryCanvas;
    public GameObject defeatCanvas;

    [Header("GameObjects a Cambiar")]
    public GameObject playerGun; //La pistola de agua en manos del jugador
    public GameObject puddle;   //Charco de agua
    public GameObject treeLadder;   //Escalera puesto en el arbol
    public GameObject treeRope;     //La cuerda que esta en la rama del arbol
    public GameObject treeStretchedRope;    //La cuerda estirada colgando de la rama del arbol
    public GameObject ropeInFloor;  //La cuerda en el piso despues de cortar la cuerda del arbol
    public GameObject glassOfWater; //Vaso de agua en la rama del arbol
    public GameObject paperPlane;   //Avion de papel al momento de ser lanzado
    public SpriteRenderer semaphore;    //Semaforo peatonal

    [Header("Sprites")]
    public Sprite emptyWater;
    public Sprite fullWater;
    public Sprite greenSemaphore;

    [Header("Tercer estrella y Sprite de Estrella")]
    public RawImage thirdStar;
    public Texture goldenStar;

    //Variables privadas
    private bool ladderUsed = false, paperUsed = false, scissorsUsed = false, gunUsed = false, ropeUsed = false; //Para saber si los objetos fueron usados
    private bool wellUsedL = false, wellUsedP = false, wellUsedS = false, wellUsedG = false, wellUsedR = false; //Para saber si los objetos fueron bien usados
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
        if (wellUsedL && wellUsedG && wellUsedS && wellUsedR) //Las condiciones para la victoria de 2 estrellas
        {
            gameObject.GetComponent<AudioSource>().Stop();
            victoryCanvas.gameObject.SetActive(true);
            victoryCanvas.GetComponent<AudioSource>().Play();
            ChangePlayerPrefs(2);
        }
        else if (wellUsedL && wellUsedP) //Las condiciones para la victoria de 3 estrellas
        {
            thirdStar.texture = goldenStar;
            gameObject.GetComponent<AudioSource>().Stop();
            victoryCanvas.gameObject.SetActive(true);
            victoryCanvas.GetComponent<AudioSource>().Play();
            ChangePlayerPrefs(3);
        }
        else if (ladderUsed && paperUsed && scissorsUsed && gunUsed) //Si todos los objetos fueron usados
        {
            gameObject.GetComponent<AudioSource>().Stop();
            defeatCanvas.gameObject.SetActive(true);
            defeatCanvas.GetComponent<AudioSource>().Play();
        }
        else if (ladderUsed && paperUsed && scissorsUsed && gunUsed && ropeUsed) //Si todos los objetos fueron usados
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

    //Funcion cuando un objeto es utilizo en el arbol
    public void TreeOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Escalera")
        {
            ladderUsed = true;
            wellUsedL = true;
            treeLadder.gameObject.SetActive(true);
            //Llamar a la corutina para subir la escalera
            StartCoroutine(ClimbTree());
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Avion")
        {
            paperUsed = true;
            Analisis();
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Cuerda")
        {
            ropeUsed = true;
            Analisis();
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Tijeras")
        {
            scissorsUsed = true;
            Analisis();
        }
        else
        {
            gunUsed = true;
            Analisis();
        }

        //Quitar objeto del inventario
        InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] = "";
        InventoryController.instance.images[InventoryController.instance.itemInUse].gameObject.SetActive(false);
    }

    //Funcion cuando se use objeto en la rama del arbol
    public void BranchOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Pistola")
        {
            gunUsed = true;
            
            if(wellUsedL)
            {
                wellUsedG = true;
                playerGun.gameObject.SetActive(true);
                SoundController.instance.sfxVolumes[2].Play();
                //Llamar a la corutina para disparar agua al caso y que la cuerda carga
                StartCoroutine(FillGlassOfWater());
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
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Avion")
        {
            paperUsed = true;
            Analisis();
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Tijeras")
        {
            scissorsUsed = true;
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

    //Funcion cuando se use un objeto debajo de la rama
    public void UnderBranchOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Tijeras")
        {
            scissorsUsed = true;

            if(wellUsedL && wellUsedG)
            {
                wellUsedS = true;
                treeStretchedRope.gameObject.SetActive(false);
                ropeInFloor.gameObject.SetActive(true);
            }
            else
            {
                Analisis();
            }
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Avion")
        {
            paperUsed = true;
            Analisis();
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Escalera")
        {
            ladderUsed = true;
            Analisis();
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Pistola")
        {
            gunUsed = true;
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
    public void SemaphoreOption()
    {
        if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Cuerda")
        {
            ropeUsed = true;

            if(wellUsedL && wellUsedG)
            {
                wellUsedR = true;
                //Llamar a la corutina para balancearse hasta el otro lado
                StartCoroutine(BalanceRope());
            }
            else
            {
                Analisis();
            }
        }
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Avion")
        {
            paperUsed = true;

            if(wellUsedL)
            {
                wellUsedP = true;
                paperPlane.gameObject.SetActive(true);
                //Llamar a la corutina para lanzar el avion de papel al semaforo peatonal
                StartCoroutine(ThrowPaperPlane());
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
        else if (InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] == "Tijeras")
        {
            scissorsUsed = true;
            Analisis();
        }
        else
        {
            gunUsed = true;
            Analisis();
        }

        //Quitar objeto del inventario
        InventoryController.instance.inventoryObjects[InventoryController.instance.itemInUse] = "";
        InventoryController.instance.images[InventoryController.instance.itemInUse].gameObject.SetActive(false);
    }

    //Funcion para actualizar variables del PlayerPrefs después de ganar
    private void ChangePlayerPrefs(int stars)
    {
        if (playableLevels < 2)
        {
            PlayerPrefs.SetInt("playableLevels", 2);
        }
        else if(PlayerPrefs.GetInt("playableLevels") < (level + 2))
        {
            PlayerPrefs.SetInt("playableLevels", playableLevels + 1);
        }

        if (levelStars < stars)
        {
            PlayerPrefs.SetInt("levelStars" + level, stars);
        }
    }

    //Corutina para subir el arbol despues de poner la escalera
    IEnumerator ClimbTree()
    {
        player.transform.position = new Vector3(-2.36f, -0.89f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-2.14f, -0.57f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-2.09f, -0.23f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-1.9f, 0.34f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-1.71f, 0.66f, 0f);
        yield return new WaitForSeconds(1.0f);
        Analisis();
    }

    //Corutina para llenar el vaso de agua
    IEnumerator FillGlassOfWater()
    {
        yield return new WaitForSeconds(1.0f);
        glassOfWater.GetComponent<SpriteRenderer>().sprite = fullWater;
        yield return new WaitForSeconds(1.5f);
        glassOfWater.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -45f));
        yield return new WaitForSeconds(0.5f);
        playerGun.gameObject.SetActive(false);
        glassOfWater.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
        glassOfWater.transform.position = new Vector3(3.44f, 0.059f, 0f);
        yield return new WaitForSeconds(0.5f);
        glassOfWater.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -135f));
        glassOfWater.transform.position = new Vector3(3.44f, -0.532f, 0f);
        yield return new WaitForSeconds(0.5f);
        glassOfWater.transform.position = new Vector3(3.44f, -0.93f, 0f);
        glassOfWater.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -180f));
        glassOfWater.GetComponent<SpriteRenderer>().sprite = emptyWater;
        yield return new WaitForSeconds(0.5f);
        SoundController.instance.sfxVolumes[3].Play();
        treeRope.gameObject.SetActive(false);
        glassOfWater.gameObject.SetActive(false);
        treeStretchedRope.gameObject.SetActive(true);
        puddle.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        Analisis();
    }

    //Corutina para balancearte con la cuerda
    IEnumerator BalanceRope()
    {
        player.transform.position = new Vector3(-1.41f, 0.49f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-1.04f, 0.3f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.71f, 0.1f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.34f, 0.11f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.03f, 0.28f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.47f, 0.44f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.87f, 0.5f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.18f, 0.33f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.41f, 0.09f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.7f, -0.71f, 0f);
        yield return new WaitForSeconds(1.0f);
        Analisis();
    }

    //Corutina para lanzar avion de papel al semaforo
    IEnumerator ThrowPaperPlane()
    {
        paperPlane.transform.position = new Vector3(-1.44f, 0.57f, 0f);
        yield return new WaitForSeconds(0.5f);
        paperPlane.transform.position = new Vector3(-1.08f, 0.27f, 0f);
        yield return new WaitForSeconds(0.5f);
        paperPlane.transform.position = new Vector3(-0.72f, 0.01f, 0f);
        yield return new WaitForSeconds(0.5f);
        paperPlane.transform.position = new Vector3(-0.38f, -0.25f, 0f);
        paperPlane.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 12f));
        yield return new WaitForSeconds(0.5f);
        paperPlane.transform.position = new Vector3(0.06f, -0.06f, 0f);
        paperPlane.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 40f));
        yield return new WaitForSeconds(0.5f);
        paperPlane.transform.position = new Vector3(0.57f, 0.08f, 0f);
        yield return new WaitForSeconds(0.5f);
        paperPlane.transform.position = new Vector3(0.93f, 0.32f, 0f);
        yield return new WaitForSeconds(0.5f);
        paperPlane.transform.position = new Vector3(1.26f, 0.45f, 0f);
        paperPlane.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 12f));
        yield return new WaitForSeconds(0.5f);
        paperPlane.gameObject.SetActive(false);
        semaphore.sprite = greenSemaphore;
        yield return new WaitForSeconds(0.5f);
        //Semaforo en Verde y el jugador empieza a caminar al otro lado
        player.transform.position = new Vector3(-1.59f, -2.27f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-1.04f, -2.27f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(-0.51f, -2.27f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.02f, -2.27f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(0.5f, -2.27f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.0f, -2.27f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.5f, -2.27f, 0f);
        yield return new WaitForSeconds(0.5f);
        player.transform.position = new Vector3(1.82f, -2.27f, 0f);
        yield return new WaitForSeconds(1.0f);
        Analisis();
    }
}
