using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDifficultyManager : MonoBehaviour
{
    GameDifficulty[] difficultySettings;
    public GameDifficulty selectedDifficulty;
    private GameDifficulty previousDifficulty;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedDifficulty != previousDifficulty)
        {
            selectedDifficulty.ApplyDifficulty();
            previousDifficulty = selectedDifficulty;
        }
    }

    public void ChangeDifficulty(GameDifficulty difficulty)
    {
        selectedDifficulty = difficulty;
        selectedDifficulty.ApplyDifficulty();
    }
}
