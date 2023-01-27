using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace TestModuleViewer
{
    public partial class ViewerCode : Form
    {
        /// <summary>
        /// 세 개 뷰어의 초기 값 값들 json 형식으로 관리
        /// 뷰어 상태 (waiting = 대기 중, reading = 판독 중)
        /// 뷰어 가용 여부 (true = 가능, false = 불능)
        /// </summary>
        static List<ParametersClass> parameterClass = new List<ParametersClass>() {
            new ParametersClass() { ip = "000.00.000.01", pcStatus = "waiting", open = true, option = 1 },
            new ParametersClass() { ip = "000.00.000.02", pcStatus = "waiting", open = true, option = 2 },
            new ParametersClass() { ip = "000.00.000.03", pcStatus = "waiting", open = true, option = 3 }
        };
        static string parameterJson = System.Text.Json.JsonSerializer.Serialize<List<ParametersClass>>(parameterClass, new JsonSerializerOptions() { WriteIndented = true });
        List<ParametersClass> parameters = JsonConvert.DeserializeObject < List<ParametersClass>>(parameterJson);

        static string divisionServerIP = "13.124.14.40"; // ec2 주소 / 분배 시스템 ip, 변동 가능성 있으므로 전역 변수로 지정
        //static string divisionServerIP = "152.67.222.12"; // aws 주소
        static int divisionServerPort = 9002;// 분배 시스템 port
        
        static string xrayMqttIP = "localhost";//Xray와 뷰어 간의 통신을 위한 xray ip 추후 변경 예정
        static int xrayMqttPort = 9003;// 추후 변경 예정

        private System.Windows.Forms.Timer timer = null; // 뷰어는 일정 간격마다 timer로 분배시스템에게 상태 정보 전송
        Mat receiveImage; // 이미지 데이터는 메모리 관리를 위해 전역 변수로 지정

        public ViewerCode()
        {
            Console.WriteLine("Viewer");
            InitializeComponent();

            //뷰어는 일정 간격마다 timer로 분배시스템에게 상태 정보 전송
            timer = new System.Windows.Forms.Timer();
            timer.Tick += Timer_Viewer1_Tick;
            timer.Interval = 5000; // (1000 = 1초)
            timer.Start();
            

            // Xray로 부터 검수 대상 이미지를 mqtt로 받기 위해서 구독(subscribe) 하기 위한 쓰레드를 뷰어 개수만큼 생성
            Thread recieveViewer1 = new Thread(() => RecieveImageFromXrayEquipment(parameters[0].ip, parameters[0].option));
            Thread recieveViewer2 = new Thread(() => RecieveImageFromXrayEquipment(parameters[1].ip, parameters[1].option));
            Thread recieveViewer3 = new Thread(() => RecieveImageFromXrayEquipment(parameters[2].ip, parameters[2].option));
            recieveViewer1.Start();
            recieveViewer2.Start();
            recieveViewer3.Start();
        }
        #region 뷰어에서 분배시스템으로 상태 정보 전송
        /// <summary>
        /// 분배시스템으로 상태 정보 전송 함수, 타이머에 의해 주기적으로 실행
        /// </summary>
        /// <param name="ip">뷰어 ip</param>
        /// <param name="divisionServerPort">분배 시스템 포트</param>
        /// <param name="pcStatus">뷰어 상태(waiting = 대기 중, reading = 판독 중)</param>
        /// <param name="open">뷰어 가용 여부(true = 가능, false = 불능)</param>
        private void SocketSend(string ip, int divisionServerPort, string pcStatus, bool open)
        {
            byte[] receiverBuff = new byte[1000]; // 소켓 핸드 쉐이킹, 리시브 크기
            try
            {
                // 소켓 생성 tcp 통신
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                var ep = new IPEndPoint(IPAddress.Parse(divisionServerIP), divisionServerPort);
                
                string timeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                // json 형식으로 통신
                var jsonData = new JObject
                {
                    { "ip", ip },/// 뷰어 ip
                    { "pcStatus", pcStatus },/// 뷰어 상태 (waiting = 대기 중, reading = 판독 중)
                    { "open", open },/// 뷰어 가용 여부 (true = 가능, false = 불능)
                    { "time", timeNow } /// 전송 시간(24시간 모드, ex. 오후 8시 -> 20시)
                };

                string jsonSerial = JsonConvert.SerializeObject(jsonData, Formatting.None);
                byte[] send_data = Encoding.UTF8.GetBytes(jsonSerial);

                sock.LingerState = new LingerOption(true, 0); // 소켓 닫은 후 TIME_WAIT 삭제
                sock.Connect(ep);
                int n = sock.Receive(receiverBuff, receiverBuff.Length, SocketFlags.None); // 소켓 연결 후 핸드 쉐이킹 
                string data = Encoding.UTF8.GetString(receiverBuff, 0, n); 

                sock.Send(send_data, send_data.Length, SocketFlags.None); 

                sock.Disconnect(true); // ture 소켓 연결 닫은 후 재사용
                sock.Close(); 
                
                // 각각의 뷰어로 출력
                if (ip == parameters[0].ip) { RichTextBox_Viewer1.AppendText("socket\n"); }
                else if (ip == parameters[1].ip) { RichTextBox_Viewer2.AppendText("socket\n"); }
                else if (ip == parameters[2].ip) { RichTextBox_Viewer3.AppendText("socket\n"); }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("서버 접속 불량 재시도..");
            }
        }
        /// <summary>
        /// 뷰어 상태가 변경 됐을 때 소켓 통신하여 뷰어 상태 갱신
        /// 3개의 뷰어를 같은 메서드로 option만 달리하여 사용
        /// </summary>
        /// <param name="pcStatus">뷰어 상태 정보</param>
        /// <param name="open">뷰어 가용 여부</param>
        /// <param name="option">3개의 뷰어 구분(각각 1,2,3)</param>
        private void SocketSendUpdateState(string pcStatus, bool open, int option)
        {
            //  상태 업데이트 "option"으로 3개의 뷰어 구분
            switch (option)
            {
                case 1:
                    PictureViewer1.Image = BitmapConverter.ToBitmap(receiveImage); break;
                case 2:
                    PictureViewer2.Image = BitmapConverter.ToBitmap(receiveImage); break;
                case 3:
                    PictureViewer3.Image = BitmapConverter.ToBitmap(receiveImage); break;
            }
            parameters[option - 1].pcStatus = pcStatus; // 뷰어 상태 정보 갱신
            parameters[option - 1].open = open; 
            SocketSend(parameters[option - 1].ip, divisionServerPort, pcStatus, open);
        }
        /// <summary>
        /// 타이머로 3개의 뷰어 일정 주기로 상태 정보 전송
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Viewer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                SocketSend(parameters[i].ip, divisionServerPort, parameters[i].pcStatus, parameters[i].open);
            }
        }
        #endregion 뷰어에서 분배시스템으로 상태 정보 전송

        #region mqtt 통신 (X-ray 장비에서 뷰어는 이미지 받음, 뷰어에서 X-ray 장비로는 승인(accept) 재확인(re-check) 여부 전송)

        /// <summary>
        /// X-ray 장비로 부터 검수 이미지를 각각의 뷰어가 받음
        /// mqtt 통신으로 subscribe(구독)하는 형태
        /// </summary>
        /// <param name="IP">뷰어 ip</param>
        /// <param name="option">3개의 뷰어 구분(각각 1, 2, 3)</param>
        private void RecieveImageFromXrayEquipment(string IP, int option)
        {
            ManagedMqttClientOptions options = new ManagedMqttClientOptions();
            options.ClientOptions = new MqttClientOptions()
            {
                ClientId = "subscribeImage" + IP ,
                ChannelOptions = new MqttClientTcpOptions
                {
                    Server = xrayMqttIP,
                    Port = xrayMqttPort, 
                }
            };
            options.AutoReconnectDelay = TimeSpan.FromSeconds(1);
            try
            {
                var managedClient = new MqttFactory().CreateManagedMqttClient();// 클라이언트 생성
                // 구독한 토픽(Topic)으로 데이터 받을 경우 아래 리시브 핸들러 실행
                managedClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
                {
                    string jsonSerial = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    var jsonData = JObject.Parse(jsonSerial); // 문자열에서 json으로 파싱

                    string base64 = jsonData.GetValue("base64")?.ToString(); 
                    ///// base64 -> image
                    byte[] imageBytes = Convert.FromBase64String(base64);
                    receiveImage = Mat.FromImageData(imageBytes, ImreadModes.Color);
                    SocketSendUpdateState("reading", false, option); // 해당 뷰어 작업 중으로 상태 변경
                });
                // 3개의 뷰어가 각각의 ip로 토픽을 정하여 구독
                managedClient.SubscribeAsync(new MqttTopicFilter { Topic = IP + " request ", QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce });
                managedClient.StartAsync(options);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /// <summary>
        /// 각각의 뷰어에서 승인(accept) 재확인(re-check) 여부 mqtt로 게시(publish)
        /// </summary>
        /// <param name="ip">뷰어 ip</param>
        /// <param name="acceptOrReCheck">승인 재확인 여부</param>
        private void PublishMqttAfterImageInspection(string ip, string acceptOrReCheck)
        {
            ManagedMqttClientOptions options = new ManagedMqttClientOptions();

            options.ClientOptions = new MqttClientOptions()
            {
                ClientId = "accOrRe",
                ChannelOptions = new MqttClientTcpOptions
                {
                    Server = xrayMqttIP,
                    Port = xrayMqttPort,
                }
            };
            options.AutoReconnectDelay = TimeSpan.FromSeconds(3);

            JObject jsonData = new JObject
            {
                { "acceptOrReCheck", acceptOrReCheck },// 승인은 accecpt, 재확인은 re-check
                { "viewerIP", ip }// 뷰어ip
            };
            string jsonSerial = JsonConvert.SerializeObject(jsonData, Formatting.None);
            byte[] send_data = Encoding.UTF8.GetBytes(jsonSerial);

            try
            {
                var managedClient = new MqttFactory().CreateManagedMqttClient();
                managedClient.StartAsync(options);
                // 3개의 뷰어 각각 ip가 토픽이 됨
                managedClient.PublishAsync(builder => builder.WithTopic(ip+" response ").WithPayload(send_data).WithAtLeastOnceQoS());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
        #endregion mqtt 통신 (X-ray 장비에서 뷰어는 이미지 전송, 뷰어에서 X-ray 장비로는 승인(accept) 재확인(re-check) 여부 전송)
        
        #region 버튼 클릭 관련 메서드 묶음
        /// <summary>
        /// 버튼 클릭(accept, re-check)할 경우 뷰어의 상태가 작업 중(reading)에서 대기 중(waiting)으로 바뀜
        /// 뷰어 가용 여부도 불능(false)에서 가능(true)로 바뀜
        /// </summary>
        /// <param name="accORrech">accept 혹은 re-check</param>
        /// <param name="option">3개의 뷰어(각각 1, 2, 3)</param>
        private void ButtonClickFunc(string accORrech, int option)
        {
            SocketSendUpdateState("waiting", true, option);// 뷰어의 상태 대기 중, 뷰어 가용 여부 true, 해당 뷰어 구분
            PublishMqttAfterImageInspection(parameters[option - 1].ip, accORrech); // 검수 결과 mqtt로 전송
            switch (option)
            {
                // 뷰어는 작업 대기 화면으로 바뀜
                case 1:
                    PictureViewer1.Image = TestModuleViewer.Properties.Resources.작업_대기; 
                    RichTextBox_Viewer1.AppendText("    " + accORrech + "\n"); break;
                case 2:
                    PictureViewer2.Image = TestModuleViewer.Properties.Resources.작업_대기; 
                    RichTextBox_Viewer2.AppendText("    " + accORrech + "\n"); break;
                case 3:
                    PictureViewer3.Image = TestModuleViewer.Properties.Resources.작업_대기; 
                    RichTextBox_Viewer3.AppendText("    " + accORrech + "\n"); break;
            }
        }
        private void accept1_Click(object sender, EventArgs e)
        {
            ButtonClickFunc("accept", 1); // 뷰어 1번 승인 클릭
        }
        private void accept2_Click(object sender, EventArgs e)
        {
            ButtonClickFunc("accept", 2); // 뷰어 2번 승인 클릭
        }
        private void accept3_Click(object sender, EventArgs e)
        {
            ButtonClickFunc("accept", 3); // 뷰어 3번 승인 클릭
        }
        private void reCheck1_Click(object sender, EventArgs e)
        {
            ButtonClickFunc("re-check", 1); // 뷰어 1번 재확인 클릭
        }
        private void reCheck2_Click(object sender, EventArgs e)
        {
            ButtonClickFunc("re-check", 2); // 뷰어 2번 재확인 클릭
        }
        private void reCheck3_Click(object sender, EventArgs e)
        {
            ButtonClickFunc("re-check", 3); // 뷰어 3번 재확인 클릭
        }
        #endregion 버튼 클릭 관련 메서드 묶음


        /// <summary>
        /// 세 개 뷰어의 파라미터 값들 json 형식으로 관리
        /// </summary>
        private class ParametersClass
        {
            public string ip { get; set; }
            public int port { get; set; }
            public string pcStatus { get; set; }
            public bool open { get; set; }
            public int option { get; set; }
        }

    }
    
}

