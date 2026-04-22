using System;
using Global;
using TMPro;
using UnityEngine;

namespace _Project.UI
{
    public class LevelInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelName;
        [SerializeField] private TextMeshProUGUI currentProgress;
        [SerializeField] private TextMeshProUGUI currentJumps;
        [SerializeField] private TextMeshProUGUI bestScore;

        private void OnEnable()
        {
            DisplayEmpty();
        }

        public void ShowLevelInfo(LevelData levelData)
        {
            if (levelData == null)
            {
                DisplayEmpty();
                return;
            }

            string complete = levelData.IsFinished ? "Complete" : "In Progress";
            levelName.text = $"{levelData.LevelName} ({complete})";
            currentProgress.text = "Last CheckPoint: " + levelData.LastCheckpoint.Checkpoint;
            currentJumps.text = "Jumps: " + levelData.LastCheckpoint.Jumps;
            bestScore.text = "Best Score: " + levelData.JumpRecord;
        }

        private void DisplayEmpty()
        {
            levelName.SetText("");
            currentProgress.SetText("");
            currentJumps.SetText("");
            bestScore.SetText("");
        }
    }
}