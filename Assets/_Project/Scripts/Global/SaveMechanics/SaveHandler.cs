using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Global;

namespace Global
{
    public class SaveHandler
    {
        private const string SaveKey = "GameSaveData";

        /// <summary>
        /// Сохраняет объект SaveData в PlayerPrefs.
        /// </summary>
        public void Save(SaveData data)
        {
            if (data == null)
            {
                Debug.LogError("SaveData is null. Save aborted.");
                return;
            }

            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.None);
                PlayerPrefs.SetString(SaveKey, json);
                PlayerPrefs.Save();
                Debug.Log("Game saved successfully.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
            }
        }

        /// <summary>
        /// Загружает SaveData из PlayerPrefs. Если сохранения нет, возвращает новый объект.
        /// </summary>
        public SaveData Load()
        {
            if (!PlayerPrefs.HasKey(SaveKey))
            {
                Debug.Log("No save data found. Returning new SaveData.");
                return SaveDataDefault();
            }

            try
            {
                string json = PlayerPrefs.GetString(SaveKey);
                SaveData data = JsonConvert.DeserializeObject<SaveData>(json);
                Debug.Log("Game loaded successfully.");
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                return SaveDataDefault();
            }
        }

        /// <summary>
        /// Удаляет сохранение из PlayerPrefs.
        /// </summary>
        public void DeleteSave()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
            Debug.Log("Save data deleted.");
        }
        
        private SaveData SaveDataDefault()
        {
            SaveData saveData = new();
            saveData.LevelDatas = new Dictionary<SceneName, LevelData>();
            saveData.LastCheckpointData = new LastCheckpointData();
            saveData.LastCheckpointData.LevelName = SceneName.StartLab;
            var leveldata = CreateLevel(SceneName.Rocks);
            saveData.LevelDatas.Add(SceneName.Rocks, leveldata );
            return saveData;
        }
        public LevelData CreateLevel(SceneName sceneName)
        {
            var levelData = new LevelData();
            levelData. LevelName = sceneName;
            levelData.LastCheckpoint = new LastCheckpointData();
            levelData.LastCheckpoint.LevelName = sceneName;
            
            return levelData;
        }
    }
}