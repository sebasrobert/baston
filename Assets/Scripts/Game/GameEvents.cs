using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEvents {

    public class WeaponHitPlayerEvent : GameEvent {
        public GameObject Source { get; set; }
        public GameObject Target { get; set; }
        public bool HeadShot { get; set; }
    }

    public class PlayerDieEvent : GameEvent {
        public GameObject Player { get; set; }
    }
}
