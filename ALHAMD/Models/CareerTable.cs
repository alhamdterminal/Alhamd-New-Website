﻿namespace ALHAMD.Models
{
    public class CareerTable
    {
        public string Topic { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email_Address { get; set; }
        public string Phone_Number { get; set; }
        public string Message { get; set; }
        public IFormFile Cv { get; set; }
    }
}
