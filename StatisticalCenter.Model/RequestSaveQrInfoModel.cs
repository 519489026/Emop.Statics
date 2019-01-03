namespace StatisticalCenter.Model
{
    public class RequestSaveQrInfoModel
    {
        //string qrCode, int phoneType, string ip, string province, int enterType
        public string QrCode { get; set; }
        public int PhoneType { get; set; }
        public string Ip { get; set; }
        public string Province { get; set; }
        public int EnterType { get; set; }
        public string CilentCookie { get; set; }
    }
}