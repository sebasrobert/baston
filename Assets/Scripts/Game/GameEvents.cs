using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEvents {

    public class WeaponHitPlayerEvent : GameEvent {
        public GameObject Shooter { get; set; }
        public GameObject Target { get; set; }
    }

    public class PlayerDieEvent : GameEvent {
        public GameObject Killer { get; set; }
        public GameObject Dead { get; set; }
    }
}
