using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using System.Text;
using Java.Lang;

namespace CoeverOpen.Droid
{
    [Activity(Label = "CoeverOpen", Icon = "@mipmap/open_door", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            //-------------------------------------------------------------------------------------------------- 버튼 tab 이벤트에 추가
            #region ○ 로그인버튼 눌러서 문열기 테스트용 
            var mqtt = new CoM2Mqtt("m.coever.co.kr", 11883, false, null, null, 0);
            MqttClientSetting();

            mqtt.Connect(MQTT_CLIENT_ID, null, null, false, MQTT_KEEP_ALIVE_PERIOD);
            mqtt.Publish("/event/c/OpenDoor/a", Encoding.UTF8.GetBytes("Tester"));
            #endregion
            var activity = (Activity)this;
            activity.FinishAffinity();
            //JavaSystem.Exit(0);
            //LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }



        //-------------------------------------------------------------------------------------------------- 맨 위 변수선언부분 추가
        #region ○ MQTT 변수
        private string MQTT_CLIENT_ID;
        private const ushort MQTT_KEEP_ALIVE_PERIOD = 120;//연결 살아있는지 체크하는 주기
        #endregion

        //-------------------------------------------------------------------------------------------------- Method부분
        #region ○ MQTT
        private void MqttClientSetting()
        {
            MQTT_CLIENT_ID = Guid.NewGuid().ToString();

        }
        private bool Subscribe(CoM2Mqtt Mqtt, string[] _topic, byte[] _qos_level)
        {
            bool _IsSuccess = false;
            for (int i = 0; i < _topic.Length; i++)
            {
                //Mqtt 설정이 안되어있을 경우
                if (Mqtt == null) _IsSuccess = false;
                //토픽이 없을경우
                if (_topic[i] == "") _IsSuccess = false;

                //QoS 레벨은 0,1,2레벨만 존재
                if (_qos_level[i] < 0 || _qos_level[i] > 2)
                    return false;
                try
                {
                    //해당 토픽에 해당 qos레벨로 구독
                    Mqtt.Subscribe(
                        _topic,
                        _qos_level
                        );
                    _IsSuccess = true;
                }
                catch (System.Exception ex)
                {
                    _IsSuccess = false;
                }
            }
            return _IsSuccess;
        }
        private bool UnSubscribe(CoM2Mqtt Mqtt, string[] _topic)
        {
            bool _IsSuccess = false;
            for (int i = 0; i < _topic.Length; i++)
            {
                //Mqtt 설정이 안되어있을 경우
                if (Mqtt == null) _IsSuccess = false;
                //토픽이 없을경우
                if (_topic[i] == "") _IsSuccess = false;

                try
                {
                    //해당 토픽에 해당 qos레벨로 구독
                    Mqtt.Unsubscribe(
                        _topic
                        );
                    _IsSuccess = true;
                }
                catch (System.Exception ex)
                {
                    _IsSuccess = false;
                }
            }
            return _IsSuccess;
        }

        #endregion

    }
}