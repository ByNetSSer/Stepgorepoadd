using UnityEngine;

public class MissionUIManager : MonoBehaviour
{
    [Header("Mission Windows")]
    public MissionWindow dailyMissionsWindow;
    public MissionWindow longMissionsWindow;
    public MissionWindow weeklyMissionsWindow;

    [Header("Navigation Buttons")]
    public GameObject dailyButton;
    public GameObject longButton;
    public GameObject weeklyButton;

    void Start()
    {
        // Mostrar misiones diarias por defecto
        ShowDailyMissions();
    }

    public void ShowDailyMissions()
    {
        HideAllWindows();
        dailyMissionsWindow.ShowWindow();
        UpdateButtonStates(MissionType.Daily);
    }

    public void ShowLongMissions()
    {
        HideAllWindows();
        longMissionsWindow.ShowWindow();
        UpdateButtonStates(MissionType.Long);
    }

    public void ShowWeeklyMissions()
    {
        HideAllWindows();
        weeklyMissionsWindow.ShowWindow();
        UpdateButtonStates(MissionType.Weekly);
    }

    void HideAllWindows()
    {
        dailyMissionsWindow.HideWindow();
        longMissionsWindow.HideWindow();
        weeklyMissionsWindow.HideWindow();
    }

    void UpdateButtonStates(MissionType activeType)
    {
        // Aquí puedes cambiar los colores o estados de los botones
        // para indicar cuál está activo
    }

    // Para abrir/cerrar el panel completo de misiones
    public void ToggleMissionsPanel()
    {
        bool isActive = !gameObject.activeSelf;
        gameObject.SetActive(isActive);

        if (isActive)
        {
            RefreshAllWindows();
        }
    }

    void RefreshAllWindows()
    {
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.RefreshMissionWindows();
        }
    }
}