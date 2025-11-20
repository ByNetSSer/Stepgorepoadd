using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int level = 1;
    public int baseDamage = 1;

    public int coins;
    public int exp;

    private int expToNext => level * 15; // RECOMPENSA ESCALA

    public int GetDamage()
    {
        return baseDamage * level;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log("Coins = " + coins);
    }

    public void AddExp(int amount)
    {
        exp += amount;
        Debug.Log("EXP = " + exp + "/" + expToNext);

        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        while (exp >= expToNext)
        {
            exp -= expToNext;
            level++;
            Debug.Log("SUBISTE A NIVEL " + level);
        }
    }
}
