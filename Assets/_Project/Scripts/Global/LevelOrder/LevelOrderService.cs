using UnityEngine;

namespace Global
{
    public class LevelOrderService : MonoBehaviour, ILevelOrderService
    {
        public bool IsLevel(SceneName name) => LevelOrder.IsLevel(name);
        public SceneName GetNextLevel(SceneName current) => LevelOrder.GetNextLevel(current);
    }
}