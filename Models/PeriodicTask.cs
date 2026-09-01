using System;
using System.Threading;
using System.Threading.Tasks;

namespace MGSPW_MC_Cheat_Trainer.Models;

public static class PeriodicTask
{
        public static async Task Run(Action action, TimeSpan period, CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(period, cancellationToken);

                if(!cancellationToken.IsCancellationRequested)
                {
                    action();
                }
            }
        }

        public static Task Run(Action action, TimeSpan period)
        {
            return Run(action, period, CancellationToken.None);
        }
}