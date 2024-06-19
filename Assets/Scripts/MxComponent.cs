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

        [Header("상태")]
        public Connection connection = Connection.Disconnected;
        public enum Connection//연결/연결해제상태 열거형
        {
            Connected,
            Disconnected,
        }

        [Header("옵션")]
        public TMP_Text log;
        public DataManager dataManager;
        public static MxComponent instance;
        public Button startBtn;
        public Button stopBtn;
        public Button emergencyBtn;
        public Image connectImage;
        public Image connectImage_UI; //Guest 화면 표시용
        public TMP_Text connectBtnTxt;
        public TMP_Text connectBtnTxt_UI;//Guest 화면 표시용

        [Header("연결_컨베이어")]
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

        [Header("연결_로드시스템")]
        public Sensor endSensor_L;
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

        //조건부로 만들기위한 막만든 변수 //06.19/14:11
        int datasend = 0;
        int ditecd = 0;
        int ditecdar = 0;
        int ditecdal = 0;
        int ditecdgate_B = 0;
        int ditecdclose = 0;
        private void Awake()//인스턴스 지정
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
            mxComponent = new ActUtlType64();                           //mxC 초기설정1
            mxComponent.ActLogicalStationNumber = 1;               //mxC 초기설정2
            log.text = "[PLC] 초기 설정: \n PLC - 연결 해지 상태입니다.";
        }

        public void Update()
        {
            StartCoroutine(UploadDB());
            StartCoroutine(GetTotalDeviceData());                                                     //PLC 각각 지정번호 불러오기
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
            if (arriveSensor_C.isObjectDetected && alignSensor_C.isObjectDetected == false && ditecdar <3) //3회 스캔 / 물품 도착시 스캔//06.19/14:11
            {
                 ditecd = 0;
                 datasend = 0;
                 ditecdar++;
            }
            if (arriveSensor_C.isObjectDetected && alignSensor_C.isObjectDetected && ditecdal <3) //3회 스캔/물품 정렬시 스캔//06.19/14:11
            {  
                ditecd = 0;
                datasend = 0;
                ditecdal ++;                
            }
            if(alignSensor_C.isObjectDetected&&gateCyl_B.isObjectDetected&& ditecdgate_B<3)//3회 스캔  /게이트 실린더 후진시 스캔 //06.19/14:11
            {
                ditecd = 0;
                datasend = 0;
                ditecdgate_B++;
            }
            if (closeSensor_C.isObjectDetected && gateCyl_B.isObjectDetected && ditecdclose < 3)//3회 스캔 /물품 이동확인시 스캔//06.19/14:11
            {
                ditecd = 0;
                datasend = 0;
                ditecdclose++;
            }
        }
        IEnumerator GetTotalDeviceData()                                     //PLC 포트번호 GetDevice로 지정
         {
            if (connection == Connection.Connected)
            {
                if (datasend == 0)//06.19/14:11
                {
                    print("데이터1");
                    short[] xdata = ReadDeviceBlock("X0", 10);
                    short[] ydata = ReadDeviceBlock("Y0", 20);
                    string new_xdata = ConvertDataIntoString(xdata);
                    string new_ydata = ConvertDataIntoString(ydata);

                    conveyor.plcInputValue = new_ydata[0]-48; //Y0

                    pushCylinder.plcInputValue = new_ydata[20] - 48; //Y20

                    gateCylinder.plcInputValue = new_ydata[30] - 48; //Y30

                    arriveSensor_C.plcInputValue = new_xdata[1]-48; //X1
                    alignSensor_C.plcInputValue = new_xdata[2]-48; //X2
                    closeSensor_C.plcInputValue = new_xdata[3] - 48; //X3
                    endSensor_L.plcInputValue = new_xdata[4] - 48; //X4
    
                    xTransfer.plcInputValues[0] = new_ydata[40] - 48; //Y40
                    xTransfer.plcInputValues[1] = new_ydata[41] - 48; //Y41

                    LMTransfer.plcInputValues[0] = new_ydata[50] - 48; //Y50
                    LMTransfer.plcInputValues[1] = new_ydata[51] - 48; //Y51

                    zTransfer.plcInputValues[0] = new_ydata[60] - 48; //Y60
                    zTransfer.plcInputValues[1] = new_ydata[61] - 48; //Y61

                    loadCylinder.plcInputValues[0] = new_ydata[70] - 48; //Y70
                    loadCylinder.plcInputValues[1] = new_ydata[71] - 48; //Y71
                    datasend = 1;//06.19/14:11
                }
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
                    int zeroCnt = 10 - temp2.Length; // 7-> 7개의 0을 newdata에 더해준다
                    for (int j = 0; j < zeroCnt; j++)
                    {
                      new_data += "0";
                     }
                            //000000000+001+000000 = 총 20개의 비트
               }
            }
                    return new_data;
         }

        public void OnConnectPLCBtnClkEvent()                          //PLC 연결 
        {
            switch (connection)
            {
                case Connection.Connected:
                    int returnValue1 = mxComponent.Close();
                    if (returnValue1 == 0)
                    {
                        print("[PLC] PLC 연결이 해지되었습니다.");
                        log.text = "PLC 연결이 해지되었습니다.";
                        connection = Connection.Disconnected;
                    }
                    else
                    {
                        print("[PLC] PLC 연결 해지에 실패했습니다. | returnValue: 0x" + returnValue1.ToString("X"));//16진수로 변경
                        log.text = "PLC 연결 해지에 실패했습니다. | returnValue: 0x" + returnValue1.ToString("X");
                    }
                    break;

                 case Connection.Disconnected:
                    int returnValue2 = mxComponent.Open();                // 정상종료: 0 / 이상종료: 에러코드 반환
                    if (returnValue2 == 0)
                    {
                        print("[PLC] PLC 연결에 성공하였습니다.");
                        log.text = "PLC 연결에 성공하였습니다.";

                        connection = Connection.Connected;                 //초기화
                        StartCoroutine(InitPosition());

                    }
                    else
                    {
                        print($"[PLC] PLC 연결에 실패하였습니다.| returnValue: 0x{returnValue2.ToString("X")}");
                        log.text = "PLC 연결에 실패했습니다. | returnValue: 0x" + returnValue2.ToString("X");
                        // 오류문자 16진수 변환해 출력
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
                    print("[System][PLC] PLC 연결이 해지되었습니다.");
                    log.text = "PLC 연결이 해지되었습니다.";
                    connection = Connection.Disconnected;
                }
                else
                {
                    print("[System][PLC] PLC 연결 해지에 실패했습니다. | returnValue: 0x" + returnValue.ToString("X"));//16진수로 변경
                    log.text = "PLC 연결 해지에 실패했습니다. | returnValue: 0x" + returnValue.ToString("X");
                }
            }
            else
            {
                print("[System][PLC] 현재 PLC 연결 해지상태입니다.");
                log.text = "현재 PLC 연결 해지상태입니다.";
            }
        }*/
            public bool SetDevice(string device, int value)
            {
                if (connection == Connection.Connected)
                {
                   
                    int returnValue = mxComponent.SetDevice(device, value);

                    if (returnValue != 0)
                    {
                    if (ditecd == 0)//06.19/14:11
                    {
                        print(returnValue.ToString("X"));
                        print("데이터 센서");
                        ditecd=1;//06.19/14:11
                    }
                    }
                        

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

                int returnValue = mxComponent.ReadDeviceBlock2(startDeviceName, _blockSize, out devices[0]); //디바이스의 0번째로부터 100개만큼의 사이즈 가져오기

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
                print("[PLC] PLC 연동을 시작합니다.");
                datasend = 0;
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
                print("[PLC] PLC 연동을 중지합니다.");
                datasend = 0;

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
                print("[PLC][Alert] PLC 비상 정지 버튼이 활성화되었습니다.");
                datasend = 0;

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