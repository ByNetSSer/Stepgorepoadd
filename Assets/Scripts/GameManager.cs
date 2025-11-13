using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameplayUIController gameplayUIController;
    public CombatController combatController;  // Referencia al CombatController en la misma escena

    public void StartCombat()
    {
        // Desactiva el popup de confirmación
        gameplayUIController.combatPopup.SetActive(false);

        // Activamos el CombatController para manejar la lógica del combate
        combatController.gameObject.SetActive(true);
    }

    public void EndCombat()
    {
        // Desactivamos el panel de combate y lo dejamos oculto
        gameplayUIController.combatPanel.SetActive(false);

        // Desactivamos el CombatController al final del combate
        combatController.gameObject.SetActive(false);
    }
}
