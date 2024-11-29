using ALHAMD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
namespace ALHAMD.Pages
{
    public class MapModel:PageModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public void OnGet()
        {
            // Set default coordinates or fetch from a database
            Latitude = 24.87510320344546; // Example latitude
            Longitude = 66.95809603243741; // Example longitude
        }
    }
}
