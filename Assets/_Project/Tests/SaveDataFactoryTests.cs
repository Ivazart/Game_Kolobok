using Global;
using NUnit.Framework;
using UnityEngine;

public class SaveDataFactoryTests
{
    [Test]
    public void CreateDefault_AllLevelsExistAndFirstOpen()
    {
        SaveDataFactory saveDataFactory = new SaveDataFactory();
        var order = new MockLevelOrderService();
        var data = saveDataFactory.CreateDefault(order);

        Assert.IsTrue(data.LevelDatas.ContainsKey(SceneName.Rocks));
        Assert.IsTrue(data.LevelDatas.ContainsKey(SceneName.Swamp));

        Assert.IsTrue(data.LevelDatas[SceneName.Rocks].IsOpen);
        Assert.IsFalse(data.LevelDatas[SceneName.Swamp].IsOpen);

        Assert.AreEqual(SceneName.StartLab, data.LastCheckpointData.LevelName);
        Assert.AreEqual(-1, data.LastCheckpointData.Checkpoint);
        Assert.AreEqual(0, data.LastCheckpointData.Jumps);
        Assert.AreEqual(int.MaxValue, data.LevelDatas[SceneName.Rocks].JumpRecord);
    }
}