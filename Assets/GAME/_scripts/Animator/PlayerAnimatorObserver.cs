using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.GAME._scripts.Fic;
using Assets.GAME._scripts.Inventory.Signals;
using Mirror;
using UnityEngine;

namespace Assets.GAME._scripts.Animator
{
    public class PlayerAnimatorObserver: MonoBehaviour
    {
        [SerializeField] private PlayerAnimator myPlayerAnimator;

        private void Awake()
        {
            EventBus.Subscribe<Signal_SetClothes>(SetClothes);
            EventBus.Subscribe<Signal_ClearClothes>(ClearClothes);
        }
        private void SetClothes(Signal_SetClothes signal)
        {
            myPlayerAnimator.SetClothByName(signal.item.name, signal.isWeapon);
        }
        private void ClearClothes(Signal_ClearClothes signal)
        {
            if(!string.IsNullOrEmpty(signal.itemName))
            {
                myPlayerAnimator.ClearClothByName(signal.itemName);
            }
            else if(signal.byAnimationType)
            {
                myPlayerAnimator.ClearClothByType(signal.animationThpe);
            }
            else if(signal.byType)
            {
                myPlayerAnimator.ClearClothByType(signal.type);
            }
        }
    }
}
