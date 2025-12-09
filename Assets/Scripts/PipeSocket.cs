using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class PipeSocket : MonoBehaviour
{
    private XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
        if (socket != null)
        {
            socket.selectEntered.AddListener(OnPipeInserted);
            socket.selectExited.AddListener(OnPipeRemoved);
        }
    }

    void OnPipeInserted(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.TryGetComponent(out PipeStatus pipe))
        {
            pipe.isConnected = true; // El bool de la tubería se pone a TRUE
            
            // Llamar al chequeo de victoria en el Manager (que ahora será global)
            if (pipe.gameManager != null)
            {
                pipe.gameManager.CheckAllPipesSolved();
            }
        }
    }

    void OnPipeRemoved(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.TryGetComponent(out PipeStatus pipe))
        {
            pipe.isConnected = false; // El bool de la tubería se pone a FALSE
        }
    }

}
