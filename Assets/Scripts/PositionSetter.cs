using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxPositionSetter : MonoBehaviour
{
    float[] box1Size;                           //1ȣ���� ������ - ���ݿ�
    float[] box2Size;                           //2ȣ���� ������ - ���ݿ�
    public float box1Count;                          //1ȣ���� ī��Ʈ
    public float box2Count;                          //1ȣ���� ī��Ʈ
    public float maxMoveBelt;           //�����̾� �̵��Ÿ� (�ӽ�)

    public Transform box1_Origin;    //1ȣ���� ���� ���� ��ġ
    public Transform box2_Origin;    //2ȣ���� ���� ���� ��ġ
    Vector3 nowPos;                          //������ġ
    Vector3 target1Pos;                     //������1(������)
    Vector3 target2Pos;                     //������2(������)
    Vector3 robotTarget;                   //������ Transfer ����
    Vector3 LM_Origin;                     // ������ ����(�ʱ���ġ) - ����: �����̾� ��Ʈ���� �޴� ��ġ
    Vector3 X_Origin;
    Vector3 Z_Origin;
    Vector3 Load_Origin;
    Vector3 moveBoxTarget;              //box�� �־���� ��
    Vector3 box1_deltaPos;

    public loadBox loadCheck;           //�ε�ý��� ������ - ���� ���� ���� üũ�뵵

    public Transform LMTransfer;
    public Transform X_Transfer;
    public Transform Z_Transfer;
    public Transform Load_Transfer;

    public GameObject Belt;             //�����̾� �̵���(�ӽ�)

    bool isTransfer;                            //�̵�����

    void Start()
    {
        isTransfer = true;

        box1Size = new float[] { -0.19f, 0.22f, 0.09f };        //1ȣ �ڽ� ������ �ʱ�ȭ
        box2Size = new float[] { 0.271f, 0.181f, 0.0151f }; //������ ��ǥ ������ �κ� �� ����;
        nowPos = new Vector3();
        target1Pos = new Vector3();
        target2Pos = new Vector3();
        moveBoxTarget = new Vector3();
        box1_deltaPos = new Vector3();
        box1_deltaPos = Vector3.zero;

        LM_Origin = LMTransfer.localPosition;
        X_Origin = X_Transfer.localPosition;
        Z_Origin = Z_Transfer.localPosition;
        Load_Origin = Load_Transfer.localPosition;

        //LMTransfer�� Y�� ���� �̵��� ����  target��ǥ �缳�� �۾�

    }
    public void LMLoadBtnClkEvnt()
    {
        robotTarget = LMTransfer.localPosition; //LM ������

        robotTarget.z -= moveBoxTarget.x;         //����ǥ��ȯ: z-x�������� �̵�
        StartCoroutine(CoMoveLMCylinder(LM_Origin, robotTarget, 2));
    }

    public void LMOriginBtnClkEvnt()
    {
        robotTarget = LMTransfer.localPosition;
        StartCoroutine(CoMoveLMCylinder(robotTarget, LM_Origin, 2));
    }

    public void XLoadBtnClkEvnt()
    {
        robotTarget = X_Origin;
        robotTarget.x -= moveBoxTarget.y;
        StartCoroutine(CoMoveXCylinder(X_Origin, robotTarget, 2));
    }

    public void XOriginBtnClkEvnt()
    {
        print(ToString(X_Origin));

        robotTarget = X_Transfer.localPosition;

        StartCoroutine(CoMoveXCylinder(robotTarget, X_Origin, 2));

    }

    public void ZLoadBtnClkEvnt()
    {

        print(ToString(X_Origin));
        robotTarget = Z_Origin;
        robotTarget.y -= -moveBoxTarget.z;
        StartCoroutine(CoMoveZCylinder(Z_Origin, robotTarget, 2));

    }

    public void ZOriginBtnClkEvnt()
    {

        robotTarget = Z_Transfer.localPosition;
        StartCoroutine(CoMoveZCylinder(robotTarget, Z_Origin, 2));

    }

    public void CylinderForwardBtnClkEvnt()
    {
        robotTarget = Load_Origin;
        robotTarget.x = -0.2f;
        StartCoroutine(CoMoveLoadCylinder(robotTarget, Load_Origin, 0.5f));
        robotTarget = Vector3.zero;

    }

    public void CylinderBackwardBtnClkEvnt()
    {
        robotTarget = Load_Origin;
        robotTarget.x = -0.2f;
        StartCoroutine(CoMoveLoadCylinder(Load_Origin, robotTarget, 0.5f));
        robotTarget = Vector3.zero;

        if (loadCheck.isBoxLoading[0])
        {
            loadCheck.isBoxLoading[0] = false;
        }
        else if (loadCheck.isBoxLoading[1])
        {
            nowPos = GameObject.FindGameObjectWithTag("Box2").transform.position;
            target2Pos = nowPos - box2_Origin.position;// target pos z��: LM���̵� �̵� ��& X��: Z���ۺ� �̵��� & Y��: X���ۺ� �̵���;
            target2Pos = transAxis(target2Pos);
            moveBoxTarget = target2Pos;

        }
    }

    public void beltOnBtnClkEvnt()
    {
        Vector3 belt_origin = Belt.transform.localPosition;
        Vector3 belt_target = belt_origin;
        belt_target.x = maxMoveBelt;
        StartCoroutine(CoMoveBelt(belt_origin, belt_target, 1f));

        print(box1_deltaPos);
    }

    public Vector3 box1TargetTrans(Vector3 _box1Target)
    {
        box1Count += 1;
        _box1Target.y -= box1Size[1] + 0.01f;
        if (box1Count % 25 == 0)//z�� ��ȯ
        {
            _box1Target.y = 0;
            _box1Target.x = 0;
            _box1Target.z += box1Size[2];
        }
        else if (box1Count % 5 == 0)//X�� ��ȯ
        {
            _box1Target.y = 0;
            _box1Target.x += box1Size[0];
        }//LM�� ��ȯ
        print(box1Count);

        return _box1Target;
    }
    private Vector3 transAxis(Vector3 _targetPos)//x:LM���ۺ�, y:Z���ۺ�, Z: X���ۺ�
    {
        Vector3 result = new Vector3();

        result.x = _targetPos.z;
        result.y = _targetPos.x;
        result.z = _targetPos.y;

        return result;
    }
    private string ToString(Vector3 v)
    {
        return string.Format("targetPos (X: {0:F7}, Y: {1:F7}, Z: {2:F7})", v.x, v.y, v.z);
    }

    IEnumerator CoMoveLMCylinder(Vector3 _originPos, Vector3 _targetPos, float movingTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < movingTime)
        {
            float t = elapsedTime / movingTime;

            LMTransfer.localPosition = Vector3.Lerp(_originPos, _targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator CoMoveXCylinder(Vector3 _originPos, Vector3 _targetPos, float movingTime)

    {
        float elapsedTime = 0f;

        while (elapsedTime < movingTime)
        {
            float t = elapsedTime / movingTime;

            X_Transfer.localPosition = Vector3.Lerp(_originPos, _targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position is exactly targetPos
        //transform.position = moveBoxTarget;
    }
    IEnumerator CoMoveZCylinder(Vector3 _originPos, Vector3 _targetPos, float movingTime)

    {
        float elapsedTime = 0f;

        while (elapsedTime < movingTime)
        {
            float t = elapsedTime / movingTime;

            Z_Transfer.localPosition = Vector3.Lerp(_originPos, _targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position is exactly targetPos
        //transform.position = moveBoxTarget;
    }
    IEnumerator CoMoveLoadCylinder(Vector3 _originPos, Vector3 _targetPos, float movingTime)

    {
        float elapsedTime = 0f;

        while (elapsedTime < movingTime)
        {
            float t = elapsedTime / movingTime;

            Load_Transfer.localPosition = Vector3.Lerp(_originPos, _targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position is exactly targetPos
        //transform.position = moveBoxTarget;
    }
    IEnumerator CoMoveBelt(Vector3 _originPos, Vector3 _targetPos, float movingTime)

    {
        float elapsedTime = 0f;

        while (elapsedTime < movingTime)
        {
            float t = elapsedTime / movingTime;

            Belt.transform.localPosition = Vector3.Lerp(_originPos, _targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        Belt.transform.localPosition = _originPos;

        if (loadCheck.isBoxLoading[0])
        {

            nowPos = GameObject.FindGameObjectWithTag("Box1").transform.position;
            target1Pos = nowPos - box1_Origin.position;
            print(ToString(target1Pos));
            target1Pos = transAxis(target1Pos); //target pos x yz -> ���׹���   lm x z
            target1Pos += box1_deltaPos;// box c
            moveBoxTarget = target1Pos;
            print(ToString(moveBoxTarget));
            box1_deltaPos = box1TargetTrans(box1_deltaPos);
            print("box1 ���� ��ġ ��� ��ȹ �Ϸ� : " + ToString(box1_deltaPos));

        }
        yield return null;
    }
}