namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassConfigCommon
    {
        private bool lookMyLandBattle = true;
        public bool LookMyLandBattle
        {
            get { return lookMyLandBattle; }
            set { lookMyLandBattle = value; }
        }
        private bool lookOtherLandBattle = false;
        public bool LookOtherLandBattle
        {
            get { return lookOtherLandBattle; }
            set { lookOtherLandBattle = value; }
        }

        private double timeOfDisplayMessage = 1.2;
        public double TimeOfDisplayMessage
        {
            get { return timeOfDisplayMessage; }
            set { timeOfDisplayMessage = value; }
        }

        private int volumeBGM = 50;
        public int VolumeBGM
        {
            get { return volumeBGM; }
            set { volumeBGM = value; }
        }

        private int volumeSE = 50;
        public int VolumeSE
        {
            get { return volumeSE; }
            set { volumeSE = value; }
        }
    }
}
