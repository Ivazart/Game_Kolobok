namespace Global
{
    public interface ILevelOrderService
    {
        bool IsLevel(SceneName name);
        SceneName GetNextLevel(SceneName current);
    }
}