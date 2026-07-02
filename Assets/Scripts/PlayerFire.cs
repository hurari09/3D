using UnityEngine;
using static GameManager;

public class PlayerFire : MonoBehaviour
{
    // 무기를 발사할 위치 지정
    public GameObject firePosition;
    // 무기 오브젝트 
    public GameObject bombFactory;
    // 투척 파워
    public float throwPower = 15f;
    // 총알 이펙트 오브젝트
    public GameObject bulletEffect;
    // 총알 이펙트 파티클 시스템
    ParticleSystem ps;
    // 공격력
    public int waeponPower = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ps = bulletEffect.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // 게임 상태가 Run일 때만 조작할 수 있게 함
        if (GameManager.gm.gState != GameState.Run)
        {
            return;
        }

        // 마우스 버튼을 통해 무기를 발사
        if (Input.GetMouseButtonDown(1)) // 0: 좌클릭, 1: 우클릭, 2: 휠
        {
            GameObject bomb = Instantiate(bombFactory);
            bomb.transform.position = firePosition.transform.position + Camera.main.transform.forward;
            Rigidbody rb = bomb.GetComponent<Rigidbody>();
            // 카메라의 정면 방향으로 무기에 물리적 힘을 가함
            rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);
        }

        // 마우스 왼쪽 버튼 입력
        if (Input.GetMouseButtonDown(0))
        {
            // 레이를 생성하고 발사될 위치와 방향을 설정
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            // 레이가 부딪힌 대상의 정보 저장
            RaycastHit hitInfo = new RaycastHit();

            // 만일 부딪힌 물체가 있으면 피격 이펙트를 표시
            if(Physics.Raycast(ray, out hitInfo))
            {
                // 만일 부딫힌 대상의 레이어가 Enmey라면 Damage 함수를 실행
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyFSM eFSM = hitInfo.transform.GetComponent<EnemyFSM>();
                    eFSM.HitEnemy(waeponPower);
                }
                else
                {
                    bulletEffect.transform.position = hitInfo.point;
                    // 피격 이펙트의 forward 방향을 레이가 부딪힌 지점의 수직으로 발생. 충돌 지점의 방향과 일치.
                    bulletEffect.transform.forward = hitInfo.normal;
                    ps.Play();
                }
            }
        }
    }
}
