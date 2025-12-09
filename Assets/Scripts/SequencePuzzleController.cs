using UnityEngine;
using System.Collections.Generic;

using System.Collections; // Necesario para Corrutinas
using TMPro; // NECESARIO para manipular el componente de texto

public class SequencePuzzleController : MonoBehaviour
{
    [Header("Configuración del Puzle")]
    public List<int> correctSequence = new List<int> { 2, 4, 3, 1 };
    private List<int> playerSequence = new List<int>();

    [Header("Referencias de Botones")]
    public GameObject[] symbolButtons; // Arrastra los 4 Prefabs de botón aquí

    [Header("Retroalimentación")]
    public Color correctColor = Color.green; // Color para secuencia correcta (VERDE)
    public Color wrongColor = Color.red;    // Color para secuencia incorrecta (ROJO)

    // Diccionario para almacenar el color original de CADA SÍMBOLO de texto
    private Dictionary<GameObject, Color32> originalTextColors = new Dictionary<GameObject, Color32>();
    public GameManager gameManager;

    void Start()
    {
        // 1. Asigna la lógica de clic y guarda el color original
        for (int i = 0; i < symbolButtons.Length; i++)
        {
            int buttonIndex = i + 1; // Índice (1, 2, 3, 4) para el puzle
            GameObject currentButton = symbolButtons[i];

            // Forma corta de obtener el componente de interacción
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable = currentButton.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();

            // Obtener el componente TextMeshPro del objeto hijo (el símbolo)
            TMP_Text tmpText = currentButton.GetComponentInChildren<TMP_Text>();

            if (interactable != null)
            {
                // Conecta el evento de presión al código
                interactable.selectEntered.AddListener(delegate { OnButtonPress(buttonIndex, currentButton); });
            }

            // Guarda el color original del texto
            if (tmpText != null && !originalTextColors.ContainsKey(currentButton))
            {
                originalTextColors.Add(currentButton, tmpText.color);
            }
        }
    }

    private void OnButtonPress(int index, GameObject button)
    {
        if (playerSequence.Count >= correctSequence.Count)
        {
            ResetSequence();
        }

        playerSequence.Add(index);

        // Muestra retroalimentación: el símbolo flashea AMARILLO al presionarse
        StartCoroutine(FlashColor(button, Color.yellow, 0.2f));

        if (playerSequence.Count == correctSequence.Count)
        {
            CheckSequence();
        }
    }

    private void CheckSequence()
    {
        // Compara las dos listas elemento por elemento
        for (int i = 0; i < correctSequence.Count; i++)
        {
            if (playerSequence[i] != correctSequence[i])
            {
                // SECUENCIA INCORRECTA
                StartCoroutine(FlashAllButtons(wrongColor, 0.5f));
                ResetSequence();
                return;
            }
        }

        // --- SECUENCIA CORRECTA ---
        StartCoroutine(FlashAllButtons(correctColor, 0.5f));
        
        // AQUÍ VA LA LLAMADA AL MANAGER
        if (gameManager != null)
        {
            gameManager.RegistrarPuzleCompletado();
        }

        // Desactiva los botones (el código que ya tenías)
        foreach (GameObject btn in symbolButtons)
        {
            var interactable = btn.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
            if (interactable != null)
            {
                interactable.enabled = false;
            }
        }
    }

    private void ResetSequence()
    {
        playerSequence.Clear();
    }

    // Rutina para cambiar el color de un solo símbolo y luego restaurarlo
    IEnumerator FlashColor(GameObject button, Color color, float duration)
    {
        TMP_Text tmpText = button.GetComponentInChildren<TMP_Text>();
        Color originalColor = Color.white;

        if (tmpText != null && originalTextColors.ContainsKey(button))
        {
            originalColor = originalTextColors[button];
            tmpText.color = color;

            yield return new WaitForSeconds(duration);

            // Restaura el color original desde el diccionario
            tmpText.color = originalColor;
        }
        else if (tmpText != null)
        {
            // Caso de emergencia si no se guardó en el diccionario
            tmpText.color = color;
            yield return new WaitForSeconds(duration);
            tmpText.color = originalColor;
        }
    }

    // Rutina para flashear todos los símbolos (Resultado Final)
    IEnumerator FlashAllButtons(Color targetColor, float duration)
    {
        // Aplicar el color de resultado (ROJO o VERDE) a todos los símbolos
        foreach (GameObject btn in symbolButtons)
        {
            TMP_Text tmpText = btn.GetComponentInChildren<TMP_Text>();
            if (tmpText != null)
            {
                tmpText.color = targetColor;
            }
        }
        yield return new WaitForSeconds(duration);

        // Restaurar el color original de cada símbolo desde el diccionario
        foreach (GameObject btn in symbolButtons)
        {
            TMP_Text tmpText = btn.GetComponentInChildren<TMP_Text>();
            if (tmpText != null && originalTextColors.ContainsKey(btn))
            {
                tmpText.color = originalTextColors[btn];
            }
        }
    }
}