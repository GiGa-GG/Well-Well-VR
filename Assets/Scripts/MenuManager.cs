using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TestButtonClick(string buttonName)
    {
        Debug.Log("✅ ¡Botón Clicado Correctamente en VR! Botón: " + buttonName);

        // Simulación de iniciar partida:
        SceneManager.LoadScene("0");
    }

    public void BackButtonClick(string buttonName)
    {
        SceneManager.LoadScene("BasicScene");
    }

}
