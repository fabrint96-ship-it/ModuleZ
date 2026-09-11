using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace ModuleZ.Tests.PlayMode
{
    public sealed class TestInfrastructurePlayModeTests
    {
        [UnityTest]
        public IEnumerator UnityTestCanYieldOneFrame()
        {
            yield return null;

            Assert.Pass("PlayMode test resumed after yielding one frame.");
        }
    }
}
