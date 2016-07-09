namespace Axh.Retro.CPU.Z80.Contracts.Config
{
    /// <summary>
    /// The runtime configuration.
    /// </summary>
    public interface IRuntimeConfig
    {
        /// <summary>
        /// Gets a value indicating whether [debug mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [debug mode]; otherwise, <c>false</c>.
        /// </value>
        bool DebugMode { get; }

        /// <summary>
        /// Gets the core mode.
        /// </summary>
        /// <value>
        /// The core mode.
        /// </value>
        CoreMode CoreMode { get; }
    }
}