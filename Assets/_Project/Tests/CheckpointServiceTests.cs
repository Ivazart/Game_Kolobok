using Global;
using NUnit.Framework;
using UnityEngine;

public class CheckpointServiceTests
{
    private SaveData saveData;
    private SaveHandler saveHandler;
    private MockSceneContext sceneContext;
    private MockLevelOrderService levelOrder;
    private CheckpointService service;
    private int lastJumpEventValue;
    private bool newCheckpointFired;
    private SaveDataFactory saveDataFactory;
    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();
        saveHandler = new SaveHandler();
        sceneContext = new MockSceneContext { CurrentScene = SceneName.Rocks };
        levelOrder = new MockLevelOrderService();
        saveDataFactory = new SaveDataFactory();
        saveData = saveDataFactory.CreateDefault(levelOrder); // чистые данные

        service = new CheckpointService(saveData, saveHandler, sceneContext, levelOrder);
        lastJumpEventValue = -1;
        newCheckpointFired = false;

        service.OnSavedJumpsChanged += (v) => lastJumpEventValue = v;
        service.OnNewCheckpointReached += () => newCheckpointFired = true;
    }

    [Test]
    public void NewCheckPointReached_UpdatesDataAndFiresEvent()
    {
        service.NewCheckPointReached(3);

        Assert.AreEqual(3, service.LastCheckPointID);
        Assert.AreEqual(3, saveData.LastCheckpointData.Checkpoint);
        Assert.AreEqual(SceneName.Rocks, saveData.LastCheckpointData.LevelName);
        Assert.IsTrue(newCheckpointFired);

        // Должно было сохраниться в PlayerPrefs
        var loaded = saveHandler.Load();
        Assert.AreEqual(3, loaded.LastCheckpointData.Checkpoint);
    }

    [Test]
    public void NewCheckPointReached_OnNonLevelScene_DoesNothing()
    {
        // Установим сцену, которая не уровень (например, MainMenu)
        sceneContext.CurrentScene = SceneName.StartLab; // предположим есть такое значение
        // В моке IsLevel для MainMenu вернёт false
        service.NewCheckPointReached(5);

        Assert.AreEqual(-1, service.LastCheckPointID); // не изменился
        Assert.IsFalse(newCheckpointFired);
    }

    [Test]
    public void SaveJumpCounter_UpdatesGlobalAndLevelData()
    {
        service.SaveJumpCounter(10);

        Assert.AreEqual(10, saveData.LastCheckpointData.Jumps);
        Assert.AreEqual(10, saveData.LevelDatas[SceneName.Rocks].LastCheckpoint.Jumps);
        Assert.AreEqual(10, lastJumpEventValue);

        var loaded = saveHandler.Load();
        Assert.AreEqual(10, loaded.LastCheckpointData.Jumps);
    }

    [Test]
    public void ClearLevelProgress_ResetsToStart()
    {
        // Сначала зафиксируем чекпоинт
        service.NewCheckPointReached(2);
        service.SaveJumpCounter(8);

        // Сброс до начала уровня
        service.ClearLevelProgress();

        Assert.AreEqual(-1, service.LastCheckPointID);
        Assert.AreEqual(-1, saveData.LastCheckpointData.Checkpoint);
        Assert.AreEqual(0, saveData.LastCheckpointData.Jumps);
        Assert.AreEqual(0, lastJumpEventValue); // событие вызвано с нулём

        // Проверим, что данные уровня тоже сбросились
        Assert.AreEqual(-1, saveData.LevelDatas[SceneName.Rocks].LastCheckpoint.Checkpoint);
        Assert.AreEqual(0, saveData.LevelDatas[SceneName.Rocks].LastCheckpoint.Jumps);
    }
}