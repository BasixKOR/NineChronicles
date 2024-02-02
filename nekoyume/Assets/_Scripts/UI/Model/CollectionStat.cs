using Nekoyume.L10n;
using TMPro;
using UnityEngine;

namespace Nekoyume.UI.Model
{
    public class CollectionStat : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI[] statTexts;

        public void Set(Collection.Model itemData)
        {
            gameObject.SetActive(true);
            nameText.text = L10nManager.Localize($"COLLECTION_NAME_{itemData.Row.Id}");

            var stat = itemData.Row.StatModifiers;
            for (var i = 0; i < statTexts.Length; i++)
            {
                statTexts[i].gameObject.SetActive(i < stat.Count);
                if (i < stat.Count)
                {
                    statTexts[i].text = stat[i].StatModifierToString();
                }
            }
        }
    }
}
