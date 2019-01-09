namespace WhampsChallenge.Contracts
{
    public class Result
    {
        public Perception[] Perceptions { get; set; }

        public IGameState GameState { get; set; }
    }
}