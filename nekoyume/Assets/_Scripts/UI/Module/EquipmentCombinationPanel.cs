﻿using Nekoyume.State;
using Nekoyume.UI.Scroller;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Nekoyume.UI.Module
{
    public class EquipmentCombinationPanel : MonoBehaviour, ICombinationPanel
    {
        public int CostNCG { get; private set; }
        public int CostAP { get; private set; }

        public EquipmentRecipeCellView recipeCellView;
        public CombinationMaterialPanel materialPanel;

        public Button cancelButton;
        public SubmitWithCostButton submitButton;

        public void Awake()
        {
            cancelButton.onClick.AddListener(SubscribeOnClickCancel);
            submitButton.OnSubmitClick.Subscribe(_ =>
            {
                SubscribeOnClickSubmit();
            }).AddTo(gameObject);
        }

        public virtual void SetData(EquipmentRecipeCellView view, int? subRecipeId = null)
        {
            recipeCellView.Set(view.model);
            materialPanel.SetData(view.model, subRecipeId);

            gameObject.SetActive(true);
            CostNCG = (int) materialPanel.costNcg;
            CostAP = materialPanel.costAp;
            if (CostAP > 0)
            {
                submitButton.ShowAP(CostAP, States.Instance.CurrentAvatarState.actionPoint >= CostAP);
            }
            else
            {
                submitButton.HideAP();
            }

            if (CostNCG > 0)
            {
                submitButton.ShowNCG(CostNCG, States.Instance.AgentState.gold >= CostNCG);
            }
            else
            {
                submitButton.HideNCG();
            }
            submitButton.SetSubmittable(materialPanel.IsCraftable);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SubscribeOnClickCancel()
        {
            Widget.Find<Combination>().State.SetValueAndForceNotify(Combination.StateType.NewCombineEquipment);
        }

        public void SubscribeOnClickSubmit()
        {
            Widget.Find<Combination>().State.SetValueAndForceNotify(Combination.StateType.NewCombineEquipment);
        }
    }
}
