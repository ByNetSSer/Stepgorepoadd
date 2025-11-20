using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class MissionWindow : MonoBehaviour
{
    [Header("UI Elements")]
    public Transform missionsContainer;
    public GameObject missionUIPrefab;
    public TextMeshProUGUI windowTitle;

    [Header("Window Type")]
    public MissionType windowType;

    public List<MissionUI> missionUIList = new List<MissionUI>();

    void Start()
    {
        SetWindowTitle();
    }

    public void RefreshMissions(List<Mission> missions)
    {
        // Limpiar misiones existentes
        foreach (Transform child in missionsContainer)
        {
            Destroy(child.gameObject);
        }
        missionUIList.Clear();

        // Crear UI para cada misión
        foreach (var mission in missions)
        {
            GameObject missionUIObject = Instantiate(missionUIPrefab, missionsContainer);
            MissionUI missionUI = missionUIObject.GetComponent<MissionUI>();

            if (missionUI != null)
            {
                missionUI.Initialize(mission);
                missionUIList.Add(missionUI);
            }
        }
    }

    void SetWindowTitle()
    {
        if (windowTitle != null)
        {
            switch (windowType)
            {
                case MissionType.Daily:
                    windowTitle.text = "Misiones Diarias";
                    break;
                case MissionType.Long:
                    windowTitle.text = "Misiones Largas";
                    break;
                case MissionType.Weekly:
                    windowTitle.text = "Misiones Semanales";
                    break;
            }
        }
    }

    public void ShowWindow()
    {
        gameObject.SetActive(true);
    }

    public void HideWindow()
    {
        gameObject.SetActive(false);
    }
}