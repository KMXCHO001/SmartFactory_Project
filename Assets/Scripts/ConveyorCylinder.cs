using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using mxComponent;
using static UnityEditor.PlayerSettings;
//���۽� �÷��̾ �� �������� �̵�

public class ConveyorCylinder : MonoBehaviour
{
    public enum Cylinder
    {
        Conveyor_PushCylinder,
        Conveyor_GateCylinder,
    }
    public Cylinder cylinder = Cylinder.Conveyor_PushCylinder;

    public MovePos movepos = MovePos.X;
    public enum MovePos
    {
        X, Y, Z
    }
    public float runTime = 2;
    public float minRange; //����
    public float maxRange; //����
    public Vector3 minPos;
    public Vector3 maxPos;
    public CylinderSensor forwardSensor;//���� ����
    public CylinderSensor backwardSensor;//���� ����
    public Sensor objectSensor;//��ǰ Ȯ�ο�
    public Sensor returnSensor;
    public bool isCylinderMoving = false;

    public CylinderStatusData cylinderStatusData;

    [Header("PLC")]
    public int plcInputValue;

    private void Start()
    {
        if (movepos == MovePos.X)
        {
            minPos = new Vector3(minRange, transform.localPosition.y, transform.localPosition.z);
            maxPos = new Vector3(maxRange, transform.localPosition.y, transform.localPosition.z);
        }
        else if (movepos == MovePos.Y)
        {
            minPos = new Vector3(transform.localPosition.x, minRange, transform.localPosition.z);
            maxPos = new Vector3(transform.localPosition.x, maxRange, transform.localPosition.z);
        }
        else
        {
            minPos = new Vector3(transform.localPosition.x, transform.localPosition.y, minRange);
            maxPos = new Vector3(transform.localPosition.x, transform.localPosition.y, maxRange);
        }
    }

    void Update() //�������� ���ŵɶ� ����Ǵ� �޼��� 0.002~0.004�ʿ� �ѹ��� ����
    {
        if(MxComponent.instance.connection == MxComponent.Connection.Connected)
        {
            Vector3 directon = Vector3.back; //�� ��ġ����  destination������ ����
            switch (cylinder)
            {
                case Cylinder.Conveyor_PushCylinder:
                    if (//plcInputValues[0] > 0 &&
                        !isCylinderMoving && backwardSensor.isObjectDetected && objectSensor.isObjectDetected)
                    {
                        StartCoroutine(CylMove(true));                                             // �Ǹ��� ���� 
                    }

                    if (//plcInputValues[1] > 0 &&
                        !isCylinderMoving && forwardSensor.isObjectDetected && returnSensor.isObjectDetected)
                    {
                        StartCoroutine(CylMove(false));                                           // �Ǹ��� ����
                    }
                    break;

                case Cylinder.Conveyor_GateCylinder:
                    if (//plcInputValues[0] > 0 &&
                        !isCylinderMoving && forwardSensor.isObjectDetected && objectSensor.isObjectDetected)
                    {
                        StartCoroutine(CylMove(true));                                             // �Ǹ��� ���� 
                    }

                    if (//plcInputValues[1] > 0 &&
                        !isCylinderMoving && backwardSensor.isObjectDetected && returnSensor.isObjectDetected)
                    {
                        StartCoroutine(CylMove(false));                                           // �Ǹ��� ����
                    }
                    break;
            }
            }
        }

    public void OnCylinderButtonClickEvent(bool dir)
    {
        StartCoroutine(CylMove(dir));
    }

    IEnumerator CylMove(bool dir)
    {
        if(cylinder == Cylinder.Conveyor_PushCylinder)
        {
            if (dir)
            {
                print("Conveyor_PushCylinder - �۵����Դϴ�...");
                print("Conveyor_PushCylinder - �ڽ��� �ùٸ� ��ġ�� �����մϴ�");
            }
            else
            {
                print("Conveyor_PushCylinder - �ڽ� ������ Ȯ�εǾ����ϴ�...");
                print("Conveyor_PushCylinder - ���� �ڽ� ������ ���� ����ġ�� ���ư��ϴ�.");
            }
        }
        else
        {
            if (dir)
            {
                print("Conveyor_GateCylinder - �۵����Դϴ�...");
                print("Conveyor_GateCylinder - ����� ���� ����Ʈ�� ���ϴ�");
            }
            else
            {
                print("Conveyor_GateCylinder - ��ü ����� �����߽��ϴ�");
                print("Conveyor_GateCylinder - ���� ��ü ������ ���� ����Ʈ�� �ݽ��ϴ�");
            }
        }

        isCylinderMoving = true;
        int justOnce = 1;
        do
        {
            cylinderStatusData.usageCount = cylinderStatusData.usageCount + 1;
            justOnce++;
        }
        while (justOnce == 1);

        cylinderStatusData.operationStatus = true;

        float elapsedTime = 0;
        while (elapsedTime < runTime)
        {
            elapsedTime += Time.deltaTime;

            if (dir)
            {
                MovePositionRod(minPos, maxPos, elapsedTime, runTime);
            }
            else
            {
                MovePositionRod(maxPos, minPos, elapsedTime, runTime);
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }

        isCylinderMoving = false;
        cylinderStatusData.operationStatus = false;
    }

    public void MovePositionRod(Vector3 startPos, Vector3 endPos, float _elapsedtime, float _runtime)
    {
        Vector3 newPos = Vector3.Lerp(startPos, endPos, _elapsedtime / _runtime);//t���� 0(min)~1(max)���� ��ȭ
        transform.localPosition = newPos;
     }
}   
