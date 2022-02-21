using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //�⺻ �ʿ��� ������ ���
    public GameObject menuCam;//�޴� ī�޶�
    public GameObject gameCam;//�ΰ��� ī�޶�
    public Player player;//�÷��̾�
    public Boss boss;//����
    public int stage;//��������
    public float playTime;//���� ���� �ð�
    public bool isBattle;//���� ������
    public int enemyCntA;//���� �� A�� ��
    public int enemyCntB;//���� �� B�� ��
    public int enemyCntC;//���� �� C�� ��

    //UI ���� ����
    public GameObject menuPanel;//���� �޴�
    public GameObject gamePanel;//�ΰ��� �޴�

    public Text maxScoreText; //�ְ� ���� ���̱�

    public Text scoreText; //���� ����
    public Text stageText; //��������
    public Text playTimeText; //�÷��� �ð�
    public Text playerHealthText; //�÷��̾��� ä��
    public Text playerAmmoText;//�÷��̾� ź��
    public Text playerCoinText;//�÷��̾� ����

    public Image weapon1Img;//1�� ����(��ġ)
    public Image weapon2Img;//2�� ����(����)
    public Image weapon3Img;//3�� ����(����ӽŰ�)
    public Image weaponRImg;//R ����(����ź)

    public Text enemyAText;//�� A ���� ��
    public Text enemyBText;//�� B ���� ��
    public Text enemyCText;//�� C ���� ��

    public RectTransform bossHealthGroup; //������ ä���� ������ ���� ���� �����ֱ� ���� ��ġ�� ������
    public RectTransform bossHealthBar; //���� ���� ä�� ���̱�

    private void Awake()
    {
        //maxScoreText�� 0 3���� ,�ϳ��� ������ ���߾� ����
        maxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
    }

    //���� ���� ��ư
    public void GameStart()
    {
        menuCam.SetActive(false); //�ʱ�ȭ�� ī�޶� off
        gameCam.SetActive(true);//�ΰ��� ī�޶� on

        menuPanel.SetActive(false);//���� ȭ�� �ǳ� off
        gamePanel.SetActive(true);//�ΰ��� ī�޶� on

        player.gameObject.SetActive(true);//�÷��̾� on
    }

    private void Update()
    {

        if (isBattle)
            playTime += Time.deltaTime;
    }

    //LateUpdate: Update�� �� ������ ����Ǵ� ��
    private void LateUpdate()
    {
        
        scoreText.text = string.Format("{0:n0}", player.score); //���� �����ֱ�
        stageText.text = "STAGE " + stage.ToString(); //�������� �����ֱ�

        //�÷��� �ð� �����ֱ�
        int hour = (int) (playTime / 3600);
        int min = (int) ((playTime - hour * 3600) / 60);
        int sec = (int) (playTime % 60);
        playTimeText.text = string.Format("{0:00}", hour) + ":" 
            + string.Format("{0:00}", min) + ":"+ string.Format("{0:00}", sec);

        playerHealthText.text = player.health + " / " + player.maxHealth; //�÷��̾� ä��
        playerCoinText.text = string.Format("{0:n0}", player.coin); //���� �����ֱ�

        //ź�� �����ֱ�(Ÿ�Կ� ���� �ٸ�)
        if (player.equipWeapon == null)
            playerAmmoText.text = "- / " + player.maxAmmo;
        else if(player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmoText.text = "- / " + player.maxAmmo;
        else
            playerAmmoText.text = player.ammo + " / " + player.maxAmmo;

        //���� ���� ���ο� ���� �̹��� ���� ����
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.hasGrendes > 0 ? 1 : 0);

        //���� ��
        enemyAText.text = enemyCntA.ToString();
        enemyBText.text = enemyCntB.ToString();
        enemyCText.text = enemyCntC.ToString();

        //���� ä��
        bossHealthBar.localScale = new Vector3((float) boss.curHealth / boss.maxHealth, 1, 1);
    }
}
