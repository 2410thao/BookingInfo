using Microsoft.AspNetCore.Components;

namespace WebApplication1.Models
{
    public class HtmlModel
    {
        public string code { get; set; }
        public List<string> AreaHtmlList { get; set; } = new List<string>();
        public List<string> ProvinceHtmlList { get; set; } = new List<string>();
        public List<string> DistrictHtmlList { get; set; } = new List<string>();
        public List<string> ContHtmlList { get; set; } = new List<string>();
        public List<string> BookingHtmlList { get; set; } = new List<string>();
    }
}
