using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for FaceSendInfo
/// </summary>

public class FaceData
{
    public string deviceId { get; set; }
    public string blobName { get; set; }
    public string cameraId { get; set; }
    public bool entryCamera { get; set; }
    public DateTime timeStamp { get; set; }
}

public class FaceSendInfo
{
        public string faceId { get; set; }

        private double _age;
        public double age
        {
            get { return _age; }
            set { _age = Math.Round(value, 1); }
        }

        public string gender { get; set; }

        private double _headRoll;
        public double headRoll
        {
            get { return _headRoll; }
            set { _headRoll = Math.Round(value, 1); }
        }

        private double _headYaw;
        public double headYaw
        {
            get { return _headYaw; }
            set { _headYaw = Math.Round(value, 1); }
        }

        private double _headPitch;
        public double headPitch
        {
            get { return _headPitch; }
            set { _headPitch = Math.Round(value, 1); }
        }

        private double _smile;
        public double smile
        {
            get { return _smile; }
            set { _smile = Math.Round(value, 3); }
        }

        private double _moustache;
        public double moustache
        {
            get { return _moustache; }
            set { _moustache = Math.Round(value, 1); }
        }

        private double _beard;
        public double beard
        {
            get { return _beard; }
            set { _beard = Math.Round(value, 1); }
        }

        private double _sideburns;
        public double sideburns
        {
            get { return _sideburns; }
            set { _sideburns = Math.Round(value, 1); }
        }

        public string glasses { get; set; }

        private double _anger;
        public double anger
        {
            get { return _anger; }
            set { _anger = Math.Round(value, 3); }
        }

        private double _contempt;
        public double contempt
        {
            get { return _contempt; }
            set { _contempt = Math.Round(value, 3); }
        }

        private double _disgust;
        public double disgust
        {
            get { return _disgust; }
            set { _disgust = Math.Round(value, 3); }
        }

        private double _fear;
        public double fear
        {
            get { return _fear; }
            set { _fear = Math.Round(value, 3); }
        }

        private double _happiness;
        public double happiness
        {
            get { return _happiness; }
            set { _happiness = Math.Round(value, 3); }
        }

        private double _neutral;
        public double neutral
        {
            get { return _neutral; }
            set { _neutral = Math.Round(value, 3); }
        }

        private double _sadness;
        public double sadness
        {
            get { return _sadness; }
            set { _sadness = Math.Round(value, 3); }
        }

        private double _surprise;
        public double surprise
        {
            get { return _surprise; }
            set { _surprise = Math.Round(value, 3); }
        }

        public DateTime timeStamp { get; set; }

        public FaceSendInfo()
        {
        }
}