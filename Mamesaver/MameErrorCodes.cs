namespace Mamesaver
{
    /// <summary>
    ///     Error codes returned from MAME
    /// </summary>
    public class MameErrorCodes
    {
        /// <summary>
        ///     Known ROM but missing files 
        /// </summary>
        public const int RequireFilesMissing = 2;

        /// <summary>
        ///     Unknown ROM
        /// </summary>
        public const int UnknownSystem = 5;
    }
}