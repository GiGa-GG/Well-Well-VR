using UnityEngine;
using UnityEngine.SceneManagement;

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

    private bool wasSubmerged = false;

    [Header("Audio de Ahogamiento")]
    public AudioSource drowningAudioSource; // AudioSource en la Cámara VR
    public AudioClip drowningClip;         // Sonido de burbujas (debe ser loop)
    public AudioClip breathingClip;        // Sonido de respiración al salir del agua

    void Update()
    {
        // Altura del agua
        float waterHeight = waterPlane.position.y;
        // Altura de la cabeza del jugador
        float playerHeadHeight = transform.position.y;

        bool isSubmerged = (playerHeadHeight < waterHeight);

        if (isSubmerged)
        {
            // A. ESTANDO SUMERGIDO: Control de tiempo y audio ON
            
            // Incremento de tiempo (solo una vez)
            currentDrowningTime += Time.deltaTime; 

            // 1. Audio: Si el clip no está asignado, lo asignamos y empezamos a reproducir
            if (drowningAudioSource.clip != drowningClip)
            {
                drowningAudioSource.clip = drowningClip;
                drowningAudioSource.loop = true;
            }
            if (!drowningAudioSource.isPlaying)
            {
                drowningAudioSource.Play();
            }
            
            // 2. Registramos que ESTAMOS sumergidos AHORA
            wasSubmerged = true; 
        }
        else // Si NO está sumergido (en el aire)
        {
            // B. ESTANDO FUERA: Control de tiempo y transición de audio
            
            // 1. Lógica de Recuperación de Tiempo
            currentDrowningTime = Mathf.Max(0f, currentDrowningTime - Time.deltaTime * 3);
            
            // 2. DETECCIÓN DE LA TRANSICIÓN (wasSubmerged == true)
            if (wasSubmerged)
            {
                // Solo se ejecuta en el PRIMER FRAME fuera del agua
                
                // Detener el sonido de ahogamiento (burbujas)
                drowningAudioSource.Stop();
                
                // Reproducir el sonido de respiración UNA SOLA VEZ
                if (breathingClip != null)
                {
                    drowningAudioSource.PlayOneShot(breathingClip);
                }

                // Reiniciamos la bandera, la transición ha ocurrido
                wasSubmerged = false;
            }
        }
        
        // 3. --- LÓGICA DE VISUALES Y VICTORIA/DERROTA (Global) ---
        
        // Asegurarse de que el contador no exceda el límite
        currentDrowningTime = Mathf.Clamp(currentDrowningTime, 0f, drowningTimeLimit);

        // Controlar el Alpha del Overlay (El oscurecimiento)
        float targetAlpha = currentDrowningTime / drowningTimeLimit;
        drowningOverlay.alpha = Mathf.MoveTowards(drowningOverlay.alpha, targetAlpha, fadeSpeed * Time.deltaTime);

        // Condición de Pérdida
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

        SceneManager.LoadScene("LoseScene");

        // Aquí iría la lógica para cargar el menú de Game Over o reiniciar.
        // Time.timeScale = 0f; 
    }
}
