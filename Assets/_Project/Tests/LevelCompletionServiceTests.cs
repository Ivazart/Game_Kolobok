using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Global;

public class LevelCompletionServiceTests
{
    private SaveData saveData;
    private SaveHandler saveHandler;
    private MockSceneContext sceneContext;
    private MockLevelOrderService levelOrder;
    private CheckpointService checkpointService;
    private LevelCompletionService completionService;
    private bool levelFinishedEventFired;
    private SaveDataFactory saveDataFactory;
    
    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();
        saveHandler = new SaveHandler();
        sceneContext = new MockSceneContext { CurrentScene = SceneName.Rocks };
        levelOrder = new MockLevelOrderService();
        saveDataFactory = new SaveDataFactory();
        saveData = saveDataFactory.CreateDefault(levelOrder);

        checkpointService = new CheckpointService(saveData, saveHandler, sceneContext, levelOrder);
        completionService = new LevelCompletionService(saveData, saveHandler, sceneContext, levelOrder, checkpointService);

        levelFinishedEventFired = false;
        completionService.OnLevelFinished += () => levelFinishedEventFired = true;
    }

    [Test]
    public void LevelCompleted_FirstTime_SetsRecordAndOpensNext()
    {
        // Симулируем прохождение с 5 прыжками
        checkpointService.SaveJumpCounter(5);
        completionService.LevelCompleted();

        var rocks = saveData.LevelDatas[SceneName.Rocks];
        Assert.IsTrue(rocks.IsFinished);
        Assert.AreEqual(5, rocks.JumpRecord); // первый рекорд
        Assert.AreEqual(-1, rocks.LastCheckpoint.Checkpoint);
        Assert.AreEqual(0, rocks.LastCheckpoint.Jumps);

        var swamp = saveData.LevelDatas[SceneName.Swamp];
        Assert.IsTrue(swamp.IsOpen);
        Assert.AreEqual(SceneName.Swamp, saveData.LastCheckpointData.LevelName);
        Assert.AreEqual(SceneName.Swamp, sceneContext.CurrentScene); // сцена переключена

        Assert.AreEqual(-1, checkpointService.LastCheckPointID);
        Assert.IsTrue(levelFinishedEventFired);
    }

    [Test]
    public void LevelCompleted_SecondTime_RecordUpdatesIfBetter()
    {
        // Первое прохождение: 10 прыжков
        checkpointService.SaveJumpCounter(10);
        completionService.LevelCompleted();

        // Вернёмся обратно на Rocks для переигрывания (эмулируем)
        sceneContext.CurrentScene = SceneName.Rocks;
        // Сбросим флаг завершённости и чекпоинт для нового прохождения (как при перезапуске)
        saveData.LevelDatas[SceneName.Rocks].IsFinished = false;
        saveData.LastCheckpointData = saveData.LevelDatas[SceneName.Rocks].LastCheckpoint;
        checkpointService.NewCheckPointReached(-1); // имитация начала уровня
        checkpointService.SaveJumpCounter(4);

        completionService.LevelCompleted();

        // Рекорд должен обновиться на 4 (лучше)
        Assert.AreEqual(4, saveData.LevelDatas[SceneName.Rocks].JumpRecord);
    }

    [Test]
    public void LevelCompleted_LastLevel_DoesNotOpenNew()
    {
        // Переключаемся на последний уровень Swamp
        sceneContext.CurrentScene = SceneName.Swamp;
        // Делаем вид, что Swamp открыт и является уровнем
        saveData.LevelDatas[SceneName.Swamp].IsOpen = true;
        saveData.LastCheckpointData = saveData.LevelDatas[SceneName.Swamp].LastCheckpoint;

        checkpointService.SaveJumpCounter(7);
        completionService.LevelCompleted();

        // Swamp завершён, но новый уровень не открывается (nextLevel == scene)
        Assert.IsTrue(saveData.LevelDatas[SceneName.Swamp].IsFinished);
        // Проверим, что не добавился лишний уровень и не сменилась сцена (должна остаться та же или загружена та же)
        Assert.AreEqual(SceneName.Swamp, sceneContext.CurrentScene);
        Assert.IsTrue(levelFinishedEventFired);
    }

    [Test]
    public void LevelCompleted_LevelNotInData_LogsError()
    {
        // Удалим данные текущего уровня из saveData
        saveData.LevelDatas.Remove(SceneName.Rocks);
        // Заменим обработку лога (в тесте просто не падаем)
        LogAssert.Expect(LogType.Error, "Level completed without any level data");
        completionService.LevelCompleted();
        // Событие не должно вызываться
        Assert.IsFalse(levelFinishedEventFired);
    }
}