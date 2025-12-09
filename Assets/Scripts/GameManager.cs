using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("Configuración de la Cuerda")]
    public Transform cuerda; // Arrastra la cuerda aquí
    public float distanciaBajadaPorPuzle = 3.5f; // Cuánto baja cada vez
    public float velocidadBajada = 2f; // Suavidad del movimiento

    [Header("Estado del Juego")]
    public int totalPuzlesEnEscena = 2; // Símbolos + Pipes
    private int puzlesResueltos = 0;
    private Vector3 posicionObjetivoCuerda;
    public bool pipesPuzzleCompleted = false;

    void Start()
    {
        if (cuerda != null)
            posicionObjetivoCuerda = cuerda.position;
    }

    void Update()
    {
        // Mueve la cuerda suavemente hacia su nueva posición
        if (cuerda != null && cuerda.position != posicionObjetivoCuerda)
        {
            cuerda.position = Vector3.Lerp(cuerda.position, posicionObjetivoCuerda, Time.deltaTime * velocidadBajada);
        }
    }

    // Esta es la función que llamarán tus puzles
    public void RegistrarPuzleCompletado()
    {
        puzlesResueltos++;
        Debug.Log($"Puzle completado! ({puzlesResueltos}/{totalPuzlesEnEscena})");

        // Calculamos la nueva posición hacia abajo
        posicionObjetivoCuerda.y -= distanciaBajadaPorPuzle;

        if (puzlesResueltos >= totalPuzlesEnEscena)
        {
            GanarJuego();
        }
    }

    void GanarJuego()
    {
        Debug.Log("¡VICTORIA! El jugador puede escapar.");
        // Aquí puedes detener el agua, activar un sonido de éxito 
        // o permitir que el jugador toque la cuerda para terminar.
    }

    [Header("Chequeo de Tuberías")]
    // Arrastra TODAS las piezas de tubería MÓVILES aquí
    public PipeStatus[] allPipePieces; 
    
    // Llama a esta función desde SocketStatusUpdater.cs
    public void CheckAllPipesSolved()
    {
        // Si ya está resuelto, no cheques de nuevo
        if (puzlesResueltos >= totalPuzlesEnEscena || pipesPuzzleCompleted) return;

        // Comprobación de que la lista no esté vacía
        if (allPipePieces == null || allPipePieces.Length == 0) return;

        // 1. Iterar sobre todas las tuberías
        foreach (PipeStatus pipe in allPipePieces)
        {
            // Si encuentra UNA SOLA tubería que no esté conectada, el puzle NO está completo
            if (!pipe.isConnected)
            {
                Debug.Log("Falta conectar una tubería.");
                return;
            }
            else
            {
                Debug.Log($"Tubería {pipe.myID} conectada.");
            }
        }
        pipesPuzzleCompleted = true;
        // 2. Si el bucle termina, todas están conectadas.
        RegistrarPuzleCompletado();
    }
}
