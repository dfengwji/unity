using UnityEngine.UI;

namespace ZStart.Common.View.Parts
{
    public class ProgressParts : AppParts
    {
        public Slider slider;
        public Text label;

        public void UpdateLabel(string tip)
        {
            label.text = tip;
        }

        public void UpdateProgress(float amount)
        {
            slider.value = amount;
        }

        public void UpdateProgress(string tip, float amount)
        {
            label.text = tip;
            slider.value = amount;
        }

        public override void Clear()
        {
            
        }
    }
}
