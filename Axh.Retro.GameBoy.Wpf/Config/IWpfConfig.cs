using System.Collections.Generic;
using System.Windows.Input;
using Axh.Retro.GameBoy.Contracts.Devices;

namespace Axh.Retro.GameBoy.Wpf.Config
{
    /// <summary>
    /// WPF specific config.
    /// </summary>
    public interface IWpfConfig
    {
        /// <summary>
        /// Gets the keyboard map.
        /// </summary>
        /// <value>
        /// The keyboard map.
        /// </value>
        IDictionary<Key, JoyPadButton> KeyboardMap { get; }
    }
}