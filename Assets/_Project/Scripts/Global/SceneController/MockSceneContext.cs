namespace Global
{
    public class MockSceneContext : ISceneContext
    {
        public SceneName CurrentScene { get; set; } = SceneName.StartLab;
        public void LoadScene(SceneName sceneName) => CurrentScene = sceneName;
    }
}