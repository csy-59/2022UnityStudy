using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //�⺻ �ʿ��� ������ ���
    public GameObject menuCam;//�޴� ī�޶�
    public GameObject gameCam;//�ΰ��� ī�޶�
    public Player player;//�÷��̾�
    public Boss boss;//����

    //�������� ���� �� ���� ��ҵ�
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;

    //���� ����
    public int stage;//��������
    public float playTime;//���� ���� �ð�
    public bool isBattle;//���� ������
    public int enemyCntA;//���� �� A�� ��
    public int enemyCntB;//���� �� B�� ��
    public int enemyCntC;//���� �� C�� ��
    public int enemyCntD;//���� �� D�� ��
    public Transform[] enemyZones;//���� ��ȯ�� ��ġ
    public GameObject[] enemies; //��ȯ�� ��
    public List<int> enemyList;//� ������������ �󸶳� ��ȯ�Ұ���

    //UI ���� ����
    public GameObject menuPanel;//���� �޴�
    public GameObject gamePanel;//�ΰ��� �޴�
    public GameObject gameOverPanel;//���� ���� �޴�

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

    public Text curScoreText; //���� ����
    public Text bestText;//�ְ� ���� �ȳ�

    private void Awake()
    {
        //���������� �� �� �ʱ�ȭ
        enemyList = new List<int>();

        //maxScoreText�� 0 3���� ,�ϳ��� ������ ���߾� ����
        maxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        //�ְ� ������ ������ �ʱ�ȭ
        if(!PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);
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

    //���� ����
    public void GameOver()
    {
        gamePanel.SetActive(false);//����ȭ�� off
        gameOverPanel.SetActive(true); //���� ���� ȭ�� �����ֱ�
        curScoreText.text = scoreText.text;//���� ���� �����ֱ�

        //�ְ� ��������
        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if(player.score > maxScore)
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);//�ְ� �����̸� ���� ������ �ְ� ���� �ʱ�ȭ
        }
    }

    public void Restart()
    {
        //�� �ʱ�ȭ
        SceneManager.LoadScene(0);
    }

    public void StageStart()
    {
        //�������� ����
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
        //�������� ����
        //����Ǹ� �÷��̾�� �� ��ġ��
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
        //5������ ���� ����
        if(stage % 5 == 0)
        {
            enemyCntD++;
            //���� ����
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            //���ʹ��� Ÿ���� �÷��̾�� ����
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            //���� ����
            boss = instantEnemy.GetComponent<Boss>();
        }
        else
        {
            //���ʹ� ����Ʈ�� �߰�
            for (int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                //�߰� �� ����
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

            //���ʹ� ����
            while (enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);
                //���������� ������ �ֵ��� ��ũ��Ʈ���� �ʿ��� �÷��̾� ���� ���� �� null ����, ���� ���� �������
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                //���ʹ��� Ÿ���� �÷��̾�� ����
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;
                //���ʹ��� �޴��� ����
                enemy.manager = this;
                enemyList.RemoveAt(0); //��ȯ�ϰ� �ϳ� �����, �ݺ�

                //�ڷ�ƾ���� while���� ���� ���� �� ���� �� yield �־����
                yield return new WaitForSeconds(4.0f);
            }
        }

        //�ణ ��ü �� ��� �Ŷ� �����. ���ǿ� �������� �ʴ´ٸ� ���⿡�� �Ѿ�� �ʴ´�.
        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null;
        }

        //��� ���� ���̸� �������� ����
        yield return new WaitForSeconds(4f);

        //���� ���� �� ó��
        boss = null;
        StageEnd();
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

        //���� ä��(������ ���� ��쿡�� ���) ������ ���� ��쿡�� ���̰� �ƴϸ� �Ⱥ��̰�
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
