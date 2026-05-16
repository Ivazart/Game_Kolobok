using System.Collections.Generic;
using Global;

public class MockSceneContext : ISceneContext
{
    public SceneName CurrentScene { get; set; } = SceneName.Rocks;
    public void LoadScene(SceneName sceneName) => CurrentScene = sceneName;
}

public class MockLevelOrderService : ILevelOrderService
{
    // Порядок уровней: Rocks -> Swamp
    private static readonly SceneName[] Levels = { SceneName.Rocks, SceneName.Swamp };

    public bool IsLevel(SceneName name) => System.Array.IndexOf(Levels, name) >= 0;

    public SceneName GetNextLevel(SceneName current)
    {
        int idx = System.Array.IndexOf(Levels, current);
        if (idx < 0 || idx >= Levels.Length - 1)
            return current;
        return Levels[idx + 1];
    }
}