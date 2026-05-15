using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Global;

namespace Global
{
    public class SaveHandler
    {
        private const string SaveKey = "GameSaveData";

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
            catch (Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
            }
        }

        /// <summary> Возвращает сохранённые данные или null, если сохранения нет / ошибка. </summary>
        public SaveData Load()
        {
            if (!PlayerPrefs.HasKey(SaveKey))
            {
                Debug.Log("No save data found. Returning null.");
                return null;
            }

            try
            {
                string json = PlayerPrefs.GetString(SaveKey);
                SaveData data = JsonConvert.DeserializeObject<SaveData>(json);
                Debug.Log("Game loaded successfully.");
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                return null;
            }
        }

        public void DeleteSave()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
            Debug.Log("Save data deleted.");
        }
    }
}