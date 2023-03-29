namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassUnitBuilding : ClassUnit
    {
        /// <summary>
        /// マップのタイル位置
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// マップのタイル位置
        /// </summary>
        public int Y { get; set; }

        #region IsEnable
        private bool isEnable = true;
        public bool IsEnable
        {
            get { return isEnable; }
            set { isEnable = value; }
        }
        #endregion

        public _010_Enum.MapTipObjectType Type { get; set; }
    }
}
