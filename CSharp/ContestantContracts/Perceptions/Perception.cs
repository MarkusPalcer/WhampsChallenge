namespace ContestantContracts.Perceptions
{
    public enum Perception
    {
        /// <summary>
        /// You ran against a wall.
        /// Ouch!
        /// </summary>
        Bump,

        /// <summary>
        /// The Whamps must be on an adjacent square.
        /// Be careful where you go next.
        /// </summary>
        Stench,

        /// <summary>
        /// A trap must be on an adjacent square.
        /// Be careful where you go next.
        /// </summary>
        Wind,

        /// <summary>
        /// You tried to fire an arrow but your quiver is empty.
        /// Be careful with your bowstring.
        /// </summary>
        Twang,

        /// <summary>
        /// You fired an arrow and killed the Whamps.
        /// Way to go!
        /// </summary>
        Scream,
        
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