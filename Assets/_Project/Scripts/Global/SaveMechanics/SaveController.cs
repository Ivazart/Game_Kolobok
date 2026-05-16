using System;
using _Project.Scriptable;
using UnityEngine;

namespace Global
{
    [RequireComponent(typeof(LevelOrderService))]
    public class SaveController : SingletonBase<SaveController>
    {
        [SerializeField] private SceneImageDatabase sceneImageDatabase;

        public SaveData SaveData => saveData;
        public int LastCheckPointID => checkpointService.LastCheckPointID;

        public event Action<int> OnSavedJumpsChanged;
        public event Action OnNewCheckpointReached;
        public event Action OnLevelFinished;
        public event Action OnTutorFinished;

        internal void SetSceneContext(ISceneContext ctx) => sceneContext = ctx;
        internal void SetLevelOrderService(ILevelOrderService svc) => levelOrderService = svc;

        private SaveData saveData;
        private SaveHandler saveHandler;
        private ISceneContext sceneContext;
        private ILevelOrderService levelOrderService;
        private bool isInitialized = false;

        private CheckpointService checkpointService;
        private LevelCompletionService levelCompletionService;
        private SaveDataFactory saveDataFactory;
        
        private void Start()
        {
            if (isInitialized)
                return;

            sceneContext ??= SceneController.Instance;
            levelOrderService ??= GetComponent<ILevelOrderService>();

            if (sceneContext == null || levelOrderService == null)
            {
                Debug.LogError("SaveController: ISceneContext or ILevelOrderService not found on this GameObject!");
                return;
            }

            Initialize(sceneContext, levelOrderService, new SaveDataFactory());
        }

  
        public void Initialize(ISceneContext ctx, ILevelOrderService order, SaveDataFactory factory)
        {
            if (isInitialized) return;
            sceneContext = ctx;
            levelOrderService = order;
            saveHandler = new SaveHandler();
            saveDataFactory = factory; 
            saveData = saveHandler.Load() ?? saveDataFactory.CreateDefault(order);
            saveData = saveDataFactory.EnsureAllLevelsPresent(levelOrderService, saveData);
            checkpointService = new CheckpointService(saveData, saveHandler, sceneContext, levelOrderService);
            levelCompletionService = new LevelCompletionService(saveData, saveHandler, sceneContext, levelOrderService,
                checkpointService);
            
            checkpointService.OnSavedJumpsChanged += (v) => OnSavedJumpsChanged?.Invoke(v);
            checkpointService.OnNewCheckpointReached += () => OnNewCheckpointReached?.Invoke();
            levelCompletionService.OnLevelFinished += () => OnLevelFinished?.Invoke();
            levelCompletionService.OnSavedJumpsChanged += (v) => OnSavedJumpsChanged?.Invoke(v);
            
            LoadLastSave();
            isInitialized = true;
        }
        
        public void NewCheckPointReached(int index) => checkpointService.NewCheckPointReached(index);
        public void SaveJumpCounter(int value) => checkpointService.SaveJumpCounter(value);
        public void ClearLevelProgress() => checkpointService.ClearLevelProgress();
        public void LevelCompleted() => levelCompletionService.LevelCompleted();

        public void TutorFinished()
        {
            if (saveData.IsTutorFinished) return;
            saveData.IsTutorFinished = true;
            saveHandler.Save(saveData);
            OnTutorFinished?.Invoke();
        }

        public void DeleteSave()
        {
            saveHandler.DeleteSave();
            saveData = saveDataFactory.CreateDefault(levelOrderService);
            sceneContext.LoadScene(saveData.LastCheckpointData.LevelName);
            Reinitialize();
        }
        
        public Sprite GetSpriteByScene(SceneName sceneType)
        {
            return !saveData.LevelDatas[sceneType].IsOpen && levelOrderService.IsLevel(sceneType) ? sceneImageDatabase.GetCloseSceneImage() : sceneImageDatabase.GetSpriteByScene(sceneType);
        }

        private void Reinitialize()
        {
            isInitialized = false; // разрешаем повторную инициализацию
            Initialize(sceneContext, levelOrderService, saveDataFactory);
        }
        
        private void LoadLastSave()
        {
            var scene = saveData.LastCheckpointData.LevelName;
            checkpointService.LastCheckPointID = saveData.LastCheckpointData.Checkpoint;
            Debug.Log($"Scene loaded {scene}, checkpoint {LastCheckPointID}");
            if (scene != sceneContext.CurrentScene)
                sceneContext.LoadScene(scene);
            OnSavedJumpsChanged?.Invoke(saveData.LastCheckpointData.Jumps);
        }
    }
}