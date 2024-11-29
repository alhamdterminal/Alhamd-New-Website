using ALHAMD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;

namespace ALHAMD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
       // private readonly WebDbContext _context;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
           // _context = context;
        }

      
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult ContactUs()
        {

            if (TempData["Message"]!= null)
            {
                ViewBag.Message = TempData["Message"].ToString();
                ViewBag.Title = TempData["Title"].ToString();
                ViewBag.Icon = TempData["Icon"].ToString();

            }

            return View();
        }

        private static string GetLocalIpAddress()
        {
            string ipAddress = "Unknown";
            foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipAddress = ip.ToString();
                    break;
                }
            }
            return ipAddress;
        }


        
        public IActionResult SendEmail(ContactUsTable form)
        {
            string toEmail = "";
            

            string clientIpAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ??
                          HttpContext.Connection.RemoteIpAddress?.ToString();
            string username = HttpContext.User.Identity?.Name ?? "Anonymous";
            if (ModelState.IsValid)
            {
                string subject = "New Inquiry from " + form.First_Name + " " + form.Last_Name;
                string htmlContent = $@"
            <h1>Inquiry Details</h1>
            <h3>IP: {clientIpAddress} </h3>
            <h3> {username} </h3>

            <p><strong>QueryAbout:</strong> {form.Topic}</p>
            <p><strong>First Name:</strong> {form.First_Name}</p>
            <p><strong>Last Name:</strong> {form.Last_Name}</p>
            <p><strong>Email:</strong> {form.Email_Address}</p>
            <p><strong>Phone:</strong> {form.Phone_Number}</p>
            <p><strong>Message:</strong></p>
            <p>{form.Message}</p>";


                if(form.Topic == "CSD")
                {
                    toEmail = "help.desk@aictpk.com";
                }
                if (form.Topic == "Operations")
                {
                    toEmail = "operations.alert@aictpk.com";
                }

                //if (form.Topic == "Finance")
                //{
                //    toEmail = "finance@aictpk.com";
                //}


                
                try
                {
                    SendEmail(subject, htmlContent, toEmail, "enquiries@aictpk.com", "smtp.gmail.com", 587, "enquiries@aictpk.com", "En@icties");
                    TempData["Message"] = "Your response submitted successfully.";
                    TempData["Icon"] = "success";
                    TempData["Title"] = "Thank You";

                    return RedirectToAction(nameof(ContactUs));

                }
                catch (Exception ex)
                {
                    
                    TempData["Message"] = ex.Message;
                    TempData["Icon"] = "error";
                    TempData["Title"] = "Error";

                    return RedirectToAction(nameof(ContactUs));
                }
            }
            else
            {
                TempData["Message"] = "Please Fill all required fields.";
                TempData["Icon"] = "error";
                TempData["Title"] = "Error";


            }
            return RedirectToAction(nameof(ContactUs));
        }

        private void SendEmail(string subject, string htmlContent, string toEmail, string fromEmail, string smtpServer, int smtpPort, string login, string password)
        {
            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = htmlContent,
                    IsBodyHtml = true
                };
                mail.To.Add(toEmail);

                SmtpClient smtpClient = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(login, password),
                    EnableSsl = true
                };

                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send email.", ex);
            }
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Gallery()
        {
            return View();
        }

        public IActionResult QualityPolicy()
        {
            return View();
        }

        public IActionResult InquiryPortal()
        {
            return View();
        }

        public IActionResult StoragePictures()
        {
            return View();
        }

        public IActionResult WareHousePictures()
        {
            return View();
        }

        public IActionResult CargoTransportPictures()
        {
            return View();
        }

        public IActionResult Location()
        {
            return View();
        }

        public IActionResult HR()
        {

            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"].ToString();
                ViewBag.Title = TempData["Title"].ToString();
                ViewBag.Icon = TempData["Icon"].ToString();

            }

            return View();
        }
        private void SendEmail1(string subject, string htmlContent, string toEmail,
                string fromEmail, string smtpServer, int smtpPort,
                string login, string password, string attachmentPath = null)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromEmail);
                    mail.Subject = subject;
                    mail.Body = htmlContent;
                    mail.IsBodyHtml = true;
                    mail.To.Add(toEmail);

                    
                    if (!string.IsNullOrEmpty(attachmentPath))
                    {
                        
                        if (System.IO.File.Exists(attachmentPath))
                        {
                            Attachment attachment = new Attachment(attachmentPath);
                            mail.Attachments.Add(attachment);
                        }
                        else
                        {
                            throw new FileNotFoundException("Attachment file not found.", attachmentPath);
                        }
                    }

                    using (SmtpClient smtpClient = new SmtpClient(smtpServer))
                    {
                        smtpClient.Port = smtpPort;
                        smtpClient.Credentials = new NetworkCredential(login, password);
                        smtpClient.EnableSsl = true;

                        smtpClient.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send email.", ex);
            }
        }

        public async Task<IActionResult> SendCV(CareerTable form)
        {
            string toEmail = "jobs@aictpk.com";
            string clientIpAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ??
                                      HttpContext.Connection.RemoteIpAddress?.ToString();
           

            if (ModelState.IsValid) 
            {
                string subject = $"Apply for {form.Topic} from {form.First_Name} {form.Last_Name}";
                string htmlContent = $@"
                <h1>Inquiry Details</h1>
                <h3>IP: {clientIpAddress}</h3>
                
                <p><strong>QueryAbout:</strong> {form.Topic}</p>
                <p><strong>First Name:</strong> {form.First_Name}</p>
                <p><strong>Last Name:</strong> {form.Last_Name}</p>
                <p><strong>Email:</strong> {form.Email_Address}</p>
                <p><strong>Phone:</strong> {form.Phone_Number}</p>
                <p><strong>Message:</strong></p>
                <p>{form.Message}</p>";

                try
                {
                    
                    if (form.Cv != null && form.Cv.Length > 0)
                    {
                        
                        string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                        
                        if (!Directory.Exists(tempPath))
                        {
                            Directory.CreateDirectory(tempPath);
                        }

                       
                        string cvPath = Path.Combine(tempPath, form.Cv.FileName);

                       
                        using (var stream = new FileStream(cvPath, FileMode.Create))
                        {
                            await form.Cv.CopyToAsync(stream);
                        }

                        
                        SendEmail1(subject, htmlContent, toEmail,
                                   "enquiries@aictpk.com", "smtp.gmail.com",
                                   587, "enquiries@aictpk.com", "En@icties", cvPath);

                        
                        if (System.IO.File.Exists(cvPath))
                        {
                            System.IO.File.Delete(cvPath);
                        }

                        TempData["Message"] = "Your response submitted successfully.";
                        TempData["Icon"] = "success";
                        TempData["Title"] = "Thank You for Applying";
                        return RedirectToAction(nameof(HR));
                    }
                    else
                    {
                        TempData["Message"] = "CV file not found.";
                        TempData["Icon"] = "error";
                        TempData["Title"] = "Error";
                        return RedirectToAction(nameof(HR));
                    }
                }
                catch (Exception ex)
                {
                    
                    TempData["Message"] = "There was an error sending your message.";
                    TempData["Icon"] = "error";
                    TempData["Title"] = "Error";
                    return RedirectToAction(nameof(HR));
                }
            }
            else
            {
                
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                TempData["Message"] = "Please fill all required fields correctly.";
                TempData["Icon"] = "error";
                TempData["Title"] = "Error";
            }

            return RedirectToAction(nameof(HR));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = HttpContext.TraceIdentifier;
            var errorViewModel = new ErrorViewModel
            {
                RequestId = requestId,
               
            };

            return View(errorViewModel);
        }

    }
}
