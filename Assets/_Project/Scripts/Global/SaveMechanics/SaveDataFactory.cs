using System;
using System.Collections.Generic;

namespace Global
{
    public static class SaveDataFactory
    {
        public static SaveData CreateDefault(ILevelOrderService levelOrderService)
        {
            var saveData = new SaveData
            {
                LevelDatas = new Dictionary<SceneName, LevelData>(),
                LastCheckpointData = new LastCheckpointData { LevelName = SceneName.StartLab },
            };

            foreach (SceneName name in Enum.GetValues(typeof(SceneName)))
            {
                if (!levelOrderService.IsLevel(name))
                    continue;

                var levelData = new LevelData
                {
                    LevelName = name,
                    LastCheckpoint = new LastCheckpointData { LevelName = name },
                    JumpRecord = int.MaxValue
                };
                saveData.LevelDatas.Add(name, levelData);
            }

            // Первый уровень открыт
            if (saveData.LevelDatas.ContainsKey(SceneName.Rocks))
                saveData.LevelDatas[SceneName.Rocks].IsOpen = true;

            return saveData;
        }
    }
}