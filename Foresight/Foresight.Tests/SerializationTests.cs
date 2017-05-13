using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Foresight.Tests
{
    [TestFixture]
    public class SerializationTests
    {
        private Project project;

        [SetUp]
        public void PrepareProject()
        {
            this.project = new Project();

        }

        [Test]
        public void ProjectRoundTrip()
        {
            
        }
    }
}