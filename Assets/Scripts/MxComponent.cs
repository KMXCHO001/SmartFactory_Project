using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ActUtlType64Lib;
using TMPro;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UI;
using static mxComponent.MxComponent;

namespace mxComponent
{
    public class MxComponent : MonoBehaviour
    {
        ActUtlType64 mxComponent;

        [Header("����")]
        public Connection connection = Connection.Disconnected;
        public enum Connection//����/������������ ������
        {
            Connected,
            Disconnected,
        }

        [Header("�ɼ�")]
        public TMP_Text log;
        public DataManager dataManager;
        public static MxComponent instance;
        public Button startBtn;
        public Button stopBtn;
        public Button emergencyBtn;
        public Image connectImage;
        public Image connectImage_UI; //Guest ȭ�� ǥ�ÿ�
        public TMP_Text connectBtnTxt;
        public TMP_Text connectBtnTxt_UI;//Guest ȭ�� ǥ�ÿ�

        [Header("����_�����̾�")]
        public ConveyorData conveyor;
        public ConveyorCylinder pushCylinder;
        public ConveyorCylinder gateCylinder;
        public Sensor arriveSensor_C;
        public Sensor alignSensor_C;
        public Sensor closeSensor_C;
        public CylinderSensor pushCyl_F;
        public CylinderSensor pushCyl_B;
        public CylinderSensor gateCyl_F;
        public CylinderSensor gateCyl_B;

        [Header("����_�ε�ý���")]
        public DataRead_Cyl xTransfer;
        public DataRead_Cyl LMTransfer;
        public DataRead_Cyl zTransfer;
        public DataRead_Cyl loadCylinder;
        public CylinderSensor xTransfer_F;
        public CylinderSensor yTransfer_F;
        public CylinderSensor zTransfer_F;
        public CylinderSensor xTransfer_B;
        public CylinderSensor yTransfer_B;
        public CylinderSensor zTransfer_B;
        public CylinderSensor loadCylinder_F;
        public CylinderSensor loadCylinder_B;

        private void Awake()//�ν��Ͻ� ����
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        IEnumerator UploadDB()
        {
            dataManager.data.conveyorData = conveyor.conveyorStatusData;
            dataManager.data.pushCylinderData = pushCylinder.cylinderStatusData;
            dataManager.data.pushCylinderSwitchDatas[0] = pushCyl_F.switchSensorData;
            dataManager.data.pushCylinderSwitchDatas[1] = pushCyl_B.switchSensorData;

            dataManager.data.gateCylinderData = gateCylinder.cylinderStatusData;
            dataManager.data.gateCylinderSwitchDatas[0] = gateCyl_F.switchSensorData;
            dataManager.data.gateCylinderSwitchDatas[1] = gateCyl_B.switchSensorData;

            dataManager.data.arriveSensorData = arriveSensor_C.sensorData;
            dataManager.data.alignSensorData = alignSensor_C.sensorData;
            dataManager.data.closeSensorData = closeSensor_C.sensorData;

            dataManager.data.X_TransferData = xTransfer.cylinderStatusData;
            dataManager.data.X_TransferSwitchDatas[0] = xTransfer_F.switchSensorData;
            dataManager.data.X_TransferSwitchDatas[1] = xTransfer_B.switchSensorData;

            dataManager.data.LM_TransferData = LMTransfer.cylinderStatusData;
            dataManager.data.LM_TransferSwitchDatas[0] = yTransfer_F.switchSensorData;
            dataManager.data.LM_TransferSwitchDatas[1] = yTransfer_B.switchSensorData;

            dataManager.data.Z_TransferData = zTransfer.cylinderStatusData;
            dataManager.data.Z_TransferSwitchDatas[0] = zTransfer_F.switchSensorData;
            dataManager.data.Z_TransferSwitchDatas[1] = zTransfer_B.switchSensorData;

            yield return new WaitForSeconds(10);
        }

        void Start()
        {
            mxComponent = new ActUtlType64();                           //mxC �ʱ⼳��1
            mxComponent.ActLogicalStationNumber = 1;               //mxC �ʱ⼳��2
            log.text = "[System][PLC] �ʱ� ����: \n PLC - ���� ���� �����Դϴ�.";
        }

