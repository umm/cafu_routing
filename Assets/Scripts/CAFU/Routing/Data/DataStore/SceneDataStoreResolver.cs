using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace CAFU.Routing.Data.DataStore {

    public class SceneDataStoreResolver {

        private IEnumerable<string> InBuildScenePathList { get; } = Enumerable.Range(0, SceneManager.sceneCountInBuildSettings).Select(SceneUtility.GetScenePathByBuildIndex);

        private ISceneDataStore InBuildSceneDataStore { get; } = new InBuildSceneDataStore.Factory().Create();

        private ISceneDataStore AssetBundleSceneDataStore { get; } = new AssetBundleSceneDataStore.Factory().Create();

        public ISceneDataStore ResolveSceneDataStore(string sceneName) {
            // Scene 構造体を事前に保持しておく手段がないため、無理矢理正規表現でチェックする
            //   LoadScene されていないと Scene 構造体が作られない仕様らしい
            return InBuildScenePathList.Any(scenePath => Regex.IsMatch(scenePath, $"{sceneName}\\.unity$")) ? this.InBuildSceneDataStore : this.AssetBundleSceneDataStore;
        }

    }

}