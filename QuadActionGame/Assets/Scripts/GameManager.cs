using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //기본 필요한 변수들 사용
    public GameObject menuCam;//메뉴 카메라
    public GameObject gameCam;//인게임 카메라
    public Player player;//플레이어
    public Boss boss;//보스

    //스테이지 시작 시 숨길 요소들
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;

    //전투 관련
    public int stage;//스테이지
    public float playTime;//게임 실행 시간
    public bool isBattle;//전투 중인지
    public int enemyCntA;//남은 적 A의 수
    public int enemyCntB;//남은 적 B의 수
    public int enemyCntC;//남은 적 C의 수
    public int enemyCntD;//남은 적 D의 수
    public Transform[] enemyZones;//적이 소환된 위치
    public GameObject[] enemies; //소환될 적
    public List<int> enemyList;//어떤 스테이지에서 얼마나 소환할건지

    //UI 관련 변수
    public GameObject menuPanel;//시작 메뉴
    public GameObject gamePanel;//인게임 메뉴
    public GameObject gameOverPanel;//게임 오버 메뉴

    public Text maxScoreText; //최고 점수 보이기

    public Text scoreText; //현재 점수
    public Text stageText; //스테이지
    public Text playTimeText; //플레이 시간
    public Text playerHealthText; //플레이어의 채력
    public Text playerAmmoText;//플레이어 탄알
    public Text playerCoinText;//플레이어 코인

    public Image weapon1Img;//1번 무기(망치)
    public Image weapon2Img;//2번 무기(권총)
    public Image weapon3Img;//3번 무기(서브머신건)
    public Image weaponRImg;//R 무기(수류탄)

    public Text enemyAText;//적 A 남은 수
    public Text enemyBText;//적 B 남은 수
    public Text enemyCText;//적 C 남은 수

    public RectTransform bossHealthGroup; //보스의 채력은 보스가 나올 때만 보여주기 위해 위치값 가져옴
    public RectTransform bossHealthBar; //보스 남은 채력 보이기

    public Text curScoreText; //현재 점수
    public Text bestText;//최고 점수 안내

    private void Awake()
    {
        //스테이지별 적 수 초기화
        enemyList = new List<int>();

        //maxScoreText를 0 3개당 ,하나로 포맷을 맞추어 저장
        maxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        //최고 점수가 없으면 초기화
        if(!PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);
    }

    //게임 시작 버튼
    public void GameStart()
    {
        menuCam.SetActive(false); //초기화면 카메라 off
        gameCam.SetActive(true);//인게임 카메라 on

        menuPanel.SetActive(false);//시작 화면 판낼 off
        gamePanel.SetActive(true);//인게임 카메라 on

        player.gameObject.SetActive(true);//플레이어 on
    }

    //게임 오버
    public void GameOver()
    {
        gamePanel.SetActive(false);//게임화면 off
        gameOverPanel.SetActive(true); //게임 오버 화면 보여주기
        curScoreText.text = scoreText.text;//현재 점수 보여주기

        //최고 점수인지
        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if(player.score > maxScore)
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);//최고 점수이면 현재 점수로 최고 점수 초기화
        }
    }

    public void Restart()
    {
        //씬 초기화
        SceneManager.LoadScene(0);
    }

    public void StageStart()
    {
        //스테이지 시작
        isBattle = true;
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);
        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);
        
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        //스테이지 종료
        //종료되면 플레이어는 원 위치로
        player.transform.position = Vector3.up * 0.8f;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);
        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        isBattle = false;
        stage++;
    }

    IEnumerator InBattle()
    {
        //5단위로 보스 나옴
        if(stage % 5 == 0)
        {
            enemyCntD++;
            //보스 생성
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            //에너미의 타겟을 플레이어로 정의
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            //보스 설정
            boss = instantEnemy.GetComponent<Boss>();
        }
        else
        {
            //에너미 리스트에 추가
            for (int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                //추가 수 저장
                switch (ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }

            //에너미 생성
            while (enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);
                //프리펩으로 생성된 애들의 스크립트에서 필요한 플레이어 등은 생성 시 null 상태, 따로 정의 해줘야함
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                //에너미의 타겟을 플레이어로 정의
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;
                //에너미의 메니저 정의
                enemy.manager = this;
                enemyList.RemoveAt(0); //소환하고 하나 지우고, 반복

                //코루틴에서 while문을 돌릴 때는 꼭 여기 안 yield 있어야함
                yield return new WaitForSeconds(4.0f);
            }
        }

        //약간 운체 때 배운 거랑 비슷함. 조건에 만족하지 않는다면 여기에서 넘어가지 않는다.
        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null;
        }

        //모든 적을 죽이면 스테이지 종료
        yield return new WaitForSeconds(4f);

        //보스 잡은 후 처리
        boss = null;
        StageEnd();
    }

    private void Update()
    {

        if (isBattle)
            playTime += Time.deltaTime;
    }

    //LateUpdate: Update가 다 끝나고 실행되는 것
    private void LateUpdate()
    {
        
        scoreText.text = string.Format("{0:n0}", player.score); //점수 보여주기
        stageText.text = "STAGE " + stage.ToString(); //스테이지 보여주기

        //플레이 시간 보여주기
        int hour = (int) (playTime / 3600);
        int min = (int) ((playTime - hour * 3600) / 60);
        int sec = (int) (playTime % 60);
        playTimeText.text = string.Format("{0:00}", hour) + ":" 
            + string.Format("{0:00}", min) + ":"+ string.Format("{0:00}", sec);

        playerHealthText.text = player.health + " / " + player.maxHealth; //플레이어 채력
        playerCoinText.text = string.Format("{0:n0}", player.coin); //코인 보여주기

        //탄약 보여주기(타입에 따라 다름)
        if (player.equipWeapon == null)
            playerAmmoText.text = "- / " + player.maxAmmo;
        else if(player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmoText.text = "- / " + player.maxAmmo;
        else
            playerAmmoText.text = player.ammo + " / " + player.maxAmmo;

        //무기 보유 여부에 따라 이미지 투명도 결정
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.hasGrendes > 0 ? 1 : 0);

        //적의 수
        enemyAText.text = enemyCntA.ToString();
        enemyBText.text = enemyCntB.ToString();
        enemyCText.text = enemyCntC.ToString();

        //보스 채력(보스가 있을 경우에만 사용) 보스가 있을 경우에만 보이고 아니면 안보이게
        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float) boss.curHealth / boss.maxHealth, 1, 1);
        } else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }

    }
}
