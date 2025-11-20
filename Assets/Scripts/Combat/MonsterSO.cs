using UnityEngine;

[CreateAssetMenu(fileName = "NewMonster", menuName = "Combat/Monster")]
public class MonsterSO : ScriptableObject
{
    public string monsterName;
    public int monsterID;
    public Sprite icon;
    public int maxHealth;
    public int resistance;
    public int rewardCoins;
    public int rewardExp;

    public float timeLimit; // segundos para vencerlo
}
