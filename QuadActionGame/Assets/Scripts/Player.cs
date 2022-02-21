using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    //���� ����
    public GameObject[] weapons; //���� ����
    public bool[] hasWeapons; //���� �ִ���
    public GameObject[] grenades; //����ź
    public int hasGrendes; //����ź ����
    public GameObject grenadeObject; //����ź ������

    //ī�޶� ����
    public Camera followCamera;

    //������ ����
    public int ammo; //�Ѿ�
    public int coin; //����
    public int health; //ä��
    public int score; //����

    public int maxAmmo; //�ִ� �Ѿ�
    public int maxCoin; //�ִ� ����
    public int maxHealth; //�ִ� ä��
    public int maxHasGrendes; //�ִ� ����ź

    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    //�����ϱ�
    bool fDown;
    //����ź ������
    bool gDown;
    //����
    bool rDown;
    //���� �ٲٱ�
    bool sDown1; //1��
    bool sDown2; //2��
    bool sDown3; //3��
    //������ ��ȣ�ۿ�
    bool iDown;

    //���� ������
    bool isJump;
    //ȸ�� ������
    bool isDodge;
    //���� ���� ������
    bool isSwap;
    //���� ��������
    bool isFireReady = true;
    //������ ������
    bool isReload;
    //������
    bool isBorder;
    //���ݹ޴� ������
    bool isDamage;
    //���� ������
    bool isShop;

    //����
    Vector3 moveVec; //�̵��� ����
    Vector3 dodgeVec; //ȸ�ǽ� ����

    //���� ȿ���� ���� ����
    Rigidbody rigid;
    //�ִϸ��̼� ����
    Animator anim;
    //�÷��̾��� Mesh�� ���� �迭
    MeshRenderer[] meshs;

    //������ ����
    GameObject nearObject;
    //�������� ����
    public Weapon equipWeapon;
    int equipWeaponIndex = -1;
    //���� ������
    float fireDelay; 

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();

        Debug.Log(PlayerPrefs.GetInt("MaxScore"));
        //����Ƽ���� �����ϴ� ������ ���� ���
        //PlayerPrefs.SetInt("MaxScore", 112500);
    }

    // Update is called once per frame
    void Update()
    {
        //�� �޾ƿ���
        GetInput();
        //�̵�
        Move();
        //ȸ��
        Turn();
        //����
        Jump();
        //����ź
        Grenade();
        //����
        Attack();
        //����
        Reload();
        //ȸ��
        Dodge();
        //��ȣ�ۿ�
        Interation();
        //���� �ٲٱ�
        Swap();
    }

    void GetInput()
    {
        //�̵����� �� �޾ƿ���
        hAxis = Input.GetAxisRaw("Horizontal"); //�¿�
        vAxis = Input.GetAxisRaw("Vertical"); //�յ�
        wDown = Input.GetButton("Walk"); //�ȱ�
        jDown = Input.GetButtonDown("Jump"); //����

        iDown = Input.GetButtonDown("Interation"); //��ȣ�ۿ�(������)
        
        //���� ���� ���� �� �޾ƿ���
        sDown1 = Input.GetButtonDown("Swap1"); //���� �ٲٱ�:1��
        sDown2 = Input.GetButtonDown("Swap2"); //���� �ٲٱ�:2��
        sDown3 = Input.GetButtonDown("Swap3"); //���� �ٲٱ�:3��

        //���� ��ư
        fDown = Input.GetButtonDown("Fire1");
        //����ź ��ư
        gDown = Input.GetButtonDown("Fire2");
        //������
        rDown = Input.GetButtonDown("Reload");

    }

    void Move()
    {
        //�밢������ �̵��� �ణ ���̰� �� ���� 1�� �ٲ��ִ� ��: normalized
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //ȸ�Ǹ� �ϰ� ���� ��� ȸ�� �������θ� �̵��� �� �ֵ���
        if (isDodge)
            moveVec = dodgeVec;


        //���� ���� ���̶��, �������� �ƴ϶�� �̵����� ����
        if (isSwap || !isFireReady || isReload)
            moveVec = Vector3.zero;

        //��迡 ���� ��� �̵����� ����
        if (!isBorder)
        {
            //�̵����� �ȱ� �Ҷ��� �ӵ� ���߱�(0.3���)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        }




        //�ִϸ��̼� ����
        anim.SetBool("isRun", moveVec != Vector3.zero); //�޸���(�̵� ���� 0�� �ƴ� ��)
        anim.SetBool("isWalk", wDown); //�ȱ�(left shift�� ������ ��)
    }

    void Turn()
    {
        //Ű���忡 ���� ȸ���ϱ�: �ٶ󺸴� �������� �� ȸ��
        transform.LookAt(transform.position + moveVec);

        //���콺�� ���� ȸ��
        if (fDown)//���콺�� Ŭ���Ǿ��� ���� �ٶ󺸰� ��
        {
            //ī�޶󿡼� ���콺�� ���� �������� ���̸� ��
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            //out: ���̿��� ���� ����� �ִٸ� rayHit�� ����
            if(Physics.Raycast(ray, out rayHit, 100))
            {
                //���콺 Ŭ�� ��� ��ġ
                Vector3 nextVec = rayHit.point - transform.position;
                //���̴� �����ϵ���
                nextVec.y = 0;
                //�ش� �������� ����
                transform.LookAt(transform.position + nextVec);
            }
        }
        
    }
    void Jump()
    {
        //����Ű�� ������ ������/ȸ������ �ƴϰ�, �������� ���� ��� ����
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap && !isShop)
        {
            //������ ���� �������� 15��ŭ�� ���� ������(Impulse)���� ��
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            //�ִϸ��̼� ȿ��
            anim.SetBool("isJump", true);//������ true��(land �ִϸ��̼��� ����)
            anim.SetTrigger("doJump");//���� �ִϸ��̼� Ʈ���ŷ� �ҷ�����
            isJump = true;
        }
    }

    void Attack()
    {
        //�����ϱ�
        //������ ���Ⱑ ���� ���
        if (equipWeapon == null)
            return;
 
        //���� ��� �ð��߰�
        fireDelay += Time.deltaTime;
        //���� ���� ����: ���ӱ� ��� �ð����� Ŭ ���
        isFireReady = equipWeapon.rate < fireDelay; 
 
        //����Ű�� ������, ���� �����ϰ�, ȸ�ǳ� ���⸦ ��ü���� �ƴ϶�� ������
        if(fDown && isFireReady && !isDodge && !isSwap && !isShop)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");//�ִϸ��̼� ó��
            fireDelay = 0;//���� ��� �ð� �ʱ�ȭ
        }
    }

    void Grenade()
    {
        //����ź�� ���ٸ� ��ȯ
        if (hasGrendes == 0)
            return;

        //����ź ��ư�� ������ �������� ���⺯�� ���� �ƴ϶��
        if(gDown && !isReload && !isSwap && !isShop)
        {
            //���콺�� Ŭ���� ���� ����ź ������
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            //out: ���̿��� ���� ����� �ִٸ� rayHit�� ����
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                //���콺 Ŭ�� ��� ��ġ
                Vector3 nextVec = rayHit.point - transform.position;
                //�ణ ���̰� �ְ� ����
                nextVec.y = 15;

                //������� ����ź ����
                GameObject instantGrenade = Instantiate(grenadeObject, transform.position, transform.rotation);
                //����ź ������
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                //����ź�� ȸ�� �ֱ�
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrendes--;
                grenades[hasGrendes].SetActive(false);
            }
        }
    }

    void Reload()
    {
        //������
        //���Ⱑ ������ ����
        if (equipWeapon == null)
            return;
        //���Ⱑ �ٰŸ� ����� ����
        if (equipWeapon.type == Weapon.Type.Melee)
            return;
        //���� �Ѿ��� ������ ����
        if (ammo == 0)
            return;

        //Debug.Log(rDown + ": ������" + !isJump + ": ������" + !isDodge + ":ȸ����" + !isSwap + "���� ��ȯ��"+ !isFireReady + "���� ��ȯ��");
        //������ Ű�� ������, ����, ȸ��, ���� ���� ���� �ƴϰ�, �ݹ����� �ƴ� ���
        if (rDown && !isJump && !isDodge && !isSwap && isFireReady && !isShop)
        {
            anim.SetTrigger("doReload"); //�ִϸ��̼� ����
            isReload = true;//������ �� ǥ��
 
            //������ �� ǥ�� ����(������ �ð�)
            Invoke("ReloadOut", 3f);
        }
    }
    void ReloadOut()
    {
        //�����ִ� źâ ������ max ���� ������ max��, �ƴϸ� ���� źâ �� ��ŭ �־���
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        //�÷��̾��� źâ �� ����
        ammo -= reAmmo;
        isReload = false;
    }

    void Dodge()
    {
        //����Ű�� ������ �������� �ƴϰ�, �����̴� ���� �� ȸ��
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap && !isShop)
        {
            dodgeVec = moveVec;
            //ȸ������ ���� �̵��ӵ� 2��
            speed *= 2;
            anim.SetTrigger("doDodge");//ȸ�� �ִϸ��̼� Ʈ���ŷ� �ҷ�����
            isDodge = true;

            //�ð����� �ΰ� ȸ�� ����
            Invoke("DodgeOut", 0.6f);
        }
    }
    void DodgeOut()
    {
        //ȸ�� ����
        speed /= 2;
        isDodge = false;
    }   
    
    void Swap()
    {
        //���� ����
        //�����Ϸ��� ���Ⱑ ���ų� �̹� ���� ���� ��� �������� ����
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        //������ ���� ��ȣ �޾ƿ���
        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        //���Ⱑ ������ ������ ȸ�� ���� �ƴ� ���
        if((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isSwap && !isShop)
        {
            //�̹� �������� ���Ⱑ ������ setActive �� ������� ����
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);
            equipWeaponIndex = weaponIndex;//������ ���� �ε��� ����
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>(); //������ ���� ������Ű��
            weapons[weaponIndex].gameObject.SetActive(true); //������ ���� Ȱ��ȭ

            //�ٲٱ� �ִϸ��̼� �ҷ�����
            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.04f);
        }
    }
    void SwapOut()
    {
        isSwap = false;
    }

    void Interation()
    {
        //��ȣ�ۿ�
        //��ȿ�ۿ� Ű ����, ��ó�� ������Ʈ�� ����, ������ ȸ�� ���� �ƴϸ� ��ȣ�ۿ�
        if(iDown && nearObject != null && !isJump && !isDodge && !isShop)
        {
            //������ ���
            if(nearObject.tag == "Weapon")
            {
                //�ֺ��� ������ ���⸦ �޾ƿ�
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value; //�ش� ���� ��ȣ
                hasWeapons[weaponIndex] = true;

                //����� ȹ���� �� �����
                Destroy(nearObject);
            }
            //������ ���
            else if(nearObject.tag == "Shop")
            {
                //���� ��ũ��Ʈ ��������
                Shop shop = nearObject.GetComponent<Shop>();
                //���� ���� ó��
                isShop = true;
                shop.Enter(this);
            }
        }
    }

    void FreezeRotation()
    {
        //ĳ���Ͱ� �ٸ� rigidBody�� �浹���� �� �Ͼ�� ȸ���� 0���� �ʱ�ȭ
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        //������ ���̻� �̵����� �ʱ� ���� ���� ����
        //������ ������, ������ ����� ����, ��
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        //���̾��ũ�� Wall�� ��ü�� ���̰� ������ isBorder�� true
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    private void FixedUpdate()
    {
        //FixedUpdate: ������ ������ ������������ ȣ��Ǵ� �Լ�
        FreezeRotation();
        StopToWall();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //�ɸ��Ͱ� �ٴڰ� ����ִٸ� '������'�� false��
        if(collision.gameObject.tag == "Floor")
        {
            //�ִϸ��̼� ȿ��(land �ִϸ��̼� ��������)
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //������ ����
        if (other.tag == "Weapon" || other.tag == "Shop")
        {
            nearObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //������ ���� ����
        if (other.tag == "Weapon") //����
            nearObject = null;
        //���� ���� ����
        else if(other.tag == "Shop")
        {
            //���� ��ũ��Ʈ ��������
            Shop shop = nearObject.GetComponent<Shop>();
            //������ ó��
            if(shop != null)
            {
                shop.Exit();
            }
            
            isShop = false;
            nearObject = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //������ ȹ��
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo://�Ѿ� ȹ��
                    ammo += item.value;
                    if (ammo > maxAmmo) //�ִ� ���� ���� ���ϵ���
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin://���� ȹ��
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart://ä�� ȹ��
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade://����ź ȹ��
                    //����ź Ȱ��ȭ
                    grenades[hasGrendes].SetActive(true);
                    hasGrendes += item.value;
                    if (hasGrendes > maxHasGrendes)
                        hasGrendes = maxHasGrendes;
                    break;
            }

            //ȹ���� �������� ��Ʈ����
            Destroy(other.gameObject);
        }
        else if(other.tag == "EnemyBullet")
        {
            if (!isDamage) //�������� �ް� �ִ� ���� �ƴ϶�� ����
            {
                //������ ���ݹ��� ���
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                //���� ����� �������
                bool isBossAtk = other.name == "Boss Melee Area";

                //�ǰݽ� �ڷ�ƾ ����
                StartCoroutine(onDamage(isBossAtk));

            }

            //�������� �ް� �ִ� ���̴� �ƴϴ� �׳� ������ ���ֱ�
            //rigidBody�� ������ �ش� bullet ����
            if (other.GetComponent<Rigidbody>() != null)
            {
                Destroy(other.gameObject);
            }

        }
    }

    IEnumerator onDamage(bool isBossAtk)
    {
        //�ǰݽ� 1�ʵ��� ��������� ���� ����
        isDamage = true;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }

        //���� ������ �����̶��
        if (isBossAtk)
        {
            //���ݹް� �ڷ� ��������
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(1f);

        //�ٽ� ������� ���ƿ�
        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

        //���� ���ݽ� �־��� �� �ٽ� �ʱ�ȭ
        if (isBossAtk)
        {
            rigid.velocity = Vector3.zero;
        }
    }
}
