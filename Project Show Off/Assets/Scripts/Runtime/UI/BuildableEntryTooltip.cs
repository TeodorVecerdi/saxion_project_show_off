using System.Globalization;
using DG.Tweening;
using Runtime.Data;
using TMPro;
using UnityEngine;

namespace Runtime {
    public sealed class BuildableEntryTooltip : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI objectNameText;
        [SerializeField] private TextMeshProUGUI objectDescriptionText;
        [Space]
        [SerializeField] private TextMeshProUGUI glassCount;
        [SerializeField] private CanvasGroup glassCanvasGroup;
        [Space]
        [SerializeField] private TextMeshProUGUI metalCount;
        [SerializeField] private CanvasGroup metalCanvasGroup;
        [Space]
        [SerializeField] private TextMeshProUGUI paperCount;
        [SerializeField] private CanvasGroup paperCanvasGroup;
        [Space]
        [SerializeField] private TextMeshProUGUI plasticCount;
        [SerializeField] private CanvasGroup plasticCanvasGroup;
        

        private CanvasGroup canvasGroup;

        public void LoadUI(BuildableObject buildableObject) {
            canvasGroup = GetComponent<CanvasGroup>();
            
            objectNameText.text = buildableObject.Name;
            objectDescriptionText.text = buildableObject.Description;

            var glassRequirements = buildableObject.ConstructionRequirements.FirstOrDefault(stack => stack.TrashMaterial.Type == TrashMaterial.Types.Glass);
            glassCount.text = glassRequirements == null ? "0" : glassRequirements.Mass.ToString(CultureInfo.InvariantCulture);
            if (glassRequirements == null) glassCanvasGroup.alpha = 0.5f;
            
            var metalRequirements = buildableObject.ConstructionRequirements.FirstOrDefault(stack => stack.TrashMaterial.Type == TrashMaterial.Types.Metal);
            metalCount.text = metalRequirements == null ? "0" : metalRequirements.Mass.ToString(CultureInfo.InvariantCulture);
            if (metalRequirements == null) metalCanvasGroup.alpha = 0.5f;
            
            var paperRequirements = buildableObject.ConstructionRequirements.FirstOrDefault(stack => stack.TrashMaterial.Type == TrashMaterial.Types.Paper);
            paperCount.text = paperRequirements == null ? "0" : paperRequirements.Mass.ToString(CultureInfo.InvariantCulture);
            if (paperRequirements == null) paperCanvasGroup.alpha = 0.5f;
            
            var plasticRequirements = buildableObject.ConstructionRequirements.FirstOrDefault(stack => stack.TrashMaterial.Type == TrashMaterial.Types.Plastic);
            plasticCount.text = plasticRequirements == null ? "0" : plasticRequirements.Mass.ToString(CultureInfo.InvariantCulture);
            if (plasticRequirements == null) plasticCanvasGroup.alpha = 0.5f;

            canvasGroup.alpha = 0.0f;
        }

        public void Show(bool show) {
            canvasGroup.DOFade(show ? 1.0f : 0.0f, 0.25f);
        } 
    }
}