        public void Update()
        {
            StartCoroutine(UploadDB());
            StartCoroutine(GetTotalDeviceData());                                                     //PLC ���� ������ȣ �ҷ�����
            if (connection == Connection.Connected)
            {
                connectBtnTxt.text = "CONNECTED";
                connectBtnTxt_UI.text = "CONNECTED";
                connectImage.color = Color.green;
                connectImage_UI.color = Color.green;
            }
            else
            {
                connectBtnTxt.text = "DISCONNECTED";
                connectBtnTxt_UI.text = "DISCONNECTED";
                connectImage.color = Color.red;
                connectImage_UI.color = Color.red;
            }
        }
        IEnumerator GetTotalDeviceData()                                     //PLC ��Ʈ��ȣ GetDevice�� ����
         {
            if (connection == Connection.Connected)
            {
                short[] xdata = ReadDeviceBlock("X0", 10);
                short[] ydata = ReadDeviceBlock("Y0", 20); 
                string new_xdata = ConvertDataIntoString(xdata);
                string new_ydata = ConvertDataIntoString(ydata);

                conveyor.plcInputValue = new_ydata[0]-48; //Y0

                pushCylinder.plcInputValue = new_ydata[20] - 48; //Y20

                gateCylinder.plcInputValue = new_ydata[31] - 48; //Y31

                arriveSensor_C.plcInputValue = new_xdata[1]-48; //X1
                alignSensor_C.plcInputValue = new_xdata[2]-48; //X2
                closeSensor_C.plcInputValue = new_xdata[3] - 48; //X3

                xTransfer.plcInputValues[0] = new_ydata[40] - 48; //Y40
                xTransfer.plcInputValues[1] = new_ydata[41] - 48; //Y41

                LMTransfer.plcInputValues[0] = new_ydata[50] - 48; //Y50
                LMTransfer.plcInputValues[1] = new_ydata[51] - 48; //Y51

                zTransfer.plcInputValues[0] = new_ydata[60] - 48; //Y60
                zTransfer.plcInputValues[1] = new_ydata[61] - 48; //Y61

                loadCylinder.plcInputValues[0] = new_ydata[70] - 48; //Y70
                loadCylinder.plcInputValues[1] = new_ydata[71] - 48 ; //Y71
            }
            yield return new WaitForSeconds(1);
          }
         string ConvertDataIntoString(short[] data)
         {
            string new_data = "";
            for (int i = 0; i < data.Length; i++)
             {
              if (data[i] == 0)
              {
                new_data += "0000000000";
                continue;
               }
               string temp = Convert.ToString(data[i], 2); //100
               string temp2 = new string(temp.Reverse().ToArray()); //100->001
               new_data += temp2; //000000000 +001

               if (temp2.Length < 10)
               {
                    int zeroCnt = 10 - temp2.Length; // 7-> 7���� 0�� newdata�� �����ش�
                    for (int j = 0; j < zeroCnt; j++)
                    {
                      new_data += "0";
                     }
                            //000000000+001+000000 = �� 20���� ��Ʈ
               }
            }
                    return new_data;
         }

