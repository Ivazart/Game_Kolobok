using System;
using UnityEngine;

namespace _Project.Core
{
    public class PlayerSpawner:MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private CheckpointList checkpointList;
        [SerializeField] private CameraMove cameraMove;
        public GameObject Player { get; private set; }
        
        private void Awake()
        {
            Player = Instantiate(playerPrefab);
        }

        public void MoveToLastPoint()
        {
           var checkPointPosition =  checkpointList.GetLastCheckPointPosition();
           Player.transform.position = checkPointPosition;
           cameraMove.InstantMove();
        }
    }
}