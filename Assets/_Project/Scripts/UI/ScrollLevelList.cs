using System;
using System.Collections.Generic;
using Global;
using UnityEngine;

namespace _Project.UI
{
    public class ScrollLevelList : MonoBehaviour
    {
        [SerializeField] private GameObject levelPrefab;
        [SerializeField] private Transform scrollRectContent;
        
        public LevelPrefab SelectedLevel { get; private set; }
        public event Action OnNewLevelSelected;
        
        private List<LevelPrefab> createdLevels = new();
        private SaveData SaveData => SaveController.Instance.SaveData;

        private void OnEnable()
        {
            LevelPrefab.OnClick += LevelPrefab_OnClick;
            foreach ((SceneName key, LevelData leveldata) in SaveData.LevelDatas)
            {
                var levelPref = Instantiate(levelPrefab, scrollRectContent).GetComponent<LevelPrefab>();
                levelPref.Init(leveldata);
                createdLevels.Add(levelPref);
            }
        }

        private void LevelPrefab_OnClick(LevelPrefab level)
        {
            if (level == SelectedLevel)
                return;
            
            foreach (LevelPrefab createdLevel in createdLevels)
            {
                createdLevel.SetSelected(createdLevel == level);
            }

            SelectedLevel = level;
            OnNewLevelSelected?.Invoke();
        }

        private void OnDisable()
        {
            foreach (LevelPrefab createdLevel in createdLevels)
            {
                Destroy(createdLevel.gameObject);
            }
            createdLevels.Clear();
            SelectedLevel = null;
            LevelPrefab.OnClick -= LevelPrefab_OnClick;
        }
    }
}