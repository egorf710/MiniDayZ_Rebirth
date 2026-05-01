using UnityEngine;

namespace Assets.GAME._scripts.Fic
{
    public class ServiceLocatorInitializer_GamePlay: MonoBehaviour
    {
        public GamePlayManager gamePlayManager;
        public S_Inventory inventory;
        public InventoryWeaponChanger inventoryWeaponChanger;
        public S_Weapon weapon;

        public void InitializeService()
        {
            ServiceLocator.Reset();

            ServiceLocator.Register<S_Inventory>(inventory); inventory.Init();
            ServiceLocator.Register<InventoryWeaponChanger>(inventoryWeaponChanger);
            ServiceLocator.Register<GamePlayManager>(gamePlayManager);
            ServiceLocator.Register<S_Weapon>(weapon); weapon.Init();
        }
    }
}
