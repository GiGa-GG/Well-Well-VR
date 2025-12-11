using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ValvePuzzleController : MonoBehaviour
{
    [Header("Referencias del Puzle")]
    // Referencia al objeto que gestiona el estado del juego (PuzleManager, etc.)
    public GameManager gameManager; 
    
    // Objeto opcional: La tubería o el mecanismo que debe destruirse/desaparecer
    public GameObject mechanismToDestroy; 

    private XRSocketInteractor valveSocket;
    private bool isPuzzleCompleted = false;

    void Awake()
    {
        // Obtiene el Socket Interactor de este objeto (la tubería)
        valveSocket = GetComponent<XRSocketInteractor>();
        
        if (valveSocket != null)
        {
            // Nos suscribimos al evento que se dispara cuando un objeto entra al socket y se engancha.
            valveSocket.selectEntered.AddListener(OnValveConnected);
        }
    }

    private void OnValveConnected(SelectEnterEventArgs args)
    {
        // 1. Verificar si ya completamos el puzle (evita doble conteo)
        if (isPuzzleCompleted) return;

        // 2. Iniciar la secuencia de congelamiento/notificación
        // El objeto enganchado es args.interactableObject.transform.gameObject
        StartCoroutine(FinalizeValvePuzzle(args.interactableObject.transform.gameObject));
    }
    
    private IEnumerator FinalizeValvePuzzle(GameObject valveObject)
    {
        // Espera un frame. ¡Esto es VITAL! Permite que el sistema XR complete el snap y la alineación.
        yield return null; 

        if(gameManager.valvePuzzleCompleted)
            yield break;

        // 1. Congelar la Válvula: La volvemos Kinematic para que se quede fija
        if (valveObject.TryGetComponent(out Rigidbody rb) && rb != null)
        {
            // ***** CÓDIGO DE DETENCIÓN DE FÍSICA *****
            rb.angularVelocity = Vector3.zero; 
            rb.linearVelocity = Vector3.zero; 
            // ***** CÓDIGO PARA FORZAR LA ROTACIÓN *****
            // Creamos un nuevo Quaternion (rotación) a partir de los ángulos de Euler deseados.
            Quaternion targetRotation = Quaternion.Euler(0f, 48f, 0f);
            
            // Aplicamos la rotación al Transform del objeto.
            valveObject.transform.rotation = targetRotation; 
            
            rb.isKinematic = true; 
            rb.useGravity = false;
        }
        
        // 2. Desactivar el XRGrabInteractable para que no se pueda agarrar más
        if (valveObject.TryGetComponent(out XRGrabInteractable grab))
        {
            grab.enabled = false;
        }

        // 3. Notificar y Limpiar
        isPuzzleCompleted = true;
        Debug.Log("¡Válvula conectada y congelada! Puzle completado.");

        if (gameManager != null)
        {
            // Asumiendo que tu manager tiene una función para registrar este puzle
            gameManager.RegistrarPuzleCompletado(); 
            gameManager.valvePuzzleCompleted = true;
        }

        if (mechanismToDestroy != null)
        {
            Destroy(mechanismToDestroy);
        }
        
        // NOTA: Dejamos valveSocket.enabled = true, ya que el objeto ahora está congelado y no agarrable.
    }
}
