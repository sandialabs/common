using gov.sandia.sld.common.utilities;
using System.Collections.Generic;
using Xunit;

namespace UnitTest
{
    public class ChunkByShould
    {
        [Fact]
        public void ChunkInto5ListsOf1()
        {
            List<List<int>> chunked = m_test.ChunkBy(1);
            Assert.Equal(5, chunked.Count);
            Assert.Single(chunked[0]);
            Assert.Single(chunked[1]);
            Assert.Single(chunked[2]);
            Assert.Single(chunked[3]);
            Assert.Single(chunked[4]);
            Assert.Equal(1, chunked[0][0]);
            Assert.Equal(2, chunked[1][0]);
            Assert.Equal(3, chunked[2][0]);
            Assert.Equal(4, chunked[3][0]);
            Assert.Equal(5, chunked[4][0]);
        }

        [Fact]
        public void ChunkTheSameAs1When0()
        {
            List<List<int>> chunked = m_test.ChunkBy(0);
            Assert.Equal(5, chunked.Count);
            Assert.Single(chunked[0]);
            Assert.Single(chunked[1]);
            Assert.Single(chunked[2]);
            Assert.Single(chunked[3]);
            Assert.Single(chunked[4]);
            Assert.Equal(1, chunked[0][0]);
            Assert.Equal(2, chunked[1][0]);
            Assert.Equal(3, chunked[2][0]);
            Assert.Equal(4, chunked[3][0]);
            Assert.Equal(5, chunked[4][0]);
        }

        [Fact]
        public void ChunkInto3ListsOf2()
        {
            List<List<int>> chunked = m_test.ChunkBy(2);
            Assert.Equal(3, chunked.Count);
            Assert.Equal(2, chunked[0].Count);
            Assert.Equal(2, chunked[1].Count);
            Assert.Single(chunked[2]);
            Assert.Equal(1, chunked[0][0]);
            Assert.Equal(2, chunked[0][1]);
            Assert.Equal(3, chunked[1][0]);
            Assert.Equal(4, chunked[1][1]);
            Assert.Equal(5, chunked[2][0]);
        }

        [Fact]
        public void ChunkIntoASingleListWhenChunkByIsSameSizeAsList()
        {
            List<List<int>> chunked = m_test.ChunkBy(5);
            Assert.Single(chunked);
            Assert.Equal(m_test, chunked[0]);
        }

        [Fact]
        public void ChunkIntoASingleListWhenChunkByIsLargerThanList()
        {
            List<List<int>> chunked = m_test.ChunkBy(6);
            Assert.Single(chunked);
            Assert.Equal(m_test, chunked[0]);
        }

        [Fact]
        public void ChunkTheSameAsWhenWhenNegative()
        {
            List<List<int>> chunked = m_test.ChunkBy(-1);
            Assert.Equal(5, chunked.Count);
            Assert.Single(chunked[0]);
            Assert.Single(chunked[1]);
            Assert.Single(chunked[2]);
            Assert.Single(chunked[3]);
            Assert.Single(chunked[4]);
            Assert.Equal(1, chunked[0][0]);
            Assert.Equal(2, chunked[1][0]);
            Assert.Equal(3, chunked[2][0]);
            Assert.Equal(4, chunked[3][0]);
            Assert.Equal(5, chunked[4][0]);
        }

        [Fact]
        public void ChunkingStringsShouldWorkTheSameAsInts()
        {
            List<string> test = new List<string>(new string[] { "a", "b", "c" });
            List<List<string>> chunked = test.ChunkBy(2);
            Assert.Equal(2, chunked.Count);
            Assert.Equal(2, chunked[0].Count);
            Assert.Single(chunked[1]);
            Assert.Equal("a", chunked[0][0]);
            Assert.Equal("b", chunked[0][1]);
            Assert.Equal("c", chunked[1][0]);
        }

        private List<int> m_test = new List<int>(new int[] { 1, 2, 3, 4, 5 });
    }
}
