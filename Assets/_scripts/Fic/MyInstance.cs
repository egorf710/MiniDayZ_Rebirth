using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets._scripts
{
    public static class MyInstance
    {
        public static Vector2 RandomVector(float range)
        {
            return new Vector2(UnityEngine.Random.Range(-range, range), UnityEngine.Random.Range(-range, range));
        }
    }
}
