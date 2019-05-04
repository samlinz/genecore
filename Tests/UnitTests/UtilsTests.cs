using System;
using GeneCore.Common;
using Xunit;

namespace Tests.UnitTests
{
    public class UtilTests
    {
        [Fact]
        public void DeepCopyShouldReturnDeepCopy()
        {
            TestClass obj1 = new TestClass
            {
                Hello = 1
                , Reference = new TestClass2
                {
                    Hello = 1
                    , World = "Test"
                }
            };

            TestClass obj2 = Utils.DeepCopy(obj1);

            Assert.Equal(obj1.Hello, obj2.Hello);
            Assert.Equal(obj1.Reference.Hello, obj2.Reference.Hello);
            Assert.Equal(obj1.Reference.World, obj2.Reference.World);
            Assert.NotEqual(obj1.Reference, obj2.Reference);
            Assert.NotSame(obj1.Reference.World, obj2.Reference.World);
        }
    }


    internal class TestClass
    {
        public Int32 Hello;
        public TestClass2 Reference;
    }

    internal class TestClass2
    {
        public Int32 Hello;
        public String World;
    }
}