using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [Header("Mission Settings")]
    public int maxDailyResets = 3;
    public int baseDailySteps = 1000;
    public int dailyStepIncrement = 500;

    [Header("UI References")]
    public MissionWindow dailyMissionsWindow;
    public MissionWindow longMissionsWindow;
    public MissionWindow weeklyMissionsWindow;

    private List<Mission> allMissions = new List<Mission>();
    private DateTime lastResetTime;
    public UnityEvent Oncompletemission;
    private const string MISSIONS_KEY = "MissionsData";
    private const string LAST_RESET_KEY = "LastResetTime";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeMissions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (StepCounter.Instance != null)
        {
            StepCounter.Instance.OnStepsUpdated += OnStepsUpdated;
        }

        UpdateAllMissionProgress();
        RefreshMissionWindows();
    }

    void InitializeMissions()
    {
        LoadMissions();

        if (allMissions.Count == 0 || ShouldResetMissions())
        {
            GenerateMissions();
        }

        CheckMissionExpiration();
    }

    // ---------------------------------------------------------------
    //      GENERACIÓN DE MISIONES ACTUALIZADA
    // ---------------------------------------------------------------
    public void GenerateMissions()
    {
        allMissions.Clear();

        // 3 Misiones diarias
        for (int i = 0; i < 3; i++)
        {
            int stepsRequired = baseDailySteps + (i * dailyStepIncrement);
            int reward = 100 * (i + 1);  // Recompensas escaladas

            allMissions.Add(new Mission(
                $"daily_{i}",
                $"Misión Diaria {i + 1}",
                $"Camina {stepsRequired} pasos hoy",
                stepsRequired,
                reward,
                MissionType.Daily,
                i
            ));
        }

        // Misiones largas
        allMissions.Add(new Mission(
            "long_1",
            "Explorador Inicial",
            "Camina 5000 pasos en 3 días",
            5000,
            300,
            MissionType.Long
        ));

        allMissions.Add(new Mission(
            "long_2",
            "Caminante Experto",
            "Camina 10000 pasos en 3 días",
            10000,  // 🔥 CORREGIDO: era 700, debería ser mayor
            500,
            MissionType.Long
        ));

        // Misiones semanales
        allMissions.Add(new Mission(
            "weekly_1",
            "Maratoniano Semanal",
            "Camina 25000 pasos esta semana",
            25000,
            1200,
            MissionType.Weekly
        ));

        allMissions.Add(new Mission(
            "weekly_2",
            "Aventurero Semanal",
            "Camina 15000 pasos esta semana",
            15000,
            800,
            MissionType.Weekly
        ));

        lastResetTime = DateTime.Now;
        SaveMissions();
    }

    public void OnStepsUpdated(int totalSteps)
    {
        UpdateAllMissionProgress();
        RefreshMissionWindows();
    }

    public void UpdateAllMissionProgress()
    {
        if (StepCounter.Instance == null) return;

        int currentSteps = StepCounter.Instance.GetTotalSteps();

        foreach (var mission in allMissions)
        {
            if (!mission.isClaimed)
            {
                mission.UpdateProgress(currentSteps);
            }
        }

        SaveMissions();
    }

    public void CheckMissionExpiration()
    {
        bool needsRefresh = false;

        foreach (var mission in allMissions.ToList())
        {
            if (mission.IsExpired() && !mission.isClaimed)
            {
                if (mission.type == MissionType.Daily && mission.difficultyLevel < maxDailyResets)
                {
                    int newStepsRequired = baseDailySteps + ((mission.difficultyLevel + 1) * dailyStepIncrement);
                    mission.currentSteps = 0;
                    mission.requiredSteps = newStepsRequired;
                    mission.difficultyLevel++;
                    mission.SetTimeBounds();
                    needsRefresh = true;
                }
                else
                {
                    allMissions.Remove(mission);
                    needsRefresh = true;
                }
            }
        }

        if (needsRefresh)
        {
            if (allMissions.Count(m => m.type == MissionType.Daily) < 3)
            {
                GenerateMissions();
            }
            else
            {
                SaveMissions();
                RefreshMissionWindows();
            }
        }
    }

    public bool ShouldResetMissions()
    {
        if (!PlayerPrefs.HasKey(LAST_RESET_KEY))
            return true;

        string lastResetStr = PlayerPrefs.GetString(LAST_RESET_KEY);
        if (DateTime.TryParse(lastResetStr, out lastResetTime))
        {
            return DateTime.Now.Date > lastResetTime.Date;
        }
        return true;
    }

    // -------------------------------------------------------------------
    //       CLAIM ACTUALIZADO - AHORA DA MONEDAS REALES
    // -------------------------------------------------------------------
    public void ClaimMissionReward(Mission mission)
    {
        if (mission == null) return;
        if (!mission.isCompleted || mission.isClaimed) return;

        mission.isClaimed = true;

        int moneyGained = mission.rewardAmount;

        // 🔥 DAR MONEDAS AL JUGADOR
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCoins(moneyGained);
            Debug.Log($"💰 Recompensa de misión: {moneyGained} monedas");
        }
        else
        {
            Debug.LogWarning("CurrencyManager no encontrado para dar recompensa de misión");
        }

        Oncompletemission?.Invoke();
        SaveMissions();
        RefreshMissionWindows();
    }

    public void RefreshMissionWindows()
    {
        if (dailyMissionsWindow != null)
            dailyMissionsWindow.RefreshMissions(GetMissionsByType(MissionType.Daily));

        if (longMissionsWindow != null)
            longMissionsWindow.RefreshMissions(GetMissionsByType(MissionType.Long));

        if (weeklyMissionsWindow != null)
            weeklyMissionsWindow.RefreshMissions(GetMissionsByType(MissionType.Weekly));
    }

    public List<Mission> GetMissionsByType(MissionType type)
    {
        return allMissions.Where(m => m.type == type).ToList();
    }

    public void SaveMissions()
    {
        MissionData data = new MissionData
        {
            missions = allMissions,
            lastResetTime = lastResetTime
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(MISSIONS_KEY, json);
        PlayerPrefs.SetString(LAST_RESET_KEY, lastResetTime.ToString());
        PlayerPrefs.Save();
    }

    public void LoadMissions()
    {
        if (PlayerPrefs.HasKey(MISSIONS_KEY))
        {
            string json = PlayerPrefs.GetString(MISSIONS_KEY);
            MissionData data = JsonUtility.FromJson<MissionData>(json);

            if (data != null)
            {
                allMissions = data.missions;
                lastResetTime = data.lastResetTime;
            }
        }
    }

    public void AddTestSteps(int steps)
    {
        if (StepCounter.Instance != null)
        {
            StepCounter.Instance.AddTestSteps(steps);
        }
    }

    public void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            CheckMissionExpiration();
            UpdateAllMissionProgress();
            RefreshMissionWindows();
        }
    }

    [System.Serializable]
    private class MissionData
    {
        public List<Mission> missions;
        public DateTime lastResetTime;
    }
}