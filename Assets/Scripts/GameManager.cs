using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Variables publicas
    [Header("Niveles y Textos")]
    public GameObject[] levels;     //Todos los niveles

    [Header("Sprites de Caja")]
    public Sprite boxDefault;
    public Sprite boxLock;

    [Header("Sprite de Estrella Dorada")]
    public Sprite starGolden;

    //Variables privadas
    private int playableLevels; //Cantidad de niveles desbloqueados
    private int levelStars;
    private int actualLevel;

    private void Awake()
    {
        playableLevels = PlayerPrefs.GetInt("playableLevels");
    }

    private void Start()
    {
        InitializeLevelsCanvas();
    }

    //Funcion para inicializar el canvas de niveles
    private void InitializeLevelsCanvas()
    {
        for(int i = 0; i < playableLevels; i++)
        {
            levelStars = PlayerPrefs.GetInt("levelStars" + i);
            actualLevel = i + 1;

            //Quitar la imagen de candado y activar boton
            levels[i].GetComponent<Image>().sprite = boxDefault;
            levels[i].GetComponent<Button>().enabled = true;
            levels[i].transform.GetChild(0).GetComponent<Text>().text = actualLevel.ToString();

            //Actualizar las estrellas del nivel
            if (levelStars > 0)
            {
                for (int j = 1; j <= levelStars; j++)
                {
                    levels[i].transform.GetChild(j).gameObject.GetComponent<Image>().sprite = starGolden;
                }
            }
        }
    }

    //Funcion para resetear TODOS los valores guardados en PlayerPrefs
    /*
    private void ResetPlayerPrefs()
    {
        for(int i = 0; i < 6; i++)
        {
            PlayerPrefs.SetInt("levelStars" + i, 0);
        }

        PlayerPrefs.SetInt("playableLevels", 1);
        PlayerPrefs.SetFloat("musicVolume", 0.8f);
        PlayerPrefs.SetFloat("sfxVolume", 0.5f);
    }
    */
}