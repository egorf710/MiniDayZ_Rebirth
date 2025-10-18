using Assets._scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._scripts.World
{
    
    public interface AliveTarget : Vulnerable
    {
        public Transform getTransform();
        public uint getNetID();
        public bool isActive();
    }
}
