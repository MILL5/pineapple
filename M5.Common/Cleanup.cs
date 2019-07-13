using System;
using static Pineapple.Common.Preconditions;
// ReSharper disable UnusedMember.Global

namespace Pineapple.Common
{
    public static class Cleanup
    {
        public interface ISafeContinue
        {
            ISafeContinue Continue(Action a);
            ISafeContinue OnSuccess(Action a);
            ISafeContinue OnFailure(Action a);
        }

        private class SafeContinue : ISafeContinue
        {
            private readonly bool _success;

            public SafeContinue(bool success)
            {
                _success = success;
            }

            public ISafeContinue Continue(Action a)
            {
                CheckIsNotNull(nameof(a), a);

                SafeMethod(a);

                return new SafeContinue(true);
            }

            public ISafeContinue OnSuccess(Action a)
            {
                CheckIsNotNull(nameof(a), a);

                if (_success)
                {
                    return SafeMethod(a);
                }

                return new SafeContinue(false);
            }

            public ISafeContinue OnFailure(Action a)
            {
                CheckIsNotNull(nameof(a), a);

                if (!_success)
                {
                    return SafeMethod(a);
                }

                return new SafeContinue(true);
            }
        }

        public static ISafeContinue SafeMethod(Action a)
        {
            bool success = false;

            CheckIsNotNull(nameof(a), a);

            try
            {
                a();
                success = true;
            }
            catch
            {
                // ignored
            }

            return new SafeContinue(success);
        }
    }
}