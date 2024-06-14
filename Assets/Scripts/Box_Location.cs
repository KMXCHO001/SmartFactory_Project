using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace box_Location
{
    public class Box_Location : MonoBehaviour
    {
        int box2Mode;

        bool isbox2;
        bool isRotate;

        public Box_Location instance;
        float[] box1Size;
        float[] box2Size;
        float box1Count;
        float box2Count;
        public float maxMoveBelt;
        public float ConveyorSpeed;

        public Transform box1_Origin;
        public Transform box2Origin_W;
        public Transform box2Origin_L;
        public Transform belt_Target;
        public Sensor arriveSensor;
        public GameObject box1EndSensor;
        public Transform XRotate;
        Quaternion rotOrigin;
        Quaternion rot90Degrees;
        Quaternion rot0Degrees;

        Vector3 nowPos;
        Vector3 target1Pos;
        Vector3 target2Pos;
        Vector3 robotTarget;
        Vector3 LM_Origin;
        Vector3 X_Origin;
        Vector3 Z_Origin;
        Vector3 Load_Origin;
        Vector3 moveBoxTarget;
        Vector3 box1_deltaPos;
        Vector3 box2_deltaPos;
        public loadBox loadCheck;

        public Transform LMTransfer;
        public Transform X_Transfer;
        public Transform Z_Transfer;
        public Transform Load_Transfer;

        public GameObject LoadFixedPart;
        public GameObject Belt;

        public TMP_Text box1CountDisplay;
        public TMP_Text box2CountDisplay;

        bool isTransfer;

        void Start()
        {
            isTransfer = true;

            box1Size = new float[] { 0.22f, 0.19f, 0.09f };
            box2Size = new float[] { 0.271f, 0.181f, 0.0151f };

            nowPos = new Vector3();
            target1Pos = new Vector3();
            target2Pos = new Vector3();
            moveBoxTarget = new Vector3();
            box1_deltaPos = Vector3.zero;

            LM_Origin = LMTransfer.localPosition;
            X_Origin = X_Transfer.localPosition;
            Z_Origin = Z_Transfer.localPosition;
            Load_Origin = Load_Transfer.localPosition;

            Debug.Log(ToString(Z_Origin));
        }

        public void Update()
        {
            box1CountDisplay.text = box1Count.ToString();
            box2CountDisplay.text = box2Count.ToString();
        }

        public void LMLoadBtnClkEvnt()
        {
            robotTarget = Vector3.zero;
            Vector3 temp = LM_Origin;
            temp.z -= moveBoxTarget.x;
            robotTarget = temp;
            //Debug.Log($"LMLoadBtnClkEvnt - Origin: {ToString(LM_Origin)}, Target: {ToString(robotTarget)}");
            StartCoroutine(CoMoveLMCylinder(LM_Origin, robotTarget, 2));
        }

        public void LMOriginBtnClkEvnt()
        {
            //Debug.Log($"LMOriginBtnClkEvnt - Origin: {ToString(LM_Origin)}, Current Position: {ToString(LMTransfer.localPosition)}");
            robotTarget = LM_Origin;
            StartCoroutine(CoMoveLMCylinder(LMTransfer.localPosition, LM_Origin, 2));
        }

        public void XLoadBtnClkEvnt()
        {
            robotTarget = X_Origin;
            robotTarget.x -= moveBoxTarget.y;
            // Debug.Log($"XLoadBtnClkEvnt - Origin: {ToString(X_Origin)}, Target: {ToString(robotTarget)}");
            StartCoroutine(CoMoveXCylinder(X_Origin, robotTarget, 2));
        }

        public void XOriginBtnClkEvnt()
        {
            // Debug.Log($"XOriginBtnClkEvnt - Origin: {ToString(X_Origin)}, Current Position: {ToString(X_Transfer.localPosition)}");
            robotTarget = X_Origin;
            StartCoroutine(CoMoveXCylinder(X_Transfer.localPosition, X_Origin, 2));
        }

        public void ZLoadBtnClkEvnt()
        {
            // Debug.Log($"ZLoadBtnClkEvnt - Origin: {ToString(Z_Origin)}, Target: {ToString(robotTarget)}");
            robotTarget = Z_Origin;
            robotTarget.y -= moveBoxTarget.z;

            StartCoroutine(CoMoveZCylinder(Z_Origin, robotTarget, 2));
        }

        public void ZOriginBtnClkEvnt()
        {
            // Debug.Log($"ZOriginBtnClkEvnt - Origin: {ToString(Z_Origin)}, Current Position: {ToString(Z_Transfer.localPosition)}");
            robotTarget = Z_Origin;
            StartCoroutine(CoMoveZCylinder(Z_Transfer.localPosition, Z_Origin, 2));
        }

        public void CylinderForwardBtnClkEvnt()
        {
            robotTarget = Load_Origin;
            robotTarget.x = -0.195f;
            if (box2Count % 24 % 10 == 4)
                isRotate = false;
            else if (box2Count % 24 % 10 == 0)
                isRotate = true;
            StartCoroutine(CoMoveLoadCylinder(Load_Origin, robotTarget, 0.5f));
        }

        public void CylinderBackwardBtnClkEvnt()
        {
            robotTarget = Load_Origin;
            robotTarget.x = -0.195f;
            StartCoroutine(CoMoveLoadCylinder(robotTarget, Load_Origin, 0.5f));
            robotTarget = Vector3.zero;

            if (loadCheck.isBoxLoading[0])
            {
                GameObject box1 = GameObject.FindGameObjectWithTag("Box1");
                box1.transform.SetParent(null);
                Rigidbody rb = box1.GetComponent<Rigidbody>();
                ;
                rb.isKinematic = true;
                StartCoroutine(Timer(rb, 0.5f));
                box1.tag = "Untagged";
                loadCheck.isBoxLoading[0] = false;
            }
            else if (loadCheck.isBoxLoading[1])
            {
                GameObject box2 = GameObject.FindGameObjectWithTag("Box2");
                box2.transform.SetParent(null);
                Rigidbody rb = box2.GetComponent<Rigidbody>();

                rb.isKinematic = true;
                StartCoroutine(Timer(rb, 0.5f));
                box2.tag = "Untagged";
                loadCheck.isBoxLoading[1] = false;
            }
        }
        private IEnumerator Timer(Rigidbody rb, float delay)
        {
            yield return new WaitForSeconds(delay);
            rb.isKinematic = false;
        }

        public void beltOnBtnClkEvnt()
        {
            Vector3 belt_origin = Belt.transform.localPosition;
            Vector3 belt_target = belt_origin;
            belt_target.x = belt_Target.localPosition.x;
            StartCoroutine(CoMoveBelt(belt_origin, belt_target, ConveyorSpeed));
        }

        public Vector3 box1TargetTrans(Vector3 _box1Target)
        {
            box1Count += 1;
            _box1Target.y -= box1Size[1] + 0.005f;
            if (box1Count % 25 == 0)
            {
                _box1Target.y = 0;
                _box1Target.x = 0;
                _box1Target.z -= box1Size[2];
            }
            else if (box1Count % 5 == 0)
            {
                _box1Target.y = 0;
                _box1Target.x -= box1Size[0];
            }
            return _box1Target;
        }

        private Vector3 transAxis(Vector3 _targetPos)
        {
            return new Vector3(_targetPos.z, _targetPos.x, _targetPos.y);
        }

        private string ToString(Vector3 v)
        {
            return string.Format("targetPos (X: {0:F7}, Y: {1:F7}, Z: {2:F7})", v.x, v.y, v.z);
        }
        public Vector3 box2TargetTrans(Vector3 _box2Target)
        {

            float floorBoxEA;
            floorBoxEA = box2Count % 24;//������ �ڽ� ���� 24����
            _box2Target.y -= box2Size[0] + 0.005f;
            if (floorBoxEA == 1)
            {

                _box2Target.x = 0;
                _box2Target.y = 0;
                _box2Target.z -= box2Size[2];// XY ��ġ �ʱ�ȭ �� Z�� ��ĭ �̵�
                if (box2Count == 1)
                    _box2Target = Vector3.zero;
            }
            else if (floorBoxEA % 10 == 1 && floorBoxEA != 1)
            {
                box2Mode = 0;
                switchMode(box2Mode);
                _box2Target.y = 0;
                _box2Target.x -= box2Size[0] + 0.005f;
                print("11or 21���϶�");
            }
            else if (floorBoxEA % 10 == 5)
            {
                print("5,15���϶�");
                box2Mode = 1;
                switchMode(box2Mode);
                _box2Target.y = 0;
                _box2Target.x -= box2Size[0] + 0.005f;
            }
            print(box2Mode + "&" + isRotate);
            return _box2Target;
        }
        public void switchMode(int mode)
        {

            switch (mode)
            {
                case 0:
                    isRotate = true;
                    box2Size[0] = 0.27f;
                    box2Size[1] = 0.18f;

                    break;
                case 1:
                    isRotate = false;
                    box2Size[0] = 0.18f;
                    box2Size[1] = 0.27f;
                    break;
            }
        }

        public void XRotateBtnClkEvnt()
        {
            StartCoroutine(CoRotateCylinder(rot0Degrees, rot90Degrees, 1f));
        }

        public void XRotateOriginBtnClkEvnt()
        {

            box2Count = 24;

            StartCoroutine(CoRotateCylinder(rot90Degrees, rot0Degrees, 1f));

        }

        IEnumerator CoRotateCylinder(Quaternion _origin, Quaternion _rot, float rotateTime)
        {
            if (isRotate && isbox2)
            {
                float elapsedTime = 0f;
                while (elapsedTime < rotateTime)
                {
                    elapsedTime += Time.deltaTime;
                    float t = Mathf.SmoothStep(0f, 1f, elapsedTime / rotateTime);
                    XRotate.rotation = Quaternion.Lerp(_origin, _rot, t);
                    yield return null;
                }
                XRotate.rotation = _rot;
                if (loadCheck.isBoxLoading[1])
                {
                    GameObject gameobject = GameObject.FindGameObjectWithTag("Box2");
                    gameObject.transform.SetParent(null);


                    nowPos = gameobject.transform.position;


                    target2Pos = nowPos - box2Origin_W.position;
                    print("���� �缳��" + ToString(target2Pos));
                    target2Pos = transAxis(target2Pos);
                    box2_deltaPos = box2TargetTrans(box2_deltaPos);
                    target2Pos += box2_deltaPos;
                    print("��ǥ�缳��" + ToString(target2Pos));
                    moveBoxTarget = target2Pos;

                    Debug.Log($"box2 ���� ��ġ ��� ��ȹ �Ϸ� : {ToString(box2_deltaPos)}");
                    gameobject.transform.SetParent(LoadFixedPart.transform);
                }
            }
            yield return null;
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
            LMTransfer.localPosition = _targetPos;
            if (_originPos == LM_Origin)
            {
                print("LoadSystem_LMTransfer - ������������ �̵��մϴ�...");
            }
            else
            {
                print("LoadSystem_LMTransfer - ����������� �̵��մϴ�...");
            }
            LMTransfer.GetComponent<DataRead_Cyl>().cylinderStatusData.usageCount += 1;
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
            X_Transfer.localPosition = _targetPos;
            LMTransfer.GetComponent<DataRead_Cyl>().cylinderStatusData.usageCount += 1;
            if (_originPos == X_Origin)
            {
                print("LoadSystem_XTransfer - ������������ �̵��մϴ�...");
            }
            else
            {
                print("LoadSystem_XTransfer - ����������� �̵��մϴ�...");
            }

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
            Z_Transfer.localPosition = _targetPos;
            LMTransfer.GetComponent<DataRead_Cyl>().cylinderStatusData.usageCount += 1;
            if (_originPos == Z_Origin)
            {
                print("LoadSystem_ZTransfer - ������������ �̵��մϴ�...");
            }
            else
            {
                print("LoadSystem_ZTransfer - ����������� �̵��մϴ�...");
            }
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
            Load_Transfer.localPosition = _targetPos;
            LMTransfer.GetComponent<DataRead_Cyl>().cylinderStatusData.usageCount += 1;
            if (_originPos == Load_Origin)
            {
                print("LoadSystem_Load�Ǹ��� - ���� ��� �����Դϴ�...");
            }
            else
            {
                print("LoadSystem_Load�Ǹ��� - ���� �Ϸ� �����Դϴ�...");
            }
        }

        IEnumerator CoMoveBelt(Vector3 _originPos, Vector3 _targetPos, float movingTime)
        {
            int justonce = 1;
            float elapsedTime = 0f;
            while (elapsedTime < movingTime)
            {
                float t = elapsedTime / movingTime;
                Belt.transform.localPosition = Vector3.Lerp(_originPos, _targetPos, t);
                elapsedTime += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
                if (arriveSensor.isObjectDetected && justonce == 1)
                {
                    justonce = 0;
                    yield return new WaitForSeconds(5.3f);
                }
            }
            Belt.transform.localPosition = _originPos;

            if (loadCheck.isBoxLoading[0])
            {
                nowPos = GameObject.FindGameObjectWithTag("Box1").transform.position;
                target1Pos = nowPos - box1_Origin.position;
                target1Pos = transAxis(target1Pos);
                target1Pos += box1_deltaPos;
                moveBoxTarget = target1Pos;
                box1_deltaPos = box1TargetTrans(box1_deltaPos);
                // Debug.Log($"box1 ���� ��ġ ��� ��ȹ �Ϸ� : {ToString(box1_deltaPos)}");
            }
            else if (loadCheck.isBoxLoading[1])
            {
                isbox2 = true;


                box2Count++;
                print(box2Count);
                GameObject gameobject = GameObject.FindGameObjectWithTag("Box2");


                nowPos = gameobject.transform.position;
                if (!isRotate)
                {
                    box2_deltaPos = box2TargetTrans(box2_deltaPos);
                    target2Pos = nowPos - box2Origin_L.position;
                    target2Pos = transAxis(target2Pos);
                    target2Pos += box2_deltaPos;
                    moveBoxTarget = target2Pos;
                }
                Debug.Log($"box2 ���� ��ġ ��� ��ȹ �Ϸ� : {ToString(box2_deltaPos)}");
            }
        }
    }
}