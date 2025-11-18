using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RigidbodyRestorer : MonoBehaviour
{
    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        if (grabInteractable != null)
        {
            // Suscribe la función al evento de cuando el objeto es soltado (deseleccionado)
            grabInteractable.selectExited.AddListener(OnSelectExit);
        }
    }

    private void OnSelectExit(SelectExitEventArgs args)
    {
        // Esta función se ejecuta JUSTO después de que el objeto es soltado.
        // Forzamos la desactivación de Is Kinematic y reactivamos la gravedad.
        if (rb != null)
        {
            // Espera un frame para que el XRGrabInteractable complete su lógica de soltar
            StartCoroutine(RestoreRigidbodySettings());
        }
    }

    System.Collections.IEnumerator RestoreRigidbodySettings()
    {
        // Espera un frame. Vital para que funcione con el XRIT.
        yield return null; 
        
        // ******* ¡SOLUCIÓN! *******
        rb.isKinematic = false;
        rb.useGravity = true;
    }
}