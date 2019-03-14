namespace WhampsChallenge.Level2
{
    public enum Perception 
    {
        /// <summary>
        /// You ran against a wall.
        /// Ouch!
        /// </summary>
        Bump,

        /// <summary>
        /// A trap must be on an adjacent square.
        /// Be careful where you go next.
        /// </summary>
        Wind,

        /// <summary>
        /// Something is glittering on the ground.
        /// Maybe you should pick it up?
        /// </summary>
        Glitter,

        /// <summary>
        /// You just won the game
        /// </summary>
        Win, 

        /// <summary>
        /// You just lost the game.
        /// </summary>
        Death
    }
}