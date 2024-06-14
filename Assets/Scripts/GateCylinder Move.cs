using UnityEngine;
using System.Threading;
//���۽� �÷��̾ �� �������� �̵�
public class GateCylinder : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 8;
    public Transform ForWardDestination;
    public Transform BackWardDestination;
    public float distanceLimit = 0.5f;
    public Timer timer;
    public CylinderSensor ForWardSensor;//���� ����
    public CylinderSensor BackWardSensor;//���� ����
    public Sensor ObjectSensor;//��ǰ Ȯ�ο�
    public Sensor CloseSensor;

    void Update() //�������� ���ŵɶ� ����Ǵ� �޼��� 0.002~0.004�ʿ� �ѹ��� ����
    {
        Vector3 directon = Vector3.back; //�� ��ġ����  destination������ ����

        if (ObjectSensor.isObjectDetected)
        {
            if (ForWardSensor.isObjectDetected)
            {
                Vector3 dir2Dest = (BackWardDestination.position - transform.position).normalized;
                float fordistance = (BackWardDestination.position - transform.position).magnitude;

                Thread.Sleep(10);
                if (fordistance > distanceLimit)
                {
                    transform.position += dir2Dest * Time.deltaTime * speed;
                    print("Conveyor_GateCylinder - �۵����Դϴ�...");
                    print("Conveyor_GateCylinder - ����� ���� ����Ʈ�� ���ϴ�");
                }
                else
                {
                    ObjectSensor.isObjectDetected = false;
                }
            }
        }
        if (CloseSensor.isObjectDetected)
        {
            ForWardSensor.isObjectDetected = false;
            
            Vector3 backdir2Dest = (ForWardDestination.position - transform.position).normalized;
            float backdistance = (ForWardDestination.position - transform.position).magnitude;
            Thread.Sleep(10);
            if (backdistance > distanceLimit)
            {
                transform.position += backdir2Dest * Time.deltaTime * speed;
                print("Conveyor_GateCylinder - ��ü ����� �����߽��ϴ�");
                print("Conveyor_GateCylinder - ���� ��ü ������ ���� ����Ʈ�� �ݽ��ϴ�");
            }
            else
            {
                CloseSensor.isObjectDetected = false;

            }
        }
    }
}
