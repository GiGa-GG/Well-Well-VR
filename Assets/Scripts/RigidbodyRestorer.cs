using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class RigidbodyRestorer : MonoBehaviour
{
    private XRSocketInteractor socketInteractor;

    void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();

        if (socketInteractor != null)
        {
            // Suscribirse al evento que se dispara cuando una tubería es sacada del socket
            socketInteractor.selectExited.AddListener(RestorePhysics);
        }
    }

    private void RestorePhysics(SelectExitEventArgs args)
    {
        // El objeto que salió del socket es el Interactable (la tubería)
        if (args.interactableObject.transform.TryGetComponent(out Rigidbody rb))
        {
            // 1. Descongelar el Rigidbody
            rb.isKinematic = false;
            rb.useGravity = true;

            // 2. Reactivar el agarre (XRGrabInteractable)
            if (args.interactableObject.transform.TryGetComponent(out XRGrabInteractable grab))
            {
                grab.enabled = true;
            }

            // 3. (Opcional) Desactivar el socket de la cadena si es un socket libre
            if (gameObject.GetComponent<XRSocketInteractor>() != null && gameObject.name.Contains("SocketLibre"))
            {
                gameObject.SetActive(false);
            }
        }
    }
}