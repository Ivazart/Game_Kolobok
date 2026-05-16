using System;
using System.Collections.Generic;

namespace Global
{
    public class SaveDataFactory
    {
        public SaveData CreateDefault(ILevelOrderService levelOrderService)
        {
            var saveData = new SaveData
            {
                LevelDatas = new Dictionary<SceneName, LevelData>(),
                LastCheckpointData = new LastCheckpointData {LevelName = SceneName.StartLab}
            };

            foreach (var name in GetAllLevelNames(levelOrderService))
            {
                saveData.LevelDatas.Add(name, CreateEmptyLevelData(name));
            }

            if (saveData.LevelDatas.ContainsKey(SceneName.Rocks))
                saveData.LevelDatas[SceneName.Rocks].IsOpen = true;

            return saveData;
        }
        
        public SaveData EnsureAllLevelsPresent(ILevelOrderService order, SaveData saveData)
        {
            foreach (SceneName name in Enum.GetValues(typeof(SceneName)))
            {
                if (!order.IsLevel(name))
                    continue;
                if (!saveData.LevelDatas.ContainsKey(name))
                {
                    saveData.LevelDatas.Add(name, CreateEmptyLevelData(name));
                }
            }

            return saveData;
        }

        private IEnumerable<SceneName> GetAllLevelNames(ILevelOrderService order)
        {
            foreach (SceneName name in Enum.GetValues(typeof(SceneName)))
            {
                if (order.IsLevel(name))
                    yield return name;
            }
        }

        private LevelData CreateEmptyLevelData(SceneName name)
        {
            var levelData = new LevelData
            {
                LevelName = name,
                JumpRecord = int.MaxValue, // сигнализирует, что рекорда ещё нет
                LastCheckpoint = new LastCheckpointData
                {
                    LevelName = name,
                    Checkpoint = -1,
                    Jumps = 0,
                    Progress = 0
                }
            };
            
            if (levelData.LevelName == SceneName.StartLab)
            {
                levelData.IsFinished = true;
                levelData.IsOpen = true;
            }

            return levelData;
        }
    }
}