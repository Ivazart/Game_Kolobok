namespace Global
{
    public static class LevelOrder
    {
        /// <summary>
        /// Список сцен, которые считаются уровнями (в порядке прохождения).
        /// </summary>
        public static readonly SceneName[] Levels = new SceneName[]
        {
            SceneName.StartLab,
            SceneName.Rocks,
            SceneName.Rocks2,
            SceneName.Swamp,
            SceneName.Swamp2,
            SceneName.Mine,
            SceneName.MineFinal
        };

        /// <summary>
        /// Количество уровней.
        /// </summary>
        public static int Count => Levels.Length;

        /// <summary>
        /// Получить уровень по индексу.
        /// </summary>
        public static SceneName GetByIndex(int index) => Levels[index];

        /// <summary>
        /// Получить индекс уровня (или -1, если это не уровень).
        /// </summary>
        public static int GetIndex(SceneName level) => System.Array.IndexOf(Levels, level);

        /// <summary>
        /// Проверить, является ли сцена уровнем.
        /// </summary>
        public static bool IsLevel(SceneName scene) => GetIndex(scene) >= 0;

        /// <summary>
        /// Получить следующий уровень (или текущий, если последний).
        /// </summary>
        public static SceneName GetNextLevel(SceneName current)
        {
            int idx = GetIndex(current);
            if (idx < 0 || idx >= Count - 1)
                return current;
            return Levels[idx + 1];
        }
    }
}