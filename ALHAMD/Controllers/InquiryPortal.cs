using ALHAMD.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ALHAMD.Controllers
{
    public class InquiryPortal : Controller
    {
        private readonly ILogger<InquiryPortal> _logger;
       
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public InquiryPortal(ILogger<InquiryPortal> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
           
            _httpClient = httpClientFactory.CreateClient("ContainerInquiry");
        }



        public IActionResult ContainerInquiry()
        {
            return View();
        }
        public IActionResult ContainerInquiryTry()
        {
            return View();
        }


        public async Task<IActionResult> Details(string containerNo)
        {
            if (string.IsNullOrEmpty(containerNo))
            {
                return BadRequest("Container number is required.");
            }

            //var apiUrl = $"http://3.64.191.248:8083/api/Container/details/{containerNo}";

            
                try
                {
                    var response = await _httpClient.GetAsync($"/api/Container/details/{containerNo}");

                    
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResult = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Raw JSON response: {jsonResult}"); // Log the raw JSON

                        // Deserialize the response
                        var result = JsonConvert.DeserializeObject<dynamic>(jsonResult);

                        if (result != null)
                        {
                            return Json(new
                            {
                                containerNo = result.containerNo?.ToString() ?? "N/A",
                                igmNo = result.igmNo?.ToString() ?? "N/A",
                                blNo = result.blNo?.ToString() ?? "N/A",
                                indexNo = result.indexNo?.ToString() ?? "N/A",
                                line = result.line?.ToString() ?? "N/A",
                                sizeType = result.sizeType?.ToString() ?? "N/A",
                                category = result.category?.ToString() ?? "N/A",
                                goodsDescription = result.goodsDescription?.ToString() ?? "N/A",
                                portOfDischarge = result.portOfDischarge?.ToString() ?? "N/A",
                                arrivalDate = result.arrivalDate?.ToString() ?? "N/A",
                                deliverDate = result.deliverDate?.ToString() ?? "N/A",
                                vessel = result.vessel?.ToString() ?? "N/A",
                                voyage = result.voyage?.ToString() ?? "N/A",
                                shipperSeal = result.shipperSeal?.ToString() ?? "N/A",
                                portDischargeDate = result.portDischargeDate?.ToString() ?? "N/A",
                                grossWeight = result.grossWeight?.ToString() ?? "N/A"
                            });
                        }
                        else
                        {
                            return NotFound("No data found for the given container number.");
                        }
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return StatusCode((int)response.StatusCode, $"Error retrieving data: {errorMessage}");
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            
        }






        //public async Task<IActionResult> Details(string containerNo)
        //{
        //    // Define your connection string (consider moving this to a config file)
        //    var connectionString = "Server=192.168.1.252;Database=Tosdb;User Id=devteam;Password=Test@123;";

        //    using (var connection = new SqlConnection(connectionString))
        //    {
        //        await connection.OpenAsync();

        //        var sql = @"
        //                    SELECT cfs.ContainerNo, cfs.VirNo AS igmNo, cfs.BLNo, cfs.IndexNo,
        //                           sh.Name AS line, cfs.size,
        //                           CONCAT(cfs.Status, '/', cfs.CargoType) AS sizeType,
        //                           gh.GoodsHeadName AS category,
        //                           cfs.Description AS goods_description,
        //                           pt.PortName AS port_of_discharge,
        //                           cfs.CFSContainerGateInDate AS arrival_date,
        //                           cfs.CFSContainerGateOutDate AS deliver_date,
        //                           ve.VesselName AS vessel,
        //                           vo.VoyageNo AS voyage,
        //                           cfs.ManifestedSealNumber AS shipperSeal,
        //                           i.OutTime AS port_discharge_date,
        //                           cfs.ContainerGrossWeight AS gross_weight
        //                    FROM ContainerIndex cfs
        //                    INNER JOIN ShippingAgent sh ON cfs.ShippingAgentId = sh.ShippingAgentId
        //                    INNER JOIN GoodsHead gh ON cfs.GoodsHeadId = gh.GoodsHeadId
        //                    INNER JOIN PortAndTerminal pt ON cfs.PortAndTerminalId = pt.PortAndTerminalId
        //                    INNER JOIN Voyage vo ON cfs.VoyageId = vo.VoyageId
        //                    INNER JOIN Vessel ve ON vo.VesselId = ve.VesselId
        //                    INNER JOIN IPAOs i ON cfs.VirNo = i.VIRNumber AND cfs.ContainerNo = i.ContainerNumber
        //                    WHERE cfs.ContainerNo = @ContainerNo

        //                    UNION ALL

        //                    SELECT cy.ContainerNo, cy.VirNo AS igmNo, cy.BLNo, cy.IndexNo,
        //                   sh.Name AS line, cy.size,
        //                   CONCAT(cy.Status, '/', cy.CargoType) AS sizeType,
        //                   gh.GoodsHeadName AS category,
        //                   cy.Description AS goods_description,
        //                   pt.PortName AS port_of_discharge,
        //                   cy.CYContainerGateInDate AS arrival_date,
        //                   cy.CYContainerGateOutDate AS deliver_date,
        //                   ve.VesselName AS vessel,
        //                   vo.VoyageNo AS voyage,
        //                   cy.ManifestedSealNumber AS shipperSeal,
        //                   i.OutTime AS port_discharge_date,
        //                   cy.ContainerGrossWeight AS gross_weight
        //            FROM CYContainer cy
        //            INNER JOIN ShippingAgent sh ON cy.ShippingAgentId = sh.ShippingAgentId
        //            INNER JOIN GoodsHead gh ON cy.GoodsHeadId = gh.GoodsHeadId
        //            INNER JOIN PortAndTerminal pt ON cy.PortAndTerminalId = pt.PortAndTerminalId
        //            INNER JOIN Voyage vo ON cy.VoyageNo = vo.VoyageNo
        //            INNER JOIN Vessel ve ON vo.VesselId = ve.VesselId
        //            INNER JOIN IPAOs i ON cy.VirNo = i.VIRNumber AND cy.ContainerNo = i.ContainerNumber
        //            WHERE cy.ContainerNo = @ContainerNo;";

        //        using (var command = new SqlCommand(sql, connection))
        //        {
        //            command.Parameters.AddWithValue("@ContainerNo", containerNo);

        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                if (await reader.ReadAsync())
        //                {
        //                    var result = new
        //                    {
        //                        containerNo = reader["ContainerNo"],
        //                        igmNo = reader["igmNo"],
        //                        blNo = reader["BLNo"],
        //                        indexNo = reader["IndexNo"],
        //                        line = reader["line"],
        //                        size = reader["size"],
        //                        sizeType = reader["sizeType"],
        //                        category = reader["category"],
        //                        goodsDescription = reader["goods_description"],
        //                        portOfDischarge = reader["port_of_discharge"],
        //                        arrivalDate = reader["arrival_date"],
        //                        deliverDate = reader["deliver_date"],
        //                        vessel = reader["vessel"],
        //                        voyage = reader["voyage"],
        //                        shipperSeal = reader["shipperSeal"],
        //                        portDischargeDate = reader["port_discharge_date"],
        //                        grossWeight = reader["gross_weight"]
        //                    };

        //                    return Json(result);
        //                }
        //            }
        //        }
        //    }

        //    return NotFound(); // Handle case where no data is found
        //}


        public async Task<IActionResult> InquiryPortalByBLNo(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                ViewBag.ErrorMessage = "BL cannot be null or empty.";
                return View(Enumerable.Empty<ContainerIndex>());
            }

            List<ContainerIndex> inquiries = new List<ContainerIndex>();

            try
            {
                // Prepare the API URL
                //var apiUrl = $"http://3.64.191.248:8083/api/Container/by-blno/{input}";

                // Call the API
                var results = await _httpClient.GetFromJsonAsync<List<ContainerIndex>>($"/api/Container/by-blno/{input}");

                if (results != null && results.Any())
                {
                    inquiries = results;
                }

                if (!inquiries.Any())
                {
                    ViewBag.ErrorMessage = "No data found. Make sure your BL No is correct.";
                }
                return View(inquiries); // Pass the list to the view
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging set up
                // _logger.LogError(ex, "An error occurred while retrieving container details.");
                ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
                return View("Error");
            }
        }


        //public IActionResult InquiryPortalByBLNo(string input)
        //{
        //    if (string.IsNullOrEmpty(input))
        //    {
        //        ViewBag.ErrorMessage = "BL cannot be null or empty.";
        //        return View(Enumerable.Empty<ContainerIndex>());
        //    }

        //    List<ContainerIndex> inquiries = new List<ContainerIndex>();

        //    try
        //    {
        //        var query1 = @"
        //         SELECT 
        //                cy.Status AS Type, 
        //                cy.CFSContainerGateInDate AS ArrivalDate, 
        //                cy.ContainerNo, 
        //                cy.BLNo, 
        //                cy.VirNo AS IGM, 
        //                cy.IndexNo, 
        //                gh.GoodsHeadName, 
        //                cy.IsDestuffed AS Destuffed
        //            FROM ContainerIndex cy
        //            INNER JOIN GoodsHead gh ON cy.GoodsHeadId = gh.GoodsHeadId
        //            WHERE cy.BLNo = @input";


        //        var result1 = _context.ContainerIndex
        //            .FromSqlRaw(query1, new SqlParameter("@input", input))
        //            .ToList();

        //        if (result1.Any())
        //        {
        //            inquiries = result1;
        //        }

        //        // Query 2: By BLNo
        //        if (!inquiries.Any())
        //        {
        //            var query2 = @"
        //                SELECT 
        //                cy.Status AS Type, 
        //                cy.CYContainerGateInDate AS ArrivalDate, 
        //                cy.ContainerNo, 
        //                cy.BLNo, 
        //                cy.VirNo AS IGM, 
        //                cy.IndexNo, 
        //                gh.GoodsHeadName, 
        //                cy.IsDestuffed AS Destuffed
        //            FROM CYContainer cy
        //            INNER JOIN GoodsHead gh ON cy.GoodsHeadId = gh.GoodsHeadId
        //            WHERE cy.BLNo = @input";


        //            var result2 = _context.ContainerIndex
        //                .FromSqlRaw(query2, new SqlParameter("@input", input))
        //                .ToList();

        //            if (result2.Any())
        //            {
        //                inquiries = result2;
        //            }
        //        }


        //        if (!inquiries.Any())
        //        {
        //            ViewBag.ErrorMessage = "No data found. Make sure your BL No is correct.";
        //        }

        //        // Query for the latest invoice date
        //        DateOnly? latestInvoiceDate = null;
        //        var invoiceQuery = "SELECT TOP (1) CONVERT(DATE, InvoiceDate) AS Invoicedate FROM Invoice ORDER BY InvoiceDate DESC";

        //        using (var connection = _context.Database.GetDbConnection())
        //        {
        //            using (var command = new SqlCommand(invoiceQuery, (SqlConnection)connection))
        //            {
        //                if (connection.State != System.Data.ConnectionState.Open)
        //                {
        //                    connection.Open();
        //                }

        //                object result = command.ExecuteScalar();
        //                if (result != DBNull.Value)
        //                {
        //                    DateTime dateTime = (DateTime)result;
        //                    latestInvoiceDate = DateOnly.FromDateTime(dateTime);
        //                }
        //            }
        //        }

        //        ViewBag.LatestInvoiceDate = latestInvoiceDate;

        //        return View(inquiries); // Pass the list to the view
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        // _logger.LogError(ex, "An error occurred while retrieving container details.");
        //        ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
        //        return View("Error");
        //    }
        //}

        public async Task<IActionResult> InquiryByContainerNo(string containerNo)
        {
            if (string.IsNullOrEmpty(containerNo))
            {
                return BadRequest("Container number cannot be null or empty.");
            }

            //var apiUrl = $"http://3.64.191.248:8083/api/Container/by-container-no/{containerNo}";

            try
            {
                var results = await _httpClient.GetFromJsonAsync<List<ContainerInfo>>($"/api/Container/by-container-no/{containerNo}");
                if (results == null || results.Count == 0)
                {
                    return NotFound("No data found for the specified container number.");
                }

                return View(results); // Pass the data to the view
            }
            catch (HttpRequestException)
            {
                return StatusCode(500, "Error fetching data from the API.");
            }
        }

        //public async Task<IActionResult> InquiryByContainerNo(string containerNo)
        //{
        //    var connectionString = "Server=192.168.1.252;Database=Tosdb;User Id=devteam;Password=Test@123;";

        //    using (var connection = new SqlConnection(connectionString))
        //    {
        //        await connection.OpenAsync();

        //        var sql = @"
        //    SELECT * FROM (
        //        SELECT TOP 1 cfs.ContainerNo, cfs.VirNo AS igmNo,
        //               sh.Name AS line, cfs.size,
        //               CONCAT(cfs.Status, '/', cfs.CargoType) AS sizeType,
        //               gh.GoodsHeadName AS category,
        //               cfs.Description AS goods_description,
        //               pt.PortName AS port_of_discharge,
        //               cfs.CFSContainerGateInDate AS arrival_date,
        //               cfs.CFSContainerGateOutDate AS deliver_date,
        //               ve.VesselName AS vessel,
        //               vo.VoyageNo AS voyage,
        //               cfs.ManifestedSealNumber AS shipperSeal,
        //               i.OutTime AS port_discharge_date,
        //               cfs.ContainerGrossWeight AS gross_weight
        //        FROM ContainerIndex cfs
        //        INNER JOIN ShippingAgent sh ON cfs.ShippingAgentId = sh.ShippingAgentId
        //        INNER JOIN GoodsHead gh ON cfs.GoodsHeadId = gh.GoodsHeadId
        //        INNER JOIN PortAndTerminal pt ON cfs.PortAndTerminalId = pt.PortAndTerminalId
        //        INNER JOIN Voyage vo ON cfs.VoyageId = vo.VoyageId
        //        INNER JOIN Vessel ve ON vo.VesselId = ve.VesselId
        //        INNER JOIN IPAOs i ON cfs.VirNo = i.VIRNumber AND cfs.ContainerNo = i.ContainerNumber
        //        WHERE cfs.ContainerNo = @ContainerNo
        //        ORDER BY cfs.CFSContainerGateInDate DESC
        //    ) AS LatestContainer

        //    UNION ALL

        //    SELECT * FROM (
        //        SELECT TOP 1 cy.ContainerNo, cy.VirNo AS igmNo,
        //               sh.Name AS line, cy.size,
        //               CONCAT(cy.Status, '/', cy.CargoType) AS sizeType,
        //               gh.GoodsHeadName AS category,
        //               cy.Description AS goods_description,
        //               pt.PortName AS port_of_discharge,
        //               cy.CYContainerGateInDate AS arrival_date,
        //               cy.CYContainerGateOutDate AS deliver_date,
        //               ve.VesselName AS vessel,
        //               vo.VoyageNo AS voyage,
        //               cy.ManifestedSealNumber AS shipperSeal,
        //               i.OutTime AS port_discharge_date,
        //               cy.ContainerGrossWeight AS gross_weight
        //        FROM CYContainer cy
        //        INNER JOIN ShippingAgent sh ON cy.ShippingAgentId = sh.ShippingAgentId
        //        INNER JOIN GoodsHead gh ON cy.GoodsHeadId = gh.GoodsHeadId
        //        INNER JOIN PortAndTerminal pt ON cy.PortAndTerminalId = pt.PortAndTerminalId
        //        INNER JOIN Voyage vo ON cy.VoyageNo = vo.VoyageNo
        //        INNER JOIN Vessel ve ON vo.VesselId = ve.VesselId
        //        INNER JOIN IPAOs i ON cy.VirNo = i.VIRNumber AND cy.ContainerNo = i.ContainerNumber
        //        WHERE cy.ContainerNo = @ContainerNo
        //        ORDER BY cy.CYContainerGateInDate DESC
        //    ) AS LatestCY;";

        //        using (var command = new SqlCommand(sql, connection))
        //        {
        //            command.Parameters.AddWithValue("@ContainerNo", containerNo);

        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                var results = new List<dynamic>();

        //                while (await reader.ReadAsync())
        //                {
        //                    var result = new
        //                    {
        //                        ContainerNo = reader["ContainerNo"],
        //                        IGMNo = reader["igmNo"],
        //                        Line = reader["line"],
        //                        Size = reader["size"],
        //                        SizeType = reader["sizeType"],
        //                        Category = reader["category"],
        //                        GoodsDescription = reader["goods_description"],
        //                        PortOfDischarge = reader["port_of_discharge"],
        //                        ArrivalDate = reader["arrival_date"] as DateTime?,
        //                        DeliverDate = reader["deliver_date"] as DateTime?,
        //                        Vessel = reader["vessel"],
        //                        Voyage = reader["voyage"],
        //                        ShipperSeal = reader["shipperSeal"],
        //                        PortDischargeDate = reader["port_discharge_date"] as DateTime?,
        //                        GrossWeight = reader["gross_weight"]
        //                    };

        //                    results.Add(result);
        //                }


        //                if (results.Count > 0)
        //                {
        //                    return View(results); // Return a list with the results
        //                }
        //            }
        //        }
        //    }

        //    return NotFound(); // Handle case where no data is found
        //}





        public IActionResult ContainerDetails()
        {
            return View();
        }

        public IActionResult InquiryInvoice()
        {
            return View();
        }
        public IActionResult InquiryInvoiceCYviaBLNo(string BLNo, string Date)
        {
            return RedirectToAction("InquiryInvoice");
        }

        public IActionResult InquiryInvoiceCYviaVirNo(string VirNo, string IndexNo, string Date)
        {
            return RedirectToAction("InquiryInvoice");
        }




        public IActionResult InquiryInvoiceCFSviaBLNo(string BLNo, string CBM, string Date)
        {
            return RedirectToAction("InquiryInvoice");
        }

        public IActionResult InquiryInvoiceCFSviaVirNo(string VirNo, string IndexNo, string CBM, string Date)
        {
            return RedirectToAction("InquiryInvoice");
        }

    }
}
