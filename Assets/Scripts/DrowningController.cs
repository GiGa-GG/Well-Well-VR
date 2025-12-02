using UnityEngine;

public class DrowningController : MonoBehaviour
{
    [Header("Referencias")]
    // Arrastra el WaterPlane aquí (el objeto con el script WaterRiser)
    public Transform waterPlane;
    // Arrastra el Canvas Group del DrowningOverlay aquí
    public CanvasGroup drowningOverlay;

    [Header("Configuración")]
    // Tiempo en segundos antes de Game Over (ej: 5 segundos)
    public float drowningTimeLimit = 5f;
    // Velocidad a la que se recupera/oscurece el Alpha (ej: 0.5)
    public float fadeSpeed = 0.5f;

    private float currentDrowningTime = 0f;

    void Update()
    {
        // Altura del agua
        float waterHeight = waterPlane.position.y;
        // Altura de la cabeza del jugador
        float playerHeadHeight = transform.position.y;

        // 1. Comprobación de Submersión
        if (playerHeadHeight < waterHeight)
        {
            // Bajo el agua: el contador de ahogamiento sube
            currentDrowningTime += Time.deltaTime;
        }
        else
        {
            // Fuera del agua: el contador baja (la recuperación es más rápida)
            currentDrowningTime = Mathf.Max(0f, currentDrowningTime - Time.deltaTime * 3);
        }

        // Asegurarse de que el contador no exceda el límite
        currentDrowningTime = Mathf.Clamp(currentDrowningTime, 0f, drowningTimeLimit);

        // 2. Control del Efecto Visual (Oscurecimiento)
        // Calcula el Alpha basado en el porcentaje de tiempo sumergido
        float targetAlpha = currentDrowningTime / drowningTimeLimit;

        // Mueve el Alpha suavemente hacia el valor objetivo (oscurecer/aclarar)
        drowningOverlay.alpha = Mathf.MoveTowards(drowningOverlay.alpha, targetAlpha, fadeSpeed * Time.deltaTime);

        // 3. Condición de Pérdida
        if (currentDrowningTime >= drowningTimeLimit)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        // Lógica de Game Over
        Debug.Log("¡JUEGO TERMINADO! Te has ahogado.");

        // Detener la subida del agua
        waterPlane.GetComponent<WaterRisingController>().enabled = false;

        // Aquí iría la lógica para cargar el menú de Game Over o reiniciar.
        // Time.timeScale = 0f; 
    }
}
