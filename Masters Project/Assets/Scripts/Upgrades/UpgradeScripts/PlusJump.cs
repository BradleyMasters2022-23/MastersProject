using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusJump : IUpgrade {
    public override void LoadUpgrade(PlayerController player) {
      player.GetJumps().Increment(1);
      player.RefreshJumps();
    }
}
