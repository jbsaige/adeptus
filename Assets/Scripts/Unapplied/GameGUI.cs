using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour 
{
    public Transform experienceBar;
    public TextMesh playerHealthText;
    public TextMesh levelText;
    public TextMesh enemyHealthText;
    public Transform enemyHealthBar;

    public void SetPlayerExperience(float percentToLevel, int playerLevel)
    {
        levelText.text = "Level: " + playerLevel;
        experienceBar.localScale = new Vector3(percentToLevel, 1, 1);
    }

    public void SetPlayerHealth(float percentHealth, float currentHealth)
    {
        playerHealthText.text = "Health:  " + currentHealth;
        experienceBar.localScale = new Vector3(percentHealth, 1, 1);
    }

    public void SetEnemyHealth(float percentHealth, float currentHealth)
    {
        enemyHealthText.text = "Health:  " + currentHealth;
        enemyHealthBar.localScale = new Vector3(percentHealth, 1, 1);
    }
}
