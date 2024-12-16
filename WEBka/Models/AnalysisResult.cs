namespace WEBka.Models
{
    public class AnalysisResult
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string[] ResultData { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
