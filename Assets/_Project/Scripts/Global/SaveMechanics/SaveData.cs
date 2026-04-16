using System.Collections.Generic;

namespace Global
{
    public class SaveData
    {
        public Dictionary<SceneName, LevelData> LevelDatas;
        public LastCheckpointData LastCheckpointData;
    }

    public class LevelData
    {
        public SceneName LevelName;
        public int JumpRecord;
        public bool IsFinished;
        public LastCheckpointData LastCheckpoint;
    }

    public class LastCheckpointData
    { 
        public SceneName LevelName;
        public int Checkpoint;
        public int Jumps;

        public LastCheckpointData(){}

        public LastCheckpointData(LastCheckpointData newData)
        {
            LevelName = newData.LevelName;
            Checkpoint = newData.Checkpoint;
            Jumps = newData.Jumps;
        }
    }
}