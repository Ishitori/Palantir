namespace Ix.Palantir.Exceptions
{
    using System;
    using System.Threading;

    public static class ExceptionHelper
    {
        public static bool IsFatalException(Exception exc)
        {
            if (exc is ThreadAbortException ||
                exc is AppDomainUnloadedException ||
                exc is OutOfMemoryException)
            {
                return true;
            }

            return false;
        }
    }
}
