using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class PipeSocket : MonoBehaviour
{
    public bool isOrigin = false;
    public bool isGoal = false;

    public PipePiece connectedPipe { get; private set; }

    XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    void OnEnable()
    {
        socket.selectEntered.AddListener(OnInsert);
        socket.selectExited.AddListener(OnRemove);
    }

    void OnDisable()
    {
        socket.selectEntered.RemoveListener(OnInsert);
        socket.selectExited.RemoveListener(OnRemove);
    }

    void OnInsert(SelectEnterEventArgs args)
    {
        var piece = args.interactableObject.transform.GetComponentInParent<PipePiece>();
        if (piece == null) return;

        connectedPipe = piece;

        // ❗ Fuerza las físicas inmediatamente
        var rb = piece.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // ❗ Deshabilitar agarre inmediatamente
        var grab = piece.GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.enabled = false;
            grab.interactionLayers = LayerMask.GetMask("Nothing");
        }

        // ❗ Posicionar exactamente
        if (socket.attachTransform != null)
            piece.transform.SetPositionAndRotation(socket.attachTransform.position, socket.attachTransform.rotation);

        // ❗ Parent estable
        piece.transform.SetParent(this.transform, true);

        // Notifica a la pieza (tu lógica)
        piece.OnAttachedToSocket(this);
    }

    void OnRemove(SelectExitEventArgs args)
    {
        var piece = args.interactableObject.transform.GetComponentInParent<PipePiece>();
        if (piece == null) return;

        piece.OnDetachedFromSocket(this);

        if (connectedPipe == piece)
            connectedPipe = null;
    }

    public void ForceDetach()
    {
        if (connectedPipe == null) return;

        // Intentar forzar SelectExit vía InteractionManager
        var interactable = connectedPipe.GetComponent<XRBaseInteractable>();
        var manager = socket != null ? socket.interactionManager : null;

        IXRSelectInteractor interactor = socket as IXRSelectInteractor;
        IXRSelectInteractable interactableInt = interactable as IXRSelectInteractable;

        if (manager != null && interactor != null && interactableInt != null)
        {
            try
            {
                manager.SelectExit(interactor, interactableInt);
                // limpieza extra por bugs en XR: evitar "interactorsSelecting" fantasma
                interactableInt.interactorsSelecting.Clear();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"ForceDetach: SelectExit falló: {ex.Message}");
            }
        }
        else
        {
            // Fallback: simplemente llamar al detach local si el manager no está disponible
            Debug.LogWarning("ForceDetach: InteractionManager o interfaces no disponibles, haciendo fallback local.");
        }

        // Asegurar que la pieza restaure físicas y estado
        try
        {
            connectedPipe.OnDetachedFromSocket(this);
        }
        catch { /* swallow */ }

        connectedPipe = null;
    }

}
