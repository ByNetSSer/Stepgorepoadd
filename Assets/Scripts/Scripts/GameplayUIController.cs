using UnityEngine;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject combatPopup;  // Panel emergente para confirmar el combate
    public Button combatButton;  // Botón para iniciar combate
    public GameObject combatPanel;  // El panel de combate (inicialmente oculto)
    public CombatController combatController;  // El script que maneja la lógica del combate

    void Start()
    {
        combatButton.onClick.AddListener(OnCombatButtonClicked);
        combatPopup.SetActive(false);  // Aseguramos que el popup esté oculto al inicio
        combatPanel.SetActive(false);  // Ocultamos el panel de combate al inicio
    }

    void OnCombatButtonClicked()
    {
        combatPopup.SetActive(true);  // Muestra el popup de confirmación
    }

    public void OnCombatConfirmation(bool confirm)
    {
        if (confirm)
        {
            combatPopup.SetActive(false);  // Cierra el popup
            gameManager.StartCombat();  // Inicia el combate

            combatPanel.SetActive(true);  // Muestra el panel de combate
            combatController.StartAction();  // Inicia la acción, comienza el temporizador
        }
        else
        {
            combatPopup.SetActive(false);  // Cierra el popup si no desea ir al combate
        }
    }
}
