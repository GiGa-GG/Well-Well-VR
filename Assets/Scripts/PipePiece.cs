using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class PipePiece : MonoBehaviour
{
    // lista solo en runtime (no serializable, así evitamos errores del Inspector)
    List<PipeSocket> connectedSockets = new List<PipeSocket>();
    public bool IsConnectedToOrigin = false;

    Rigidbody rb;
    XRGrabInteractable grab;

    // configuración
    [Header("Snap settings")]
    [Tooltip("Si true, al encajar la pieza se parenteará al socket (ideal)")]
    public bool parentOnAttach = true;

    [Tooltip("Si true, desactiva el XRGrabInteractable al quedar fijada")]
    public bool disableGrabWhenAttached = true;

    [Tooltip("Impulso al soltar (para evitar que se quede flotando)")]
    public float detachImpulse = 0.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    #region Conexiones
    public void AddConnection(PipeSocket socket)
    {
        if (!connectedSockets.Contains(socket))
            connectedSockets.Add(socket);

        UpdateChainState();
    }

    public void RemoveConnection(PipeSocket socket)
    {
        if (connectedSockets.Contains(socket))
            connectedSockets.Remove(socket);

        UpdateChainState();
    }
    #endregion

    #region Estado de cadena
    void UpdateChainState()
    {
        bool newConnectedToOrigin = connectedSockets.Exists(s => s != null && s.isOrigin);

        if (newConnectedToOrigin != IsConnectedToOrigin)
            IsConnectedToOrigin = newConnectedToOrigin;

        if (IsConnectedToOrigin)
            PropagateValidity();
        else
            CheckAndDropIfDangling();
    }

    void PropagateValidity()
    {
        foreach (var socket in connectedSockets)
        {
            if (socket == null) continue;

            var other = socket.connectedPipe;
            if (other != null && !other.IsConnectedToOrigin)
            {
                other.IsConnectedToOrigin = true;
                other.PropagateValidity();
            }
        }
    }

    void CheckAndDropIfDangling()
    {
        foreach (var socket in connectedSockets.ToArray())
        {
            if (socket == null) continue;

            var other = socket.connectedPipe;

            if (other != null && !other.IsConnectedToOrigin && !socket.isOrigin)
            {
                socket.ForceDetach();
            }
        }
    }
    #endregion

    #region Attach / Detach helpers

    public void OnAttachedToSocket(PipeSocket socket)
    {
        AddConnection(socket);
        IsConnectedToOrigin |= socket.isOrigin;
    }


    public void OnDetachedFromSocket(PipeSocket socket)
    {
        RemoveConnection(socket);

        // Restaurar físicas
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            // pequeño impulso para que no quede pegado (opcional)
            rb.AddForce(Vector3.down * detachImpulse, ForceMode.VelocityChange);
        }

        // Restaurar agarre COMPLETO
        if (grab != null)
        {
            grab.enabled = true;

            // ❗ RESTAURAR LAYER NORMAL (MUY IMPORTANTE)
            grab.interactionLayers = InteractionLayerMask.GetMask("Default");
        }

        transform.SetParent(null, true);
    }



    #endregion
}
