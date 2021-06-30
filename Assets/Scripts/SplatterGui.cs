using UnityEngine;

namespace Splatter.Assets.Scripts {
    [ExecuteInEditMode]
    public class SplatterGui : MonoBehaviour {
        private void OnEnable() {
            GetComponent<Splatter>().Run();
        }
    }
}
