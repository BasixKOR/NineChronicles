using System;
using Nekoyume.Model.Item;
using Nekoyume.UI.Module;
using UniRx;

namespace Nekoyume.UI.Model
{
    public class Item : IDisposable
    {
        public readonly ReactiveProperty<ItemBase> ItemBase = new ReactiveProperty<ItemBase>();
        public readonly ReactiveProperty<bool> GradeEnabled = new ReactiveProperty<bool>(true);
        public readonly ReactiveProperty<string> Enhancement = new ReactiveProperty<string>();
        public readonly ReactiveProperty<bool> EnhancementEnabled = new ReactiveProperty<bool>(false);
        public readonly ReactiveProperty<bool> Dimmed = new ReactiveProperty<bool>(false);
        public readonly ReactiveProperty<bool> Selected = new ReactiveProperty<bool>(false);

        public readonly Subject<Item> OnClick = new Subject<Item>();
        public readonly Subject<Item> OnDoubleClick = new Subject<Item>();
        
        public Item(ItemBase value)
        {
            ItemBase.Value = value;

            if (ItemBase.Value is Equipment equipment &&
                equipment.level > 0)
            {
                Enhancement.Value = $"+{equipment.level}";
                EnhancementEnabled.Value = true;
            }
            else
            {
                Enhancement.Value = string.Empty;
                EnhancementEnabled.Value = false;
            }
        }

        public virtual void Dispose()
        {
            ItemBase.Dispose();
            GradeEnabled.Dispose();
            Enhancement.Dispose();
            EnhancementEnabled.Dispose();
            Dimmed.Dispose();
            Selected.Dispose();
            
            OnClick.Dispose();
            OnDoubleClick.Dispose();
        }
    }
}
