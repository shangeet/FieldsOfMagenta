using UnityEditor;

public class CreateAssetBundle {
    [MenuItem ("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles () {
		BuildPipeline.BuildAssetBundles ("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
	}
}
