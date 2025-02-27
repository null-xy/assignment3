using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameManager instance;
    private int mobsKilledCount = 0;
    public GameObject finishUI;
    public int killsToWin = 2;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnMobKilled()
    {
        mobsKilledCount++;

        if (mobsKilledCount >= killsToWin)
        {
            FinishGame();
        }
    }

    private void FinishGame()
    {
        if (finishUI != null)
        {
            finishUI.SetActive(true);
        }

        Debug.Log("Congratulations! You have finished the game.");
    }
}
