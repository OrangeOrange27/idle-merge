using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Common.Tests.EditorAssetsCache
{
	[InitializeOnLoad]
	public sealed class EditorAssetsCacheLifecycleController
	{
		static EditorAssetsCacheLifecycleController()
		{
			SetupListeners();
		}

		private static void SetupListeners()
		{
			var api = ScriptableObject.CreateInstance<TestRunnerApi>();
			api.RegisterCallbacks(new TestCallbacks());
		}

		private sealed class TestCallbacks : ICallbacks
		{
			public void RunStarted(ITestAdaptor testsToRun)
			{
				EditorAddressableAssetSharedProvider.Instance.Initialize();
			}

			public void RunFinished(ITestResultAdaptor result)
			{
				EditorAddressableAssetSharedProvider.Instance.CleanUp();
			}

			public void TestStarted(ITestAdaptor test)
			{
				Debug.Log(string.Format("[Test] Test started {0} ", test.Name));

			}

			public void TestFinished(ITestResultAdaptor result)
			{
				Debug.Log(string.Format("[Test] Test finished {0} ", result.Name));
			}
		}
	}
}