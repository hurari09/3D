using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 7f;
    CharacterController cc;
    // 중력 변수
    float gravity = -20f;
    // 수직 속력 변수
    float yVelocity = 0;
    // 점프력 변수
    public float jumpPower = 10f;
    // 점프 상태 변수
    public bool isJumping = false;

    // 플레이어 체력 변수
    public int hp;
    // 최대 체력 변수
    int maxHp = 20;
    // 체력 슬라이더 변수
    public Slider hpSlider;

    // Hit 효과 오브젝트
    public GameObject hitEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();

        hp = maxHp;

        hitEffect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // 게임 상태가 Run일 때만 조작할 수 있게 함
        if (GameManager.gm.gState != GameState.Run)
        {
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 이동 방향 설정
        Vector3 dir = new Vector3(h, 0, v);
        dir = dir.normalized;

        // 메인 카메라를 기준으로 방향 변경
        dir = Camera.main.transform.TransformDirection(dir);

        transform.position += dir * moveSpeed * Time.deltaTime;

        // 점프 중이고, 바닥에 착지했다면
        if(isJumping && cc.collisionFlags == CollisionFlags.Below)
        {
            isJumping = false;
            yVelocity = 0;
        }

        // 점프 구현
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            yVelocity = jumpPower;
            isJumping = true;
        }

        // 캐릭터의 수직 속도에 중력 값을 적용
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;

        // 이동 함수
        cc.Move(dir * moveSpeed * Time.deltaTime);

        // 현재 플레이어의 체력을 hp 슬라이더 값에 반영
        hpSlider.value = (float)hp / (float)maxHp;
    }

    // 플레이어의 피격 함수
    public void DamageAction(int damage)
    {
        // 적의 공격력만큼 플레이어의 체력을 깎음
        hp -= damage;
        // 체력이 음수가 될 경우에 0으로 초기화
        if(hp < 0)
        {
            hp = 0;
        }
        else
        {
            // 피격 이펙트 코루틴 시작
            StartCoroutine(PlayHitEffect());
        }
    }
    
    // 피격 효과 코루틴 함수
    IEnumerator PlayHitEffect()
    {
        // 피격 UI를 활성화
        hitEffect.SetActive(true);
        // 일정 시간동안 대기
        yield return new WaitForSeconds(0.2f);
        // 피격 UI를 비활성화
        hitEffect.SetActive(false);
    }
}
