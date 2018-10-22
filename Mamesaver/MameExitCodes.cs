namespace Mamesaver
{
    /// <summary>
    ///     Exit codes returned from MAME.
    /// </summary>
    public class MameExitCodes
    {
        public const int SuccessfulExit = 0;

        /// <summary>
        ///     Known ROM but missing files 
        /// </summary>
        public const int RequiredFilesMissing = 2;

        /// <summary>
        ///     Unknown ROM
        /// </summary>
        public const int UnknownSystem = 5;

        /// <summary>
        ///     Invalid or malformed arguments
        /// </summary>
        public const int InvalidArguments = 6;

        /// <summary>
        ///     Maps an exit code to a description.
        /// </summary>
        public static string MapCode(int exitCode)
        {
            switch (exitCode)
            {
                case RequiredFilesMissing:
                    return "Required files missing";
                case UnknownSystem:
                    return "Unknown system";
                case InvalidArguments:
                    return "Invalid arguments";
                default:
                    return "Unknown";
            }
        }
    }
}