using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    [Tooltip("El radio del círculo visual (en escala de Unity).")]
    public float radioVisual = 1.5f;
    
    [Tooltip("El color del Gizmo.")]
    public Color colorGizmo = Color.yellow;

    // Se llama automáticamente por Unity para dibujar Gizmos en la Scene View.
    void OnDrawGizmos()
    {
        // 1. Establecer el color del Gizmo.
        Gizmos.color = colorGizmo;

        // 2. Dibujar una esfera de alambre (wireframe) en la posición de este GameObject,
        //    usando el radio definido.
        //    Desde una vista superior (plano XZ), esto se verá como un círculo.
        //    Gizmos.DrawWireSphere(posición_del_centro, radio)
        Gizmos.DrawWireSphere(transform.position, radioVisual);
        
        // NOTA: Para un círculo perfectamente plano en el plano XZ, 
        // podrías necesitar lógica más compleja o un editor script. 
        // DrawWireSphere es la solución más sencilla y común para visualizaciones de área/radio.
    }
}
