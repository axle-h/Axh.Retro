using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Axh.Retro.GameBoy.Contracts.Devices;
using Axh.Retro.GameBoy.Wpf.Config;

namespace Axh.Retro.GameBoy.Wpf
{
    public class KeyboardJoyPadBehavior : Behavior<Window>
    {
        private readonly IJoyPad _joyPad;
        private readonly IWpfConfig _wpfConfig;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardJoyPadBehavior"/> class.
        /// </summary>
        /// <param name="joyPad">The joy pad.</param>
        /// <param name="wpfConfig">The WPF configuration.</param>
        public KeyboardJoyPadBehavior(IJoyPad joyPad, IWpfConfig wpfConfig)
        {
            _joyPad = joyPad;
            _wpfConfig = wpfConfig;
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            AssociatedObject.PreviewKeyDown += ProcessKeyDown;
            AssociatedObject.PreviewKeyUp += ProcessKeyUp;
            base.OnAttached();
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            AssociatedObject.PreviewKeyDown -= ProcessKeyDown;
            AssociatedObject.PreviewKeyUp -= ProcessKeyUp;
            base.OnDetaching();
        }

        private void ProcessKeyUp(object sender, KeyEventArgs args)
        {
            if (!_wpfConfig.KeyboardMap.ContainsKey(args.Key))
            {
                return;
            }

            var button = _wpfConfig.KeyboardMap[args.Key];
            _joyPad.Buttons &= ~button;

            args.Handled = true;
        }

        private void ProcessKeyDown(object sender, KeyEventArgs args)
        {
            if (!_wpfConfig.KeyboardMap.ContainsKey(args.Key))
            {
                return;
            }

            var button = _wpfConfig.KeyboardMap[args.Key];
            _joyPad.Buttons |= button;

            args.Handled = true;
        }
    }
}
