using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SocketDestroyer : MonoBehaviour
{
    [Header("Objetos a Destruir")]
    // Arrastra los objetos que deben desaparecer al completarse el enganche
    public GameObject[] targetObjectsToDestroy;

    private XRSocketInteractor socketInteractor;

    void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
        
        if (socketInteractor != null)
        {
            // Nos suscribimos al evento que se dispara cuando un objeto entra y se engancha.
            socketInteractor.selectEntered.AddListener(OnKeyConnected);
        }
    }

    private void OnKeyConnected(SelectEnterEventArgs args)
    {
        // Verificar si la acción ya ocurrió para no duplicar la destrucción
        if (targetObjectsToDestroy == null || targetObjectsToDestroy.Length == 0) return;
        
        Debug.Log("Conexión detectada. Iniciando destrucción de objetos objetivo.");

        // Recorremos la lista y destruimos cada objeto asignado
        foreach (GameObject obj in targetObjectsToDestroy)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        
        
        this.enabled = false; 
    }
}
