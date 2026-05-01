using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.GAME._scripts.Inventory.Signals
{
    public class Signal_ClearClothes
    {
        public string itemName;
        public bool byType;
        public bool byAnimationType;
        public ItemType type;
        public AnimationThpe animationThpe;
        public Signal_ClearClothes(string itemName)
        {
            this.itemName = itemName;
        }
        public Signal_ClearClothes(ItemType itemType)
        {
            this.type = itemType;
            byType = true;
        }
        public Signal_ClearClothes(AnimationThpe animType)
        {
            this.animationThpe = animType;
            byAnimationType = true;
        }
    }
}
