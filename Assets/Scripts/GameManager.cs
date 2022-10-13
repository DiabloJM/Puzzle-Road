using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Variables publicas
    [HideInInspector]
    public static GameManager instance; //Unica instancia de este script

    [Header("Niveles")]
    public int playableLevels = PlayerPrefs.GetInt("playableLevels"); //Cantidad de niveles desbloqueados
    public GameObject[] levels;     //Todos los niveles

    [Header("Sprites de Estrellas")]
    public Sprite starGray;
    public Sprite starGolden;

    [Header("Sprite de Bloque de Nivel")]
    public Sprite defaultUI;

    [Header("Volumen de sonidos")]
    [SerializeField] Slider volumeSlider;

    private void Awake()
    {
        gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("musicVolume");
        gameObject.GetComponent<AudioSource>().Play();
    }

    private void Update()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
        gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("musicVolume");
    }

    private void InitializeLevelsCanvas()
    {

    }

    //Funcion para cuando se termina un nivel
    public void FinishLevel(int level, int stars)
    {
        if(stars == 1)
        {
            levels[level - 1].transform.GetChild(1).GetComponent<Image>().sprite = starGolden;
        }
        if(stars == 2)
        {
            levels[level - 1].transform.GetChild(1).GetComponent<Image>().sprite = starGolden;
            levels[level - 1].transform.GetChild(2).GetComponent<Image>().sprite = starGolden;
        }
        if(stars == 3)
        {
            levels[level - 1].transform.GetChild(1).GetComponent<Image>().sprite = starGolden;
            levels[level - 1].transform.GetChild(2).GetComponent<Image>().sprite = starGolden;
            levels[level - 1].transform.GetChild(3).GetComponent<Image>().sprite = starGolden;
        }

        levels[level].GetComponent<Image>().sprite = defaultUI;
        levels[level].GetComponent<Button>().enabled = true;
    }
}