using System;
using System.Collections.Generic;
using System.Text;

using static Pineapple.Common.Preconditions;

namespace Pineapple.Diagnostics
{
    public class ConsoleColorScope : IDisposable
    {
        private ConsoleColor _savedColor;

        public ConsoleColorScope(ConsoleColor consoleColor)
        {
            CheckIsNotNull(nameof(consoleColor), consoleColor);

            _savedColor = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
        }

        public void Dispose()
        {
            Console.ForegroundColor = _savedColor;
        }
    }
}
