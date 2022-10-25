using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusJump : IUpgrade {
  public override void LoadUpgrade(PlayerController player) {
      player.GetJumps().ChangeVal(player.GetJumps().Current + 1);
  }
}
