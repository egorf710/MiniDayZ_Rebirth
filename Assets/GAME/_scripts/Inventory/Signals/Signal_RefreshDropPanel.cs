using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.GAME._scripts.Inventory.Signals
{
    public class Signal_RefreshDropPanel
    {
        public ItemObject[] dropItems;
        public Signal_RefreshDropPanel(ItemObject[] dropItems)
        {
            this.dropItems = dropItems;
        }
    }
}
