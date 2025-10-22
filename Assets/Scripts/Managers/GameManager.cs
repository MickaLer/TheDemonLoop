namespace Managers
{
    public static class GameManager
    {
        private static BossBehaviour _currentBoss;
        public static BossBehaviour CurrentBoss => _currentBoss;

        public static void StartFight(BossBehaviour currentBoss)
        {
            //Stock currentBoss static reference for pattern use
            _currentBoss = currentBoss;
        }
    }
}