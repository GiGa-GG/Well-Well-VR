using UnityEngine;

public class PipeStatus : MonoBehaviour
{
    [Header("Estado de la Cadena")]
    // Bool que verifica si la pieza está enganchada a un socket
    public bool isConnected = false; 

    // Indica si esta pieza es la primera en el puzle (opcional)
    public bool isOriginPipe = false; 
    
    // Referencia al Manager para notificar la conexión
    public GameManager gameManager; 

    // Solo para debug en el Inspector, no necesario en el código
    public int myID = 0;
}
