using System;
using Global;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.UI
{
    public class LoadStage : MonoBehaviour
    {
        public event Action OnLoadStageFinished;
        
        [SerializeField] private ScrollLevelList levelList;
        [SerializeField] private LevelInfo levelInfo;
        [SerializeField] private Button buttonStart;
        
        private GameController gameController => GameController.Instance;
        private void Awake()
        {
            buttonStart.onClick.AddListener(LoadLevel);
            levelList.OnNewLevelSelected += LevelList_OnNewLevelSelected;
            buttonStart.enabled = false;
        }

        private void LevelList_OnNewLevelSelected()
        {
            buttonStart.enabled = levelList.SelectedLevel != null;
            levelInfo.ShowLevelInfo(levelList.SelectedLevel.LevelData);
        }

        private void LoadLevel()
        {
            var scene = levelList.SelectedLevel.LevelData.LevelName;
            gameController.LoadLevelFromSaves(scene);
        }
    }
}