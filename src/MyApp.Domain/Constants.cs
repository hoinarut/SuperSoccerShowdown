namespace MyApp.Domain;

public static class Constants
{
    public const int NumberOfPlayers = 5;
    
    public static class ErrorMessages
    {
        public const string MaxPlayersReached = "The maximum number of players has been reached.";
        public const string PlayersNotSet = "Players not set";
        public const string DefendersNotSet = "Defenders not set";
        public const string AttackersNotSet = "Attackers not set";
        public const string GoalieNotSet = "Goalie not set";
    }
}