using UnityEngine;
using TMPro;  // Asegúrate de incluir TextMesh Pro
using UnityEngine.UI;  // Importa el espacio de nombres para UI
using System.Collections;

public class CombatController : MonoBehaviour
{
    public Button[] actionButtons;  // Botones de acciones del combate (4 botones)
    public TMP_Text feedbackText;   // Para mostrar los mensajes de éxito o fallo (TextMeshPro)
    public TMP_Text timerText;      // Para mostrar el temporizador en pantalla (TextMeshPro)
    public GameManager gameManager; // Para manejar la lógica del juego

    public float actionTimeLimit = 3f;  // Tiempo para presionar un botón, configurable en el Inspector
    public float cooldownTime = 1f;     // Cooldown entre botones, configurable en el Inspector

    private int currentButtonIndex = 0;  // Controla cuál botón debe presionar el jugador
    private bool isButtonPressable = true;  // Para gestionar si el botón puede ser presionado

    private float timer;  // Temporizador para cada acción
    private bool isTiming = false;  // Si el temporizador está en curso

    void Start()
    {
        foreach (Button button in actionButtons)
        {
            button.onClick.AddListener(OnActionButtonClicked);
            button.interactable = false;  // Desactivamos los botones al inicio
        }

        gameObject.SetActive(false);  // Inicialmente desactivamos el CombatController
    }

    void Update()
    {
        // Si el temporizador está en curso
        if (isTiming)
        {
            timer -= Time.deltaTime;
            timerText.text = "Tiempo: " + Mathf.Max(0, Mathf.Round(timer)).ToString();  // Mostrar el tiempo restante en pantalla

            if (timer <= 0)
            {
                // Si el tiempo se agota, mostramos el fallo
                feedbackText.text = "¡Fallaste! Tienes una oportunidad más.";
                isTiming = false;
                StartCoroutine(ButtonCooldown());  // Iniciamos el cooldown después del fallo
            }
        }
    }

    // Lógica cuando el jugador hace clic en un botón
    void OnActionButtonClicked()
    {
        if (!isButtonPressable) return;  // Si el botón no es presionable, no hace nada

        if (currentButtonIndex < actionButtons.Length)
        {
            // Verifica si el jugador presionó el botón correcto en el orden
            if (actionButtons[currentButtonIndex] == actionButtons[0])  // Si el botón presionado es el esperado
            {
                feedbackText.text = "¡Felicidades! Pasas al siguiente.";
                currentButtonIndex++;
                if (currentButtonIndex >= actionButtons.Length)  // Si el jugador termina el combate correctamente
                {
                    feedbackText.text = "¡Combate finalizado!";
                    gameManager.EndCombat();  // Finaliza el combate y vuelve al gameplay
                }

                // Iniciamos el cooldown después de presionar el botón correctamente
                isButtonPressable = false;
                StartCoroutine(ButtonCooldown());
            }
            else
            {
                feedbackText.text = "¡Fallaste! Tienes una oportunidad más.";
                isButtonPressable = false;
                StartCoroutine(ButtonCooldown());
            }
        }
    }

    // Función para manejar el cooldown entre acciones
    IEnumerator ButtonCooldown()
    {
        yield return new WaitForSeconds(cooldownTime);  // Espera el tiempo de cooldown
        isButtonPressable = true;  // Vuelve a habilitar el botón para presionar
        timer = actionTimeLimit;  // Reinicia el temporizador
        isTiming = true;  // Comienza el temporizador
    }

    // Función para comenzar la acción
    public void StartAction()
    {
        timer = actionTimeLimit;  // Establece el temporizador para esta acción
        isTiming = true;  // Inicia el temporizador
        feedbackText.text = "";  // Limpiar los mensajes de éxito/fracaso
        foreach (Button button in actionButtons)
        {
            button.interactable = true;  // Habilitar los botones para la acción
        }
    }
}
