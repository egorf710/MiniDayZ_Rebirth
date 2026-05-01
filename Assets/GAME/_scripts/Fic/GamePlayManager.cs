using UnityEngine;

namespace Assets.GAME._scripts.Fic
{
    public class GamePlayManager: MonoBehaviour
    {
        public ServiceLocatorInitializer_GamePlay serviceLocatorInitializer;


        private GameObject myPlayer;

        private void Awake()
        {
            myPlayer = GameObject.FindGameObjectWithTag("Player");

            serviceLocatorInitializer.InitializeService();
        }

        public GameObject GetMyPlayer()
        {
            return myPlayer;
        }
    }
}
