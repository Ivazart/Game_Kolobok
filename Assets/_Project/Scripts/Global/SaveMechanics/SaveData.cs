using System.Collections.Generic;

namespace Global
{
    public class SaveData
    {
        public Dictionary<SceneName, LevelData> LevelDatas;
        public LastCheckpointData LastCheckpointData;
        public bool IsTutorFinished;
    }

    public class LevelData
    {
        public SceneName LevelName;
        public int JumpRecord;
        public bool IsFinished;
        public bool IsOpen;
        public LastCheckpointData LastCheckpoint;
    }

    public class LastCheckpointData
    { 
        public SceneName LevelName;
        public int Checkpoint;
        public int Jumps;
        public int Progress;

        public LastCheckpointData()
        {
            Checkpoint = -1;
        }

        public LastCheckpointData(LastCheckpointData newData)
        {
            LevelName = newData.LevelName;
            Checkpoint = newData.Checkpoint;
            Jumps = newData.Jumps;
            Progress = newData.Progress;
        }
    }
}