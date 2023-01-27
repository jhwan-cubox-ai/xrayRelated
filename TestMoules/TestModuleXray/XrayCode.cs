using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Server;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using RestSharp;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TestModuleXray
{

    public partial class XrayCode : Form
    {
        /// <summary>
        /// 변동 가능성 있는 IP 주소나 포트 번호, ID, PW 등의 값 json 형식으로 관리
        /// </summary>
        static ParameterClass parameterClass = new ParameterClass()
        {
            // Xray 이미지 경로 파라미터 
            //xrayImagePaths = @"\\172.16.150.18\3. NIA X-ray data 검수\04. 슈퍼브 S3 업로드용 데이터\20220922_4292\train", 
            // 위 경로 cubox 외부 ip 접근 불가

            // 임의로 이미지 샘플 제공
            xrayImagePaths = System.IO.Directory.GetParent(System.Environment.CurrentDirectory).Parent.FullName + @"\Resources\image sample",


            // HTTP Rest API 통신 파라미터
            aiModelUrl = "http://172.16.150.19:7001", // http://172.16.150.19:7001/swagger/index.html 참고
            divisionSysUrl = "http://172.16.150.19:8090", // http://172.16.150.19:8090/swagger-ui.html

            // MQTT 통신 파라미터
            monitoringServer = "172.16.150.19",
            monitoringPort = 1883,
            localServer = "localhost",
            localPort = 9003,

            // 분배서버 database 파라미터

            //divisionserver = "172.16.150.16",
            //divisionPort = "4306",
            //divisionID = "root",
            //divisionPW = "cubox2022!"
            divisionserver = "13.124.14.40", // 오라클 db 
            divisionPort = "3306",
            divisionID = "admin",
            divisionPW = "cubox2022!"
        };
        static string stringParameterJson = JsonConvert.SerializeObject(parameterClass);
        ParameterClass parameters = JsonConvert.DeserializeObject<ParameterClass>(stringParameterJson);

        string xrayPicturePath;  // xray 이미지가 있는 주소 
        Mat boundingBoxImage; // AI 모델 결과 이미지 변수
        public XrayCode()
        {
            Console.WriteLine("(Central Server)X-ray equipment");
            InitializeComponent();
            Thread broker = new Thread(XrayAndViewersMqttCommunication); // Xray와 뷰어 mqtt로 통신하기 위해서 센트럴 서버(Xray 장비)에서 브로커 생성
            broker.Start();
        }
        #region 1. 촬영 이미지 + Xray 이미지 불러오기 이벤트
        /// <summary>
        /// 특정 폴더에서 Xray 이미지 "자동"으로 불러옴
        /// 특정 폴더 구조를 위한 코드이므로 다른 폴더에서는 사용 불가
        /// </summary>
        private void Button_NextImage_Click(object sender, EventArgs e)
        {
            StepThreePicture.Image = TestModuleXray.Properties.Resources.작업_대기;
            string rootdir = parameters.xrayImagePaths; 
            // 디렉토리 목록 가져오기
            string[] dirs = Directory.GetDirectories(rootdir);
            int numberOfdirs = dirs.GetLength(0);
            Random rand = new Random();
            while (true)
            {
                int index = rand.Next(0, numberOfdirs);
                string selcetedFolder = dirs[index];
                // 파일 목록 가져오기
                string filesPath = Path.Combine(rootdir, selcetedFolder);
                string[] files = Directory.GetFiles(filesPath);
                string realPicturePath = "";
                xrayPicturePath = "";
                // 실제 이미지(JPG)와 Xray 이미지(png) 구분 하여 각각 "step1.", "step2."에 출력
                foreach (string picture in files)
                {
                    if (picture.Contains("0.JPG"))
                    {
                        realPicturePath = Path.Combine(filesPath, picture);
                    }
                    else if (picture.Contains("1.png"))
                    {
                        xrayPicturePath = Path.Combine(filesPath, picture);
                    }
                }
                if (realPicturePath != "" && xrayPicturePath != "")
                {
                    StepOnePicture.Load(realPicturePath);
                    StepTwoPicture.Load(xrayPicturePath);
                    break;
                }
            }

        }
        /// <summary>
        /// 특정 폴더에서 Xray 이미지 "선택"해서 불러옴
        /// 폴더 경로를 바꾸고 싶으면 InitialDirectory 경로 수정 
        /// </summary>
        private void button_WinExplorer_Click(object sender, EventArgs e)
        {
            //디자인 폼에서 생성한 openFileDialog1 속성정의
            //InitialDirectory -> \\172.16.150.18\3. NIA X-ray data 검수\04. 슈퍼브 S3 업로드용 데이터\20220922_4292\train
            //Filter -> Image 파일 (*.jpg, *.png)|*.jpg;*.png|JPG 파일 (*.jpg)|*.jpg|PNG 파일 (*.png)|*.png

            // 경로 변경 230126
            openFileDialog1.InitialDirectory = parameters.xrayImagePaths;

            //OpenFileDialog 이용해서 탐색기 dialog 띄워준다
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StepOnePicture.Image = TestModuleXray.Properties.Resources.작업_대기;

                // 디렉토리 목록 가져오기
                string fileFullName = openFileDialog1.FileName;                             //"파일경로\파일이름.확장자" 저장
                string filePath = fileFullName.Replace(openFileDialog1.SafeFileName, "");   //"파일경로" 저장

                //"step 1. 촬영 이미지" 및 "step 2. x-ray 이미지" 업데이트 함수 업데이트 함수 실행
                if (Step1AndStep2ImageUpdate(fileFullName, filePath))
                {
                    //API 전송 버튼 Enable -> true
                    Button_HttpJson.Enabled = true;
                    Button_HttpForm.Enabled = true;
                }
                else
                {
                    //폴더에 jpg 파일과 png 파일 두종류가 존재하지 않는 경우가 있음.
                    //이 경우 에러 메시지 출력
                    MessageBox.Show("경로에 촬영 이미지와 x-ray 이미지가 존재하지 않습니다.", "이미지 오류");
                    //API 전송 버튼 Enable -> false
                    Button_HttpJson.Enabled = false;
                    Button_HttpForm.Enabled = false;
                }
            }
        }
        /// <summary>
        /// "step 1. 촬영 이미지" 및 "step 2. x-ray 이미지" 업데이트
        /// </summary>
        /// <param name="fileFullName">"파일경로\파일이름.확장자"</param>
        /// <param name="filePath">"파일경로"</param>
        /// <returns>업데이트 성공시 true</returns>
        public bool Step1AndStep2ImageUpdate(string fileFullName, string filePath)
        {
            bool result = true; //반환값
            string realPicturePath = "";    //"step 1. 촬영 이미지" 경로 저장
            xrayPicturePath = "";           //"step 2. x-ray 이미지" 경로 저장

            // 파일 목록 가져오기
            string[] files = Directory.GetFiles(filePath);

            //선택한 파일이 JPG 일 때 "step 1. 촬영 이미지"에 전시
            if (fileFullName.Contains(".JPG") || fileFullName.Contains(".jpg"))
            {
                realPicturePath = fileFullName;
                //선택한 파일이 있는 파일 경로에서 png 사진을 찾아 "step 2. x-ray 이미지"에 전시
                foreach (string picture in files)
                {
                    if (picture.Contains("1.png"))
                    {
                        xrayPicturePath = Path.Combine(filePath, picture);
                        break;
                    }
                }
            }
            //선택한 파일이 PNG 일 때 "step 2. x-ray 이미지"에 전시
            else
            {
                xrayPicturePath = fileFullName;
                //선택한 파일이 있는 파일 경로에서 jpg 사진을 찾아 "step 1. 촬영 이미지"에 전시
                foreach (string picture in files)
                {
                    if (picture.Contains("0.JPG"))
                    {
                        realPicturePath = Path.Combine(filePath, picture);
                        break;
                    }
                }
            }
            //이미지 불러오기 성공
            if (realPicturePath != "" && xrayPicturePath != "")
            {
                StepOnePicture.Load(realPicturePath);
                StepTwoPicture.Load(xrayPicturePath);
            }
            //이미지 불러오기 실패
            else
            {
                result = false;
            }

            return result;
        }
        #endregion 실제 이미지 + Xray 이미지 불러오기 이벤트

        #region 2. Json + Form 버튼 클릭 이벤트
        /// <summary>
        /// Json 버튼 클릭 : 이미지 베이스64로 보내어 Ai 모델 사용 
        /// </summary>
        private void Button_HttpJson_Click(object sender, EventArgs e)
        {
            // "step 3. AI 모델"에 출력을 위한 객체 감지 결과 이미지 담는 변수(boundingBoxImage)
            boundingBoxImage = Cv2.ImRead(xrayPicturePath, ImreadModes.Color);
            StepTwoPicture.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(boundingBoxImage);

            string imageToBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(xrayPicturePath));
            /// restSharp
            RestClient restClient = new RestClient(parameters.aiModelUrl);
            RestRequest request = new RestRequest("/api/ImageTest/image-json", Method.POST);
            // body 생성
            JObject body = new JObject();
            //body.Add("legID", "4"); // 센트럴 서버(x ray 장비)에서 생성
            body.Add("image", imageToBase64);
            request.Parameters.Clear();
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            try
            {
                PublishMonitoringServer("start"); //AI 모델 통신 요청할 때 mqtt 게시
                var response = restClient.Execute(request);
                PublishMonitoringServer("end");  //AI 모델 통신 응답 받을 때 mqtt 게시
                string responseData;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    DrawBoundingBox(response.Content);
                }
                else
                {
                    responseData = ((int)response.StatusCode).ToString();
                    StepFourRichText.AppendText(responseData);
                }
            }
            catch (Exception ee)
            {
                StepFourRichText.AppendText(ee.ToString());
            }
        }
        /// <summary>
        /// Form 버튼 클릭 : 이미지 자체 보내어 Ai 모델 사용 
        /// </summary>
        private void Button_HttpForm_Click(object sender, EventArgs e)
        {
            boundingBoxImage = Cv2.ImRead(xrayPicturePath, ImreadModes.Color);

            var restClient = new RestClient(parameters.aiModelUrl);
            RestRequest request = new RestRequest("/api/ImageTest/image-form", Method.POST);

            request.Parameters.Clear();
            //request에 이미지 파일을 추가해줌
            request.AddFile("image", xrayPicturePath);
            try
            {
                PublishMonitoringServer("start");
                var response = restClient.Execute(request);
                PublishMonitoringServer("end");
                string responseData;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    DrawBoundingBox(response.Content);
                }
                else
                {
                    responseData = ((int)response.StatusCode).ToString();
                    //StepFourRichText.AppendText(responseData);

                }
            }
            catch (Exception ee)
            {
                StepFourRichText.AppendText(ee.ToString());
            }

        }
        /// <summary>
        /// AI 모델 요청 응답할 때 마다 mqtt로 게시(모니터링 서버는 같은 토픽으로 구독하고 있음)
        /// <param name="state">상태값(시작이나 끝)</param>
        /// </summary>
        private void PublishMonitoringServer(string state)
        {
            ManagedMqttClientOptions options = new ManagedMqttClientOptions();

            options.ClientOptions = new MqttClientOptions()
            {
                ClientId = "aimodelapi",
                ChannelOptions = new MqttClientTcpOptions
                {
                    Server = parameters.monitoringServer,
                    Port = parameters.monitoringPort,
                }
            };
            options.AutoReconnectDelay = TimeSpan.FromSeconds(1);
            try
            {
                var managedClient = new MqttFactory().CreateManagedMqttClient();
                managedClient.StartAsync(options);
                managedClient.PublishAsync(builder => builder.WithTopic("xrayid_01").WithPayload(state).WithAtLeastOnceQoS());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /// <summary>
        /// AI 모델에서 http 통신하여 받은 응답에서 결과 추출하여 그려줌
        /// </summary>
        /// <param name="reponseContent">AI모델에서 받은 응답(json)</param>
        private void DrawBoundingBox(string reponseContent)
        {
            JObject ret = JObject.Parse(reponseContent); // JObject 정적 매서드로 string을 JSON형식으로 파싱하기
            JObject ret2 = JObject.Parse(ret["data"].ToString());
            JToken dataList = ret2["data"];
            // boundingBoxImage 에 객체 감지 결과(AI 모델 결과) 넣기
            Mat frame = boundingBoxImage;
            foreach (JToken det in dataList)
            {
                //int label_id = det["label_id"].Value<int>();
                //string ip = "'xray_01'";
                //insertDB(ip, label_id.ToString(), "Null"); // 분배서버에 결과 저장(AI 모델) AI_SDK_API에서 진행
                //float score = det["score"].Value<float>();
                //# bbox
                var bbox = det["bbox"];
                int left = bbox[0].Value<int>(); //left
                int top = bbox[1].Value<int>(); //top
                int right = bbox[2].Value<int>(); //right
                int bottom = bbox[3].Value<int>(); //bottom

                left = Math.Max(0, Math.Min(left, frame.Cols - 1));
                top = Math.Max(0, Math.Min(top, frame.Rows - 1));
                right = Math.Max(0, Math.Min(right, frame.Cols - 1));
                bottom = Math.Max(0, Math.Min(bottom, frame.Rows - 1));

                Rect box = new Rect(left, top, right - left + 1, bottom - top + 1);
                Cv2.Rectangle(frame, box, Scalar.Red, 3, LineTypes.AntiAlias);
            }
            //// 결과 출력
            ////StepThreePicture.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(boundingBoxImage);
            //// 결과 가리기
            StepThreePicture.Image = TestModuleXray.Properties.Resources.POST_완료;
        }
        /// <summary>
        /// 분배서버 DB에 감지된 물품 리스트 저장 // 현재 사용하지 않음 23.01.26
        /// </summary>
        /// <param name="ip">센트럴 서버(Xray 장비) ID 혹은 IP 번호</param>
        /// <param name="label_id">감지된 물품 ID</param>
        /// <param name="product_name">감지된 물품 name(현재는 NULL 추후 업데이트)</param>
        private void insertDB(string ip, string label_id, string product_name)
        {
            string connectString = string.Format("Server={0};Database={1};Uid ={2};Pwd={3};Port={4}", parameters.divisionserver, "divisionserver", parameters.divisionID, parameters.divisionPW, parameters.divisionPort);
            string sql = string.Format("Insert Into xray_analyze_statistics (ip, label_id, product_name) values ({0},{1},{2})", ip, label_id, product_name);

            using (MySqlConnection conn = new MySqlConnection(connectString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion Json + Form 버튼 클릭 이벤트

        #region 3. "판독 뷰어 전송" 버튼 클릭 이벤트
        /// <summary>
        /// 작업 대기 중인 뷰어로 AI 모델 판독 결과 (base 64형태) mqtt로 게시(모니터링 서버와 다른 주소, 뷰어와 통신하시 위함)
        /// </summary>
        private void Button_SendViewer_Click(object sender, EventArgs e)
        {
            string viewerIP = selectIP(); // 작업 대기 중인 뷰어 가져옴
            string imageToBase64;
            // Mat형식의 판독결과 이미지를 base64로 변환
            using (var ms = new MemoryStream())
            {
                using (var bitmap = new Bitmap(BitmapConverter.ToBitmap(boundingBoxImage)))
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    imageToBase64 = Convert.ToBase64String(ms.GetBuffer()); //Get Base64
                }
            }
            //json 생성
            JObject jsonData = new JObject();
            jsonData.Add("base64", imageToBase64); // 이미지
            jsonData.Add("viewerIP", viewerIP); // 보내야할 뷰어 IP
            string jsonSerial = JsonConvert.SerializeObject(jsonData, Formatting.None);
            byte[] send_data = Encoding.UTF8.GetBytes(jsonSerial);

            // mqtt로 퍼블리시
            ManagedMqttClientOptions options = new ManagedMqttClientOptions();

            options.ClientOptions = new MqttClientOptions()
            {
                ClientId = "publishImage",
                ChannelOptions = new MqttClientTcpOptions
                {
                    Server = "localhost", // 하나의 컴퓨터에서 Xray, 뷰어 테스트 하므로 로컬 IP
                    Port = 9003,
                }
            };
            options.AutoReconnectDelay = TimeSpan.FromSeconds(1);
            try
            {
                var managedClient = new MqttFactory().CreateManagedMqttClient();
                managedClient.StartAsync(options);
                // Xray 에서 이미지 보낼 때와 뷰어에서 판독 결과 반환 할 때 구분하기 위해서 토픽 나눔(각각 request, response)
                managedClient.PublishAsync(builder => builder.WithTopic(viewerIP + " request ").WithPayload(send_data).WithAtLeastOnceQoS());

                StepThreePicture.Image = TestModuleXray.Properties.Resources.작업_대기;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
            }
        }
        /// <summary>
        /// 작업 대기 중인 뷰어의 IP를 HTTP GET 으로 가져옴
        /// </summary>
        private string selectIP()
        {
            JObject ret;
            string re = "";
            /// 작업 대기 중인 판독 뷰어 PC
            RestClient restClient = new RestClient(parameters.divisionSysUrl);
            RestRequest request = new RestRequest("/api/pc/free", Method.GET);
            var response = restClient.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ret = JObject.Parse(response.Content);
                re = (string)ret["ip"];
            }
            return re; // ip 반환
        }
        #endregion "판독 뷰어 전송" 버튼 클릭 이벤트



        /// <summary>
        /// Xray와 뷰어 mqtt로 통신하기 위해서 센트럴 서버(Xray 장비)에서 브로커 생성
        /// (뷰어에서 원격 판독 결과 값 받는 역할도 함)
        /// </summary>
        private void XrayAndViewersMqttCommunication()
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
        .WithConnectionBacklog(100)
        .WithDefaultEndpointPort(9003);

            IMqttServer broker = new MqttFactory().CreateMqttServer();

            broker.StartAsync(optionsBuilder.Build());

            broker.UseApplicationMessageReceivedHandler(e =>
            {
                try
                {
                    string topic = e.ApplicationMessage.Topic;
                    if (topic.Contains("response")) // 뷰어에서 원격 판독 결과 값 받음
                    {
                        string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                        JObject ret = JObject.Parse(payload);
                        string result = ret.GetValue("acceptOrReCheck").ToString();
                        string ip = ret.GetValue("viewerIP").ToString();
                        StepFourRichText.AppendText("IP : " + ip + "  reuslt : " + result + "\n");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex);
                }
            });
        }

        /// <summary>
        /// 변동 가능성 있는 IP 주소나 포트 번호, ID, PW 등의 값 관리
        /// </summary>
        public class ParameterClass
        {
            // xray 이미지 경로
            public string xrayImagePaths { get; set; }
            // HTTP Rest API 통신 
            public string aiModelUrl { get; set; }
            public string divisionSysUrl { get; set; }

            // MQTT 통신 
            public string monitoringServer { get; set; }
            public int monitoringPort { get; set; }
            public string localServer { get; set; }
            public int localPort { get; set; }

            // 분배서버
            public string divisionserver { get; set; }
            public string divisionPort { get; set; }
            public string divisionID { get; set; }
            public string divisionPW { get; set; }


        }
    }
}
