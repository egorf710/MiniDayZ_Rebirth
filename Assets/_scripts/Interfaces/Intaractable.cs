using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._scripts.Interfaces
{
    public interface Interactable
    {
        public void Interact();
        public bool IsInteractable(out string message);
        /// <summary>
        /// When interact button active
        /// </summary>
        /// <returns></returns>
        public bool IsActive();
        public Vector2 GetPosition();
    }
    public interface Statetable
    {
        public void SetStatetable(int state);
        public int GetStatetable();
    }
    [Serializable]
    public struct VulnerabledData
    {
        public int health;
        public int armor;
        public int blockChanse;
    }
    public interface Vulnerable
    {
        public void TakeDamage(int damage, int code = 0); //code - Unified ID target
    }
    public interface Initializable
    {
        public void Init(params dynamic[] args);
    }
}
