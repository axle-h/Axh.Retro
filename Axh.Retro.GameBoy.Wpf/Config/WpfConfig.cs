using System.Collections.Generic;
using System.Windows.Input;
using Axh.Retro.GameBoy.Contracts.Devices;

namespace Axh.Retro.GameBoy.Wpf.Config
{
    /// <summary>
    /// WPF specific config.
    /// </summary>
    public class WpfConfig : IWpfConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WpfConfig"/> class.
        /// </summary>
        public WpfConfig()
        {
            KeyboardMap = new Dictionary<Key, JoyPadButton>
                          {
                              { Key.Space, JoyPadButton.A },
                              { Key.LeftCtrl, JoyPadButton.B },
                              { Key.Return, JoyPadButton.Start },
                              { Key.RightShift, JoyPadButton.Select },
                              { Key.Up, JoyPadButton.Up },
                              { Key.Down, JoyPadButton.Down },
                              { Key.Left, JoyPadButton.Left },
                              { Key.Right, JoyPadButton.Right }
                          };
        }

        /// <summary>
        /// Gets the keyboard map.
        /// </summary>
        /// <value>
        /// The keyboard map.
        /// </value>
        public IDictionary<Key, JoyPadButton> KeyboardMap { get; }
    }
}
