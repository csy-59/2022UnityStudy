using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform UIGroup; //UI 위치
    public Animator anim; //애니메이션

    //구매 관련
    public GameObject[] itemObj;//구매 아이템
    public int[] itemPrice;//아이템 가격
    public Transform[] itemPos;//아이템 소환 위치
    public string[] talkData; //대화 내용
    public Text talkText; //상점 대화

    Player enterPlayer;

    public void Enter(Player player)
    {
        //플레이어 설정
        enterPlayer = player;
        //UI가 정중앙에 오도록
        UIGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        //애니메이션 설정
        anim.SetTrigger("doHello");
        //UI가 원래 자리로 돌아가도록
        UIGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int index)
    {
        //가격
        int price = itemPrice[index];

        //돈이 모자름!
        if(price > enterPlayer.coin)
        {
            //대화하기 변화
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }
        
        //구매
        enterPlayer.coin -= price;//구매 연산
        //구매 위치 랜덤
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3);
        //구매한 아이템 소환
        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);
    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
