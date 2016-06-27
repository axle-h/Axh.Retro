using System;

namespace Axh.Retro.CPU.Common.Contracts.Exceptions
{
    public class PlatformConfigurationException : Exception
    {
        public PlatformConfigurationException(string message) : base(message)
        {
        }
    }
}