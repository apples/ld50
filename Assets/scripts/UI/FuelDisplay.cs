using UnityEngine;

namespace UI
{
    public class FuelDisplay : MonoBehaviour
    {
        [SerializeField] private RaftController raftController;
        [SerializeField] private ProgressBar progressBar;

        public Color fullColor;
        public Color emptyColor;

        // Update is called once per frame
        void Update()
        {
            progressBar.current = Mathf.CeilToInt(raftController.FuelTime);
            progressBar.color = Color.Lerp(emptyColor, fullColor, GetFillProgress(progressBar));
        }

        private float GetFillProgress(ProgressBar progressBar)
        {
            float currentOffset = progressBar.current - progressBar.minimum;
            float maximumOffset = progressBar.maximum - progressBar.minimum;
            return currentOffset / maximumOffset;
        }
    }
}
