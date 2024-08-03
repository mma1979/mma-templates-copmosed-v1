
namespace WebApplication1.Common.Models
{
    public record AppSettings
    {
        public string TranslatorEndPoint { get; set; }
        public string APIEndPoint { get; set; }
        public int Enviroment { get; set; }
        public string LogPath { get; set; }
        public string Orgins { get; set; }
    }
}
