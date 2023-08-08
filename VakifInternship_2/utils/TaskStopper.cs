using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VakifInternship_2.utils
{
    internal class TaskStopper
    {
        private static CancellationTokenSource tokenSource;
        private static CancellationToken instance;

        private TaskStopper(){}

        public static void Build()
        {
            instance = new CancellationToken();
            tokenSource = new CancellationTokenSource();
        }

        public static CancellationToken GetInstance()
        {
            return instance;
        }

        public static CancellationTokenSource GetTokenSource()
        {
            return tokenSource;
        }

        public static void Stop()
        {
            tokenSource.Cancel();
        }
    }
}
