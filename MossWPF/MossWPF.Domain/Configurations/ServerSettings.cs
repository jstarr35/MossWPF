namespace MossWPF.Domain.Configurations
{
    public class ServerSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public int ReceiveTimeout { get; set; }
        public int SendTimeout { get; set; }
        public int ReplySize { get; set; }
    }

}
