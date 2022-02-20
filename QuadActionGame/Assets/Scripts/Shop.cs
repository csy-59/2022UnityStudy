using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform UIGroup; //UI ��ġ
    public Animator anim; //�ִϸ��̼�

    //���� ����
    public GameObject[] itemObj;//���� ������
    public int[] itemPrice;//������ ����
    public Transform[] itemPos;//������ ��ȯ ��ġ
    public string[] talkData; //��ȭ ����
    public Text talkText; //���� ��ȭ

    Player enterPlayer;

    public void Enter(Player player)
    {
        //�÷��̾� ����
        enterPlayer = player;
        //UI�� ���߾ӿ� ������
        UIGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        //�ִϸ��̼� ����
        anim.SetTrigger("doHello");
        //UI�� ���� �ڸ��� ���ư�����
        UIGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int index)
    {
        //����
        int price = itemPrice[index];

        //���� ���ڸ�!
        if(price > enterPlayer.coin)
        {
            //��ȭ�ϱ� ��ȭ
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }
        
        //����
        enterPlayer.coin -= price;//���� ����
        //���� ��ġ ����
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3);
        //������ ������ ��ȯ
        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);
    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
