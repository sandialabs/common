using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.utilities
{
    public class CancellationTokenShould
    {
        /// <summary>
        /// Doesn't test our code, but I wanted to make sure that cancellation tokens
        /// properly stop a task.
        /// </summary>
        [Fact]
        public void StopATaskProperly()
        {
            bool completed = false;
            CancellationTokenSource source = new CancellationTokenSource();

            Task task = new Task(() =>
            {
                Thread.Sleep(1000);
                completed = true;
            });

            task.Start();
            source.CancelAfter(100);
            Assert.Throws<OperationCanceledException>(() => task.Wait(source.Token));
            Assert.False(completed);

            source.Dispose();
        }
    }
}
