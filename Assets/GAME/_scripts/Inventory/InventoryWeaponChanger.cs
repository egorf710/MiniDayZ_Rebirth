using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.GAME._scripts.Fic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWeaponChanger: MonoBehaviour
{
    public Image changeBttIcon;
    public Sprite[] changeBttSprites;
    public InventorySlot[] weaponSlots;
    public int currentWeaponSlot;

    public void ChangeWeapon()
    {
        int nextI = currentWeaponSlot;

        do
        {
            nextI++;

            if(nextI >= weaponSlots.Length)
            {
                nextI = 0;
                break;
            }
        }
        while(weaponSlots[nextI].SlotIsNull() && nextI != 0);

        if (currentWeaponSlot == nextI) { return; }

        currentWeaponSlot = nextI;

        SelectWeapon(nextI);
    }
    private void SelectWeapon(int index)
    {
        weaponItem weaponItem = null;

        if (!weaponSlots[index].SlotIsNull())
        {
            weaponItem = weaponSlots[index].itemInfo.item as weaponItem;
        }

        changeBttIcon.sprite = changeBttSprites[index];

        EventBus.Invoke<Signal_ChangeWeapon>(new Signal_ChangeWeapon(weaponItem));

        ServiceLocator.Get<S_Weapon>().ChangeWeapon(weaponSlots[index]);

        if (weaponItem != null)
        {
            ServiceLocator.Get<S_Inventory>().SetClothes(weaponItem, true);
        }
        else
        {
            ServiceLocator.Get<S_Inventory>().ClearClothes(AnimationThpe.hand);
        }
    }
    public void WeaponSlotUpdate(InventorySlot inventorySlot)
    {
        int eventI = weaponSlots.ToList().IndexOf(inventorySlot);

        if(currentWeaponSlot < eventI)
        {
            SelectWeapon(eventI);
        }
    }

    public void ChangeBestWeapon()
    {
        for (int i = weaponSlots.Length - 1; i >= 0; i--)
        {
            if (!weaponSlots[i].SlotIsNull() || i == 0)
            {
                currentWeaponSlot = i;
                SelectWeapon(i);
                break;
            }
        }
    }
}
