using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour {
    public void PressedNetworkBattleButton() {
        SceneTransit.Instance.LoadScene(@"Matching", 0.4f);
    }
}
