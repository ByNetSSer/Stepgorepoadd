using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionUIManager : MonoBehaviour
{
    [Header("Mission Windows")]
    public MissionWindow dailyMissionsWindow;
    public MissionWindow longMissionsWindow;
    public MissionWindow weeklyMissionsWindow;

    [Header("Navigation Buttons")]
    public Button dailyButton;
    public Button longButton;
    public Button weeklyButton;

    [Header("Button Colors")]
    public Color normalColor = Color.white;
    public Color activeColor = Color.green;

    private MissionType currentActiveType = MissionType.Daily;

    void Start()
    {
        // Verificar que todas las referencias estén asignadas
        if (dailyMissionsWindow == null || longMissionsWindow == null || weeklyMissionsWindow == null)
        {
            Debug.LogError("❌ MissionUIManager: Faltan referencias a MissionWindows");
            return;
        }

        if (dailyButton == null || longButton == null || weeklyButton == null)
        {
            Debug.LogError("❌ MissionUIManager: Faltan referencias a los botones");
            return;
        }

        // Configurar listeners de botones
        dailyButton.onClick.AddListener(ShowDailyMissions);
        longButton.onClick.AddListener(ShowLongMissions);
        weeklyButton.onClick.AddListener(ShowWeeklyMissions);

        // Mostrar misiones diarias por defecto
        ShowDailyMissions();

        Debug.Log("✅ MissionUIManager inicializado correctamente");
    }

    public void ShowDailyMissions()
    {
        Debug.Log("🔄 Mostrando misiones diarias");
        HideAllWindows();
        dailyMissionsWindow.ShowWindow();
        currentActiveType = MissionType.Daily;
        UpdateButtonStates();
    }

    public void ShowLongMissions()
    {
        Debug.Log("🔄 Mostrando misiones largas");
        HideAllWindows();
        longMissionsWindow.ShowWindow();
        currentActiveType = MissionType.Long;
        UpdateButtonStates();
    }

    public void ShowWeeklyMissions()
    {
        Debug.Log("🔄 Mostrando misiones semanales");
        HideAllWindows();
        weeklyMissionsWindow.ShowWindow();
        currentActiveType = MissionType.Weekly;
        UpdateButtonStates();
    }

    void HideAllWindows()
    {
        if (dailyMissionsWindow != null)
        {
            dailyMissionsWindow.HideWindow();
            Debug.Log("📭 Ocultando ventana diaria");
        }

        if (longMissionsWindow != null)
        {
            longMissionsWindow.HideWindow();
            Debug.Log("📭 Ocultando ventana larga");
        }

        if (weeklyMissionsWindow != null)
        {
            weeklyMissionsWindow.HideWindow();
            Debug.Log("📭 Ocultando ventana semanal");
        }
    }

    void UpdateButtonStates()
    {
        // Resetear todos los botones
        ResetButtonColors();

        // Resaltar botón activo
        switch (currentActiveType)
        {
            case MissionType.Daily:
                SetButtonActive(dailyButton, true);
                break;
            case MissionType.Long:
                SetButtonActive(longButton, true);
                break;
            case MissionType.Weekly:
                SetButtonActive(weeklyButton, true);
                break;
        }

        Debug.Log($"🎯 Botón activo: {currentActiveType}");
    }

    void ResetButtonColors()
    {
        SetButtonActive(dailyButton, false);
        SetButtonActive(longButton, false);
        SetButtonActive(weeklyButton, false);
    }

    void SetButtonActive(Button button, bool isActive)
    {
        if (button == null) return;

        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = isActive ? activeColor : normalColor;
        }

        // También puedes cambiar el color del texto si lo tiene
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.color = isActive ? activeColor : normalColor;
        }
    }

    // Para abrir/cerrar el panel completo de misiones
    public void ToggleMissionsPanel()
    {
        bool isActive = !gameObject.activeSelf;
        gameObject.SetActive(isActive);

        Debug.Log($"📋 Panel de misiones: {(isActive ? "ABIERTO" : "CERRADO")}");

        if (isActive)
        {
            RefreshAllWindows();
            // Restaurar la vista activa
            SwitchToCurrentActiveView();
        }
    }

    void SwitchToCurrentActiveView()
    {
        switch (currentActiveType)
        {
            case MissionType.Daily:
                ShowDailyMissions();
                break;
            case MissionType.Long:
                ShowLongMissions();
                break;
            case MissionType.Weekly:
                ShowWeeklyMissions();
                break;
        }
    }

    void RefreshAllWindows()
    {
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.RefreshMissionWindows();
            Debug.Log("🔄 Todas las ventanas de misiones actualizadas");
        }
        else
        {
            Debug.LogWarning("⚠️ MissionManager.Instance no encontrado");
        }
    }

    // Método para debuggear el estado actual
    public void DebugCurrentState()
    {
        Debug.Log($"🔍 Estado actual - Tipo activo: {currentActiveType}");
        Debug.Log($"🔍 DailyWindow activo: {dailyMissionsWindow.gameObject.activeSelf}");
        Debug.Log($"🔍 LongWindow activo: {longMissionsWindow.gameObject.activeSelf}");
        Debug.Log($"🔍 WeeklyWindow activo: {weeklyMissionsWindow.gameObject.activeSelf}");
    }

    // Para limpiar listeners cuando se destruya el objeto
    void OnDestroy()
    {
        if (dailyButton != null) dailyButton.onClick.RemoveAllListeners();
        if (longButton != null) longButton.onClick.RemoveAllListeners();
        if (weeklyButton != null) weeklyButton.onClick.RemoveAllListeners();
    }
}