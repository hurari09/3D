using UnityEngine;
using static GameManager;

public class PlayerRotate : MonoBehaviour
{
    public float rotSpeed = 200f;
    float mx = 0;

    // Update is called once per frame
    void Update()
    {
        // 게임 상태가 Run일 때만 조작할 수 있게 함
        if (GameManager.gm.gState != GameState.Run)
        {
            return;
        }

        float mouse_X = Input.GetAxis("Mouse X");
        mx += mouse_X * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, mx, 0);
    }
}
