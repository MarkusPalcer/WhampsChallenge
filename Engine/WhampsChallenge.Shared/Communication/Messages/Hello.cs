namespace WhampsChallenge.Shared.Communication.Messages
{
    /// <summary>
    /// The initial message sent to the server by the client
    /// </summary>
    public class Hello
    {
        /// <summary>
        /// All levels this client competes in 
        /// </summary>
        public string[] Levels;

        /// <summary>
        /// The name of the author
        /// </summary>
        public string Author;

        /// <summary>
        /// The display name of the contestant
        /// </summary>
        public string ContestantName;
    }
}
