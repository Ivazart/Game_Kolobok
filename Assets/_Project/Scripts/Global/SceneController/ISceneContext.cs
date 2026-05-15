using Global;

namespace Global
{
    public interface ISceneContext
    {
        SceneName CurrentScene { get; }
        void LoadScene(SceneName sceneName);
    }
}



