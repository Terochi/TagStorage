using osu.Framework.Testing;

namespace TagStorage.App.Tests.Visual
{
    public abstract partial class TagStorageTestScene : TestScene
    {
        protected override ITestSceneTestRunner CreateRunner() => new TagTestSceneTestRunner();

        private partial class TagTestSceneTestRunner : TagStorageAppBase, ITestSceneTestRunner
        {
            private TestSceneTestRunner.TestRunner runner;

            protected override void LoadAsyncComplete()
            {
                base.LoadAsyncComplete();
                Add(runner = new TestSceneTestRunner.TestRunner());
            }

            public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
        }
    }
}
