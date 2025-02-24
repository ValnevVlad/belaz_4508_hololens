/*
The MIT License (MIT)

Copyright (c) 2018 Giovanni Paolo Vigano'

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using uPLibrary.Networking.M2Mqtt.Messages;
using TMPro;
using MixedReality.Toolkit.UX;
using Slider = MixedReality.Toolkit.UX.Slider;

/// <summary>
/// Examples for the M2MQTT library (https://github.com/eclipse/paho.mqtt.m2mqtt),
/// </summary>
namespace M2MqttUnity.Examples
{
    /// <summary>
    /// Script for testing M2MQTT with a Unity UI
    /// </summary>
    public class M2MqttUnity_ScadaBelaz : M2MqttUnityClient
    {
        [SerializeField] GameObject Motor;
        [SerializeField] Material dissolveMat;

        //[Tooltip("Текст для SM_SHE topic")]
        //[SerializeField] ToolTip SM_SHE_current1;
        //[SerializeField] ToolTip SM_SHE_voltage1;
        //[SerializeField] ToolTip SM_SHE_powerFactory;
        //[SerializeField] TextMeshPro SM_SHE_quality;
        //[SerializeField] TextMeshPro SM_SHE_timestamp;

        //[Tooltip("Текст для Dynsim topic")]
        //[SerializeField] TextMeshPro Dynsim_id;
        //[SerializeField] TextMeshPro Dynsim_value;
        //[SerializeField] TextMeshPro Dynsim_quality;
        //[SerializeField] TextMeshPro Dynsim_timestamp;





        [Tooltip("Set this to true to perform a testing cycle automatically on startup")]
        public bool autoTest = false;
        [Header("User Interface")]
        public InputField consoleInputField;
        public Toggle encryptedToggle;
        public InputField addressInputField;
        public InputField portInputField;
        public Button connectButton;
        public Button disconnectButton;
        public Button testPublishButton;
        public Button clearButton;

        private List<string> eventMessages = new List<string>();
        private bool updateUI = false;

        //For parsingData
        //private string msg_id;
        //private string msg_value;
        //private string msg_quality;
        //private string msg_timestamp;


        //For apps
        [Tooltip("MQTT Gameobject1")]
        [SerializeField] TextMeshPro S2_W;
        [SerializeField] TextMeshPro S5_W;
        [SerializeField] TextMeshPro S7_T;
        [SerializeField] TextMeshPro S9_T;
        [SerializeField] TextMeshPro S11_W;
        [SerializeField] TextMeshPro SRC1_PB;
        [SerializeField] TextMeshPro SRC1_TB;
        [SerializeField] TextMeshPro SRC2_PB;
        [SerializeField] TextMeshPro V2_FLASH_VF;
        [SerializeField] TextMeshPro V2_L;
        [SerializeField] TextMeshPro V2_P;
        [SerializeField] TextMeshPro STMDMD;
        [SerializeField] TextMeshPro LVLSP;
        [SerializeField] TextMeshPro MASTER1_B_OUT0;
        [SerializeField] TextMeshPro MASTER1_B_OUT1;


        //For apps
        //[Tooltip("MQTT Gameobject2")]
        //[SerializeField] TextMeshPro S2_W2;
        //[SerializeField] TextMeshPro S5_W2;
        //[SerializeField] TextMeshPro S7_T2;
        //[SerializeField] TextMeshPro S9_T2;
        //[SerializeField] TextMeshPro S11_W2;
        //[SerializeField] TextMeshPro SRC1_PB2;
        //[SerializeField] TextMeshPro SRC1_TB2;
        //[SerializeField] TextMeshPro SRC2_PB2;
        //[SerializeField] TextMeshPro V2_FLASH_VF2;
        //[SerializeField] TextMeshPro V2_L2;
        //[SerializeField] TextMeshPro V2_P2;



        ////For apps
        //[Tooltip("MQTT Gameobject3")]
        //[SerializeField] TextMeshPro S2_W3;
        //[SerializeField] TextMeshPro S5_W3;
        //[SerializeField] TextMeshPro S7_T3;
        //[SerializeField] TextMeshPro S9_T3;
        //[SerializeField] TextMeshPro S11_W3;
        //[SerializeField] TextMeshPro SRC1_PB3;
        //[SerializeField] TextMeshPro SRC1_TB3;
        //[SerializeField] TextMeshPro SRC2_PB3;
        //[SerializeField] TextMeshPro V2_FLASH_VF3;
        //[SerializeField] TextMeshPro V2_L3;
        //[SerializeField] TextMeshPro V2_P3;


        //For control
        [SerializeField] GameObject SliderControl;
        //[SerializeField] PinchSlider LVLSP_SliderControl;
        //[SerializeField] PinchSlider B0_SliderControl;
        //[SerializeField] PinchSlider B1_SliderControl;

        private int value_to_write;
        private string up2_32;
        private string down11_32;
        private string main4_32;

        //private void Start()
        //{
            
        //}
        public void TestPublish()
        {
            client.Publish("scada_belaz", System.Text.Encoding.UTF8.GetBytes("Test message"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("Test message published");
            AddUiMessage("Test message published.");
        }

        public void SetBrokerAddress(string brokerAddress)
        {
            if (addressInputField && !updateUI)
            {
                this.brokerAddress = brokerAddress;
            }
        }

        public void SetBrokerPort(string brokerPort)
        {
            if (portInputField && !updateUI)
            {
                int.TryParse(brokerPort, out this.brokerPort);
            }
        }

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }


        public void SetUiMessage(string msg)
        {
            if (consoleInputField != null)
            {
                consoleInputField.text = msg;
                updateUI = true;
            }
        }

        public void AddUiMessage(string msg)
        {
            if (consoleInputField != null)
            {
                consoleInputField.text += msg + "\n";
                updateUI = true;
            }
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            SetUiMessage("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
            //debugText.text = "Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n";
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            SetUiMessage("Connected to broker on " + brokerAddress + "\n");
            //debugText.text = "Connected to broker on " + brokerAddress + "\n";

            if (autoTest)
            {
                TestPublish();
            }
        }

        protected override void SubscribeTopics()
        {
            //client.Subscribe(new string[] { "SM_SHE" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            //client.Subscribe(new string[] { "iot_aveva" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "unity_belaz" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(new string[] { "unity_belaz" });
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            AddUiMessage("CONNECTION FAILED! " + errorMessage);
        }

        protected override void OnDisconnected()
        {
            AddUiMessage("Disconnected.");
        }

        protected override void OnConnectionLost()
        {
            AddUiMessage("CONNECTION LOST!");
        }

        private void UpdateUI()
        {
            if (client == null)
            {
                if (connectButton != null)
                {
                    connectButton.interactable = true;
                    disconnectButton.interactable = false;
                    testPublishButton.interactable = false;
                }
            }
            else
            {
                if (testPublishButton != null)
                {
                    testPublishButton.interactable = client.IsConnected;
                }
                if (disconnectButton != null)
                {
                    disconnectButton.interactable = client.IsConnected;
                }
                if (connectButton != null)
                {
                    connectButton.interactable = !client.IsConnected;
                }
            }
            if (addressInputField != null && connectButton != null)
            {
                addressInputField.interactable = connectButton.interactable;
                addressInputField.text = brokerAddress;
            }
            if (portInputField != null && connectButton != null)
            {
                portInputField.interactable = connectButton.interactable;
                portInputField.text = brokerPort.ToString();
            }
            if (encryptedToggle != null && connectButton != null)
            {
                encryptedToggle.interactable = connectButton.interactable;
                encryptedToggle.isOn = isEncrypted;
            }
            if (clearButton != null && connectButton != null)
            {
                clearButton.interactable = connectButton.interactable;
            }
            updateUI = false;
        }

        protected override void Start()
        {
            SetUiMessage("Ready.");
            updateUI = true;
            base.Start();
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("Received: " + msg);
            //MQTT_Msg_to_Json(msg);


            StoreMessage(msg);

            //if (topic == "M2MQTT_Unity/test")
            //{
            //    if (autoTest)
            //    {
            //        autoTest = false;
            //        Disconnect();
            //    }
            //}

            //if (topic == "SM_SHE")
            //{
            //    SM_SHE_id.text = msg_id;
            //    SM_SHE_value.text = msg_value;
            //    SM_SHE_quality.text = msg_quality;
            //    SM_SHE_timestamp.text = msg_timestamp;
            //}

            //if (topic == "iot_aveva")
            //{

            //    //Dynsim_id.text = msg_id;
            //    //Dynsim_value.text = msg_value;
            //    //Dynsim_quality.text = msg_quality;
            //    //Dynsim_timestamp.text = msg_timestamp;

            //}

        }

        private void StoreMessage(string eventMsg)
        {
            eventMessages.Add(eventMsg);
        }

        private void ProcessMessage(string msg)
        {
            AddUiMessage("Received: " + msg);
        }

        protected override void Update()
        {
            base.Update(); // call ProcessMqttEvents()

            if (eventMessages.Count > 0)
            {
                foreach (string msg in eventMessages)
                {
                    ProcessMessage(msg);
                }
                eventMessages.Clear();
            }
            if (updateUI)
            {
                UpdateUI();
            }
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnValidate()
        {
            if (autoTest)
            {
                autoConnect = true;
            }
        }

        public void StartMotor()
        {
            client.Publish("unity_belaz", System.Text.Encoding.UTF8.GetBytes("1"), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
        }

        public void StopMotor()
        {
            client.Publish("unity_belaz", System.Text.Encoding.UTF8.GetBytes("0"), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
        }

        //On Slider Update Value
        public void WriteSliderValue()
        {
            //var value_to_write = (100 * SliderControl.value).ToString("0").Replace(',', '.');
            var value_to_write = (100 * SliderControl.GetComponent<Slider>().Value).ToString("0").Replace(',', '.');

            //var json_msg_to_write = "[{\"id\": \"DynsimChannel.Device1.Dynsim.STMDMD\", \"v\": " + $"{value_to_write}}}]";
            //var json_msg_to_write = "[{\"id\": \"DynsimChannel.Device1.Dynsim.STMDMD\",\"v\": 20}]";
            Debug.Log(value_to_write);
            client.Publish("unity_belaz/speed", System.Text.Encoding.UTF8.GetBytes(value_to_write), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }


        //private void ParseData(string parsingMsg, out string id, out string value, out string quality, out string timestamp)
        //{
        //    string[] parseData = parsingMsg.Split(new string[] { @"id"":""", @""",""v"":", @",""q"":", @",""t"":", @"},{" }, StringSplitOptions.None);

        //    id = parseData[1];
        //    value = parseData[2];
        //    quality = parseData[3];
        //    timestamp = parseData[4];
        //}


        //private void MQTT_Msg_to_Json(string msg)
        //{
        //    Debug.Log(msg);
        //    MQTT_Data mqtt_data = JsonUtility.FromJson<MQTT_Data>(msg); // Получаем JSON
        //    Debug.Log(mqtt_data.values.Length);
        //    foreach (Data data in mqtt_data.values)
        //    {
        //        if (data.id == "DynsimChannel.Device1.Dynsim.S2_W")
        //        {
        //            S2_W.text = data.value;
        //            //S2_W2.text = data.value;
        //            //S2_W3.text = data.value;

        //        }
        //        if (data.id == "DynsimChannel.Device1.Dynsim.S5_W")
        //        {
        //            S5_W.text = data.value;
        //            //S5_W2.text = data.value;
        //            //S5_W3.text = data.value;

        //        }
        //        if (data.id == "DynsimChannel.Device1.Dynsim.S7_T")
        //        {
        //            S7_T.text = data.value;
        //            //S7_T2.text = data.value;
        //            //S7_T3.text = data.value;

        //        }
        //        if (data.id == "DynsimChannel.Device1.Dynsim.S9_T")
        //        {
        //            S9_T.text = data.value;
        //            //S9_T2.text = data.value;
        //            //S9_T3.text = data.value;

        //        }
        //        if (data.id == "DynsimChannel.Device1.Dynsim.S11_W")
        //        {
        //            S11_W.text = data.value;
        //            //S11_W2.text = data.value;
        //            //S11_W3.text = data.value;

        //        }
        //        if (data.id == "DynsimChannel.Device1.Dynsim.SRC1_PB")
        //        {
        //            SRC1_PB.text = data.value;
        //            //SRC1_PB2.text = data.value;
        //            //SRC1_PB3.text = data.value;

        //        }
        //        if (data.id == "DynsimChannel.Device1.Dynsim.SRC1_TB")
        //        {
        //            SRC1_TB.text = data.value;
        //            //SRC1_TB2.text = data.value;
        //            //SRC1_TB3.text = data.value;

        //        }
        //        if (data.id == "DynsimChannel.Device1.Dynsim.SRC2_PB")
        //        {
        //            SRC2_PB.text = data.value;
        //            //SRC2_PB2.text = data.value;
        //            //SRC2_PB3.text = data.value;

        //        }
        //        if (data.id == "DynsimChannel.Device1.Dynsim.V2_FLASH_VF")
        //        {
        //            V2_FLASH_VF.text = data.value;
        //            //V2_FLASH_VF2.text = data.value;
        //            //V2_FLASH_VF3.text = data.value;


        //        }
        //        if (data.id == "DynsimChannel.Device1.Dynsim.V2_L")
        //        {
        //            V2_L.text = data.value;
        //            //V2_L2.text = data.value;
        //            //V2_L3.text = data.value;

        //        }
        //        if (data.id == "DynsimChannel.Device1.Dynsim.V2_P")
        //        {
        //            V2_P.text = data.value;
        //            //V2_P2.text = data.value;
        //            //V2_P3.text = data.value;


        //        }
        //        if (data.id == "DynsimChannel.Device1.Dynsim.STMDMD")
        //        {
        //            STMDMD.text = data.value;

        //        }
        //        if (data.id == "DynsimChannel.Device1.Dynsim.LVLSP")
        //        {
        //            LVLSP.text = data.value;

        //        }
        //        if (data.id == "DynsimChannel.Device1.Dynsim.MASTER1_B_OUT[0]")
        //        {
        //            MASTER1_B_OUT0.text = data.value;

        //        }
        //        if (data.id == "DynsimChannel.Device1.Dynsim.MASTER1_B_OUT[1]")
        //        {
        //            MASTER1_B_OUT1.text = data.value;

        //        }
        //        //
        //        if (data.id == "Channel MODBUS TCP/IP.schetchik022.Current_Phase1")
        //        {
        //            SM_SHE_current1.ToolTipText = "I321\n" + data.value.Substring(0, 4) + " A";

        //        }
        //        if (data.id == "Channel MODBUS TCP/IP.schetchik021.Voltage_L1-L2")
        //        {
        //            SM_SHE_voltage1.ToolTipText = "U321\n" + Math.Round(Convert.ToDecimal(data.value), 2) + " В";

        //        }
        //        if (data.id == "Channel MODBUS TCP/IP.schetchik021.Power Factory")
        //        {
        //            SM_SHE_powerFactory.ToolTipText = "P321\n" + data.value.Substring(0,4) + " кВт";

        //        }

        //        if (data.id == "Channe2.SmLink2_UP.up2_32")
        //        {
        //            up2_32 = data.value;
        //        }
        //        if (data.id == "Channe2.SmLink2_DOWN.down11_32")
        //        {
        //            down11_32 = data.value;
        //        }
        //        if (data.id == "Channe2.SmLink2_MAIN.Main4_32")
        //        {
        //            main4_32 = data.value;
        //        }
        //    }


        //}



        //public void WriteSliderValueLVLSP()
        //{
        //    var value_to_write = (2*LVLSP_SliderControl.SliderValue).ToString("0").Replace(',', '.');
        //    var json_msg_to_write = "[{\"id\": \"DynsimChannel.Device1.Dynsim.LVLSP\", \"v\": " + $"{value_to_write}}}]";
        //    //var json_msg_to_write = "[{\"id\": \"DynsimChannel.Device1.Dynsim.STMDMD\",\"v\": 20}]";
        //    //Debug.Log(json_msg_to_write);
        //    client.Publish("iotgateway/write", System.Text.Encoding.UTF8.GetBytes(json_msg_to_write), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
        //}

        //public void WriteSliderValueMASTER_B0()
        //{
        //    var value_to_write = (B0_SliderControl.SliderValue).ToString("0").Replace(',', '.');
        //    var json_msg_to_write = "[{\"id\": \"DynsimChannel.Device1.Dynsim.MASTER1_B_OUT[0]\", \"v\": " + $"{value_to_write}}}]";
        //    //var json_msg_to_write = "[{\"id\": \"DynsimChannel.Device1.Dynsim.STMDMD\",\"v\": 20}]";
        //    //Debug.Log(json_msg_to_write);
        //    client.Publish("iotgateway/write", System.Text.Encoding.UTF8.GetBytes(json_msg_to_write), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
        //}

        //public void WriteSliderValueMASTER_B1()
        //{
        //    var value_to_write = (B1_SliderControl.SliderValue).ToString("0").Replace(',', '.');
        //    var json_msg_to_write = "[{\"id\": \"DynsimChannel.Device1.Dynsim.MASTER1_B_OUT[1]\", \"v\": " + $"{value_to_write}}}]";
        //    //var json_msg_to_write = "[{\"id\": \"DynsimChannel.Device1.Dynsim.STMDMD\",\"v\": 20}]";
        //    //Debug.Log(json_msg_to_write);
        //    client.Publish("iotgateway/write", System.Text.Encoding.UTF8.GetBytes(json_msg_to_write), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
        //}

        //public void StartStopMotor()
        //{

        //    //if (down11_32 == "2")
        //    //{
        //    //    value_to_write = 2;
        //    //}
        //    //if (down11_32 == "3")
        //    //{
        //    //    value_to_write = 1;
        //    //}

        //    //var json_msg_to_write = "[{\"id\": \"Channe2.SmLink2_DOWN.down11_Con2\", \"v\": " + $"{value_to_write}}}]";
        //    //client.Publish("SM_SHE/write", System.Text.Encoding.UTF8.GetBytes(json_msg_to_write), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);


        //    if (main4_32 == "2")
        //    {
        //        value_to_write = 2;
        //    }
        //    if (main4_32 == "3")
        //    {
        //        value_to_write = 1;
        //    }

        //    var json_msg_to_write = "[{\"id\": \"Channe2.SmLink2_MAIN.Main4_Con2\", \"v\": " + $"{value_to_write}}}]";
        //    client.Publish("SM_SHE/write", System.Text.Encoding.UTF8.GetBytes(json_msg_to_write), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);

        //}

        public void InteractWith3DView()
        {
            if (Motor.gameObject.activeSelf)
            {
                Motor.gameObject.SetActive(false);
            }
            else
            {
                Motor.gameObject.SetActive(true);
            }
            
        }

        
       


    }


   



}
