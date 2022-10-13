using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    //Funcion para cargar escena
    public void GoToScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    //Funcion para desactivar un canvas
    public void DeactivateCanvas(GameObject canvas)
    {
        canvas.SetActive(false);
    }

    //Funcion para activar un canvas
    public void ActivateCanvas(GameObject canvas)
    {
        canvas.SetActive(true);
    }
}