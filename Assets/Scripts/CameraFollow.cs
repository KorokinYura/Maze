using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    void Update()
    {
        FollowAfterPlayer();
    }

    private void FollowAfterPlayer()
    {
        if (Spawner.Instance.Player != null)
            Camera.main.transform.position = new Vector3(Spawner.Instance.Player.transform.position.x,
                                                         Spawner.Instance.Player.transform.position.y,
                                                         Camera.main.transform.position.z);
    }
}