        public void OnConnectPLCBtnClkEvent()                          //PLC ���� 
        {
            switch (connection)
            {
                case Connection.Connected:
                    int returnValue1 = mxComponent.Close();
                    if (returnValue1 == 0)
                    {
                        print("[System][PLC] PLC ������ �����Ǿ����ϴ�.");
                        log.text = "PLC ������ �����Ǿ����ϴ�.";
                        connection = Connection.Disconnected;
                    }
                    else
                    {
                        print("[System][PLC] PLC ���� ������ �����߽��ϴ�. | returnValue: 0x" + returnValue1.ToString("X"));//16������ ����
                        log.text = "PLC ���� ������ �����߽��ϴ�. | returnValue: 0x" + returnValue1.ToString("X");
                    }
                    break;

                 case Connection.Disconnected:
                    int returnValue2 = mxComponent.Open();                // ��������: 0 / �̻�����: �����ڵ� ��ȯ
                    if (returnValue2 == 0)
                    {
                        print("[System][PLC] PLC ���ῡ �����Ͽ����ϴ�.");
                        log.text = "PLC ���ῡ �����Ͽ����ϴ�.";

                        connection = Connection.Connected;                 //�ʱ�ȭ
                        StartCoroutine(InitPosition());

                    }
                    else
                    {
                        print($"[System][PLC] PLC ���ῡ �����Ͽ����ϴ�.| returnValue: 0x{returnValue2.ToString("X")}");
                        log.text = "PLC ���ῡ �����߽��ϴ�. | returnValue: 0x" + returnValue2.ToString("X");
                        // �������� 16���� ��ȯ�� ���
                    }
                    break;
            }
        }
        /*public void OnDisConnectPLCBtnClkEvent()
        {
            if (connection == Connection.Connected)
            {
                int returnValue = mxComponent.Close();
                if (returnValue == 0)
                {
                    print("[System][PLC] PLC ������ �����Ǿ����ϴ�.");
                    log.text = "PLC ������ �����Ǿ����ϴ�.";
                    connection = Connection.Disconnected;
                }
                else
                {
                    print("[System][PLC] PLC ���� ������ �����߽��ϴ�. | returnValue: 0x" + returnValue.ToString("X"));//16������ ����
                    log.text = "PLC ���� ������ �����߽��ϴ�. | returnValue: 0x" + returnValue.ToString("X");
                }
            }
            else
            {
                print("[System][PLC] ���� PLC ���� ���������Դϴ�.");
                log.text = "���� PLC ���� ���������Դϴ�.";
            }
        }*/
            public bool SetDevice(string device, int value)
            {
                if (connection == Connection.Connected)
                {
                    int returnValue = mxComponent.SetDevice(device, value);

                    if (returnValue != 0)
                        print(returnValue.ToString("X"));

                    return true;
                }
                else
                    return false;
            }
        short[] ReadDeviceBlock(string startDeviceName, int _blockSize)
        {
            if (connection == Connection.Connected)
            {
                short[] devices = new short[_blockSize];

                int returnValue = mxComponent.ReadDeviceBlock2(startDeviceName, _blockSize, out devices[0]); //����̽��� 0��°�κ��� 100����ŭ�� ������ ��������

                if (returnValue != 0)
                    print(returnValue.ToString("X"));

                return devices;
            }
            else
                return null;
        }
        public void OnStartPLCBtnClkEvent()
            {
                if (connection == Connection.Connected)
                {
                    startBtn.image.color = Color.green;
                stopBtn.image.color = Color.white;

                    SetDevice("X0", 1);
                print("[System][PLC] PLC ������ �����մϴ�.");
                }
                else
                {
                    SetDevice("X0", 0);
                }
            }
        public void OnStopPLCBtnClkEvent()
        {
            if (connection == Connection.Connected)
            {
            startBtn.image.color = Color.white;
            stopBtn.image.color = Color.red;
            SetDevice("X5", 1);
                print("[System][PLC] PLC ������ �����մϴ�.");
            }
            else
            {
             SetDevice("X5", 0);
            }
         }
        public void OnEmergencyStopBtnClkEvent()
        {
            if (connection == Connection.Connected)
            {
                startBtn.image.color = Color.white;
                stopBtn.image.color = Color.red;
                emergencyBtn.image.color = Color.red;

                SetDevice("X99", 1);
                print("[System][PLC][Alert] PLC ��� ���� ��ư�� Ȱ��ȭ�Ǿ����ϴ�.");
            }
            else
            {
                SetDevice("X99", 0);
            }
        }
        IEnumerator InitPosition()
        {
            SetDevice(pushCyl_F.plcAddress, pushCyl_F.plcInputValue);
            SetDevice(pushCyl_B.plcAddress, pushCyl_B.plcInputValue);

            SetDevice(gateCyl_F.plcAddress, gateCyl_F.plcInputValue);
            SetDevice(gateCyl_B.plcAddress, gateCyl_B.plcInputValue);

            SetDevice(xTransfer_F.plcAddress, xTransfer_F.plcInputValue);
            SetDevice(yTransfer_F.plcAddress , yTransfer_F.plcInputValue);
            SetDevice(zTransfer_F.plcAddress, zTransfer_F.plcInputValue);
            SetDevice(loadCylinder_F.plcAddress, loadCylinder_F.plcInputValue);

            SetDevice(xTransfer_B.plcAddress , xTransfer_B.plcInputValue);
            SetDevice(yTransfer_B.plcAddress, yTransfer_B.plcInputValue);
            SetDevice(zTransfer_B.plcAddress,zTransfer_B.plcInputValue);
            SetDevice(loadCylinder_B.plcAddress, loadCylinder_B.plcInputValue);

            yield return new WaitForSeconds(1);
        }
    }
}