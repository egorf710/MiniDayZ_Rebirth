using UnityEngine;

namespace Assets.GAME._scripts.Fic
{
    public class ServiceLocatorInitializer_GamePlay: MonoBehaviour
    {
        public GamePlayManager gamePlayManager;
        public S_Inventory inventory;

        public void InitializeService()
        {
            ServiceLocator.Reset();

            ServiceLocator.Register<S_Inventory>(inventory); inventory.Init();
            ServiceLocator.Register<GamePlayManager>(gamePlayManager);
        }
    }
}
