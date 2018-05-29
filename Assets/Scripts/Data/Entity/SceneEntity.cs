using CAFU.Core.Data.Entity;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace CAFU.Routing.Data.Entity
{
    [PublicAPI]
    public class SceneEntity : IEntity
    {
        /// <summary>
        /// シーン名
        /// </summary>
        /// <remarks>UnityScene との二重管理になるが、Unload 時に Unload した Scene の構造体を拾えないため、やむを得ず…。</remarks>
        public string Name { get; set; }

        public Scene UnityScene { get; set; }
    }
}