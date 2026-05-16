using System.Collections.Generic;
using Global;
using NUnit.Framework;
using UnityEngine;

public class SaveHandlerTests
{
    private SaveHandler saveHandler;

    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();
        saveHandler = new SaveHandler();
    }

    [Test]
    public void Load_NoSave_ReturnsNull()
    {
        Assert.IsNull(saveHandler.Load());
    }

    [Test]
    public void SaveAndLoad_ReturnsSameData()
    {
        var data = new SaveData
        {
            LevelDatas = new Dictionary<SceneName, LevelData>(),
            LastCheckpointData = new LastCheckpointData { LevelName = SceneName.Rocks, Checkpoint = 2, Jumps = 7 }
        };

        saveHandler.Save(data);
        var loaded = saveHandler.Load();

        Assert.NotNull(loaded);
        Assert.AreEqual(SceneName.Rocks, loaded.LastCheckpointData.LevelName);
        Assert.AreEqual(2, loaded.LastCheckpointData.Checkpoint);
        Assert.AreEqual(7, loaded.LastCheckpointData.Jumps);
    }

    [Test]
    public void DeleteSave_RemovesData()
    {
        saveHandler.Save(new SaveData());
        Assert.NotNull(saveHandler.Load());

        saveHandler.DeleteSave();
        Assert.IsNull(saveHandler.Load());
    }
}