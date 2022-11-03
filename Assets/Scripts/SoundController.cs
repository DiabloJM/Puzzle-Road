using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    //Variables publicas
    public static SoundController instance;

    [Header("Sliders de Musica y SFX")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Fuentes de Audio de Musica y SFXs")]
    public AudioSource musicVolume;
    public AudioSource[] sfxVolumes;

    private void Awake()
    {
        instance = this;
    }

    //Funcion para inicializar el volumen de la musica de los sfx
    private void Start()
    {
        //Le damos volumen a la musica con el valor que ya tenemos guardado
        musicVolume.volume = PlayerPrefs.GetFloat("musicVolume");
        //Le damos volumen a los sfx con el valor que ya tenemos guardado
        for(int i=0;i < sfxVolumes.Length; i++)
        {
            sfxVolumes[i].volume = PlayerPrefs.GetFloat("sfxVolume");
        }
        //Inicializamos los slider
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
    }

    //Funcion para cambiar y actualizar el volumen de la musica
    public void SetMusicVolume()
    {
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        musicVolume.volume = PlayerPrefs.GetFloat("musicVolume");
    }
    
    //Funcion para cambiar y actualizar el volumen de los SFX
    public void SetSFXvolume()
    {
        PlayerPrefs.SetFloat("sfxVolume", sfxSlider.value);

        for (int i = 0; i < sfxVolumes.Length; i++)
        {
            sfxVolumes[i].volume = PlayerPrefs.GetFloat("sfxVolume");
        }
    }
}
