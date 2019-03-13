using System.Collections.Generic;
using Xunit;

namespace UnitTest.wmi
{
    /// <summary>
    /// Just some quick tests to make sure I understand how Queue works
    /// </summary>
    public class QueueShould
    {
        [Fact]
        public void KeepTheSameOrder()
        {
            List<int> nums = new List<int>(new int[] { 1, 1, 2, 3, 5, 8, 13 });
            Queue<int> q = new Queue<int>(nums);

            Assert.Equal(7, q.Count);
            Assert.Equal(1, q.Dequeue());
            Assert.Equal(1, q.Dequeue());
            Assert.Equal(2, q.Dequeue());
            Assert.Equal(3, q.Dequeue());
            Assert.Equal(5, q.Dequeue());
            Assert.Equal(8, q.Dequeue());
            Assert.Equal(13, q.Dequeue());
        }
    }
}
