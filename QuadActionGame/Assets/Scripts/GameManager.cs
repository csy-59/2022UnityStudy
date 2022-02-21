using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //기본 필요한 변수들 사용
    public GameObject menuCam;//메뉴 카메라
    public GameObject gameCam;//인게임 카메라
    public Player player;//플레이어
    public Boss boss;//보스
    public int stage;//스테이지
    public float playTime;//게임 실행 시간
    public bool isBattle;//전투 중인지
    public int enemyCntA;//남은 적 A의 수
    public int enemyCntB;//남은 적 B의 수
    public int enemyCntC;//남은 적 C의 수

    //UI 관련 변수
    public GameObject menuPanel;//시작 메뉴
    public GameObject gamePanel;//인게임 메뉴

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

    private void Awake()
    {
        //maxScoreText를 0 3개당 ,하나로 포맷을 맞추어 저장
        maxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
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

        //보스 채력
        bossHealthBar.localScale = new Vector3((float) boss.curHealth / boss.maxHealth, 1, 1);
    }
}
