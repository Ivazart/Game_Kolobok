using System;
using System.Collections.Generic;
using Global;
using UnityEngine;

namespace _Project.Core
{
    public class CheckpointList : MonoBehaviour
    {
        private List<CheckPoint> checkpoints = new ();
        private SaveController saveController => SaveController.Instance;
        private int LastCheckpoint => saveController.LastCheckPointID;

        public Vector3 GetLastCheckPointPosition()
        {
            if (LastCheckpoint == -1)
                return checkpoints[0].transform.position;
            
            if (checkpoints.Count > LastCheckpoint)
                return checkpoints[LastCheckpoint].transform.position;
            
            Debug.LogError("Last checkpoint index error");
            return checkpoints[0].transform.position;
        }
        
        private void Awake()
        {
            checkpoints.AddRange(GetComponentsInChildren<CheckPoint>(true));
            CheckPoint.OnCheckpointEnter += CheckPoints_OnCheckpointEnter;
        }

        private void CheckPoints_OnCheckpointEnter(CheckPoint obj)
        {
            int index = checkpoints.IndexOf(obj);
            if (index == -1)
                Debug.LogError("Checkpoint is not in list");
            else
            {
                if (index > LastCheckpoint)
                    saveController.NewCheckPointReached(index);
            }
        }

        private void OnDestroy()
        {
            CheckPoint.OnCheckpointEnter -= CheckPoints_OnCheckpointEnter;
        }
    }
}