using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class MyService : IMyService
    {
        private readonly IGlobalStateService _globalState;

        public MyService(IGlobalStateService globalState)
        {
            _globalState = globalState;
        }

        public void Run()
        {
            // Ausgabe der Umgebungsvariable DOTNET_ENVIRONMENT
            Console.WriteLine($"DOTNET_ENVIRONMENT: {_globalState.Environment ?? "(nicht gesetzt)"}");
            Console.WriteLine($"Global Debug: {_globalState.Debug}");
        }
    }
}