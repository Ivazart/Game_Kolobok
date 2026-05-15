/*using Global;
using NUnit.Framework;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

public class SaveHandlerTests
{
    private SaveHandler saveHandler;

    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll(); // чистим всё
        saveHandler = new SaveHandler();
    }

    [Test]
    public void Load_NoSave_ReturnsDefaultData()
    {
        var data = saveHandler.Load();
        Assert.IsNotNull(data);
        Assert.IsTrue(data.LevelDatas.ContainsKey(SceneName.StartLab));
        Assert.AreEqual(SceneName.StartLab, data.LastCheckpointData.LevelName);
        Assert.IsTrue(data.LevelDatas[SceneName.Rocks].IsOpen);
        Assert.IsFalse(data.IsTutorFinished);
    }

    [Test]
    public void SaveAndLoad_PreservesData()
    {
        var data = saveHandler.Load();
        data.LastCheckpointData.Checkpoint = 5;
        data.LastCheckpointData.Jumps = 10;
        saveHandler.Save(data);

        var loaded = saveHandler.Load();
        Assert.AreEqual(5, loaded.LastCheckpointData.Checkpoint);
        Assert.AreEqual(10, loaded.LastCheckpointData.Jumps);
    }

    [Test]
    public void DeleteSave_RemovesData()
    {
        var data = saveHandler.Load();
        data.IsTutorFinished = true;
        saveHandler.Save(data);
        saveHandler.DeleteSave();

        var after = saveHandler.Load();
        // Должны быть значения по умолчанию
        Assert.IsFalse(after.IsTutorFinished);
        Assert.AreEqual(SceneName.StartLab, after.LastCheckpointData.LevelName);
    }

    [Test]
    public void Load_HandlesMissingLevels()
    {
        // Допустим, в сохранении нет данных об уровне, который уже есть в SceneName.
        var incompleteData = new SaveData
        {
            LevelDatas = new System.Collections.Generic.Dictionary<SceneName, LevelData>
            {
                { SceneName.StartLab, new LevelData { LevelName = SceneName.StartLab, IsOpen = true } }
            },
            LastCheckpointData = new LastCheckpointData { LevelName = SceneName.StartLab }
        };
        string json = JsonConvert.SerializeObject(incompleteData);
        PlayerPrefs.SetString("GameSaveData", json);
        PlayerPrefs.Save();

        var loaded = saveHandler.Load();
        // Должен добавить недостающие уровни
        Assert.IsTrue(loaded.LevelDatas.ContainsKey(SceneName.Rocks));
    }
}*/