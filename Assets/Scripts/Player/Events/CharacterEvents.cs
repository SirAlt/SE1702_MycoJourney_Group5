using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Player.Events
{
    public class CharacterEvents
    {
        public static UnityAction<GameObject, float> characterDamaged;
        public static UnityAction<GameObject, float> characterHealed;


    }
}
