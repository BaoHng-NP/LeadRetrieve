using LeadRetrieve.Models;
using Microsoft.AspNetCore.Mvc;
using LeadRetrieve.Repositories;
using Newtonsoft.Json;
using System.Net;

namespace LeadRetrieve.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadController : ControllerBase
    {
        LeadFieldDataRepository _leadFieldDataRepository = new LeadFieldDataRepository();
        LeadRepository _leadRepository = new LeadRepository();
        private readonly PageTokenService _pageTokenService;
        private readonly LeadAdContext _context;


        public LeadController(LeadAdContext context, PageTokenService pageTokenService)
        {
            _context = context;
            _pageTokenService = pageTokenService;

        }


        //Test get lead api
        [HttpGet("FetchLeads")]
        public async Task<IActionResult> FetchLeads()
        {
            string token = await _pageTokenService.GetPageTokenAsync();
            //<formId>/leads?access_token={token}
            var formUrl = $"https://graph.facebook.com/v20.0/1056720539243856/leads?access_token={token}";


            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(formUrl);
                var leadResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LeadResponse>(response);

                foreach (var lead in leadResponse.Data)
                {

                    if (_leadRepository.GetByLeadId(lead.Id) != null)
                    {
                        continue;
                    }
                    var newlead = new Lead
                    {
                        LeadId = lead.Id,
                        CreatedTime = DateTime.Parse(lead.CreatedTime),
                        JsonResponse = JsonConvert.SerializeObject(lead.FieldData)
                    };

                    _leadRepository.AddLead(newlead);

                    foreach (var field in lead.FieldData)
                    {

                        var leadFieldData = new Leadfielddata
                        {
                            LeadId = lead.Id,
                            FieldName = field.Name,
                            FieldValue = string.Join(",", field.Values),

                        };
                        _leadFieldDataRepository.AddLeadFieldData(leadFieldData);
                        Console.WriteLine("Saving.... " + $"LeadId: {lead.Id}, CreatedTime: {lead.CreatedTime}, FieldName: {field.Name}, FieldValue: {string.Join(",", field.Values)}");
                    }
                }

                await _context.SaveChangesAsync();
            }
            return Ok(new { Message = "Leads fetched and stored successfully." });
        }

        //Webhooks endpoint verification 
        [HttpGet]
        [Route("webhooks")]
        public IActionResult VerifyWebhook([FromQuery(Name = "hub.mode")] string mode, [FromQuery(Name = "hub.challenge")] string challenge, [FromQuery(Name = "hub.verify_token")] string token)
        {
            string verifyToken = Environment.GetEnvironmentVariable("WEBHOOKS_VERIFY_TOKEN");  

            if (token == verifyToken && mode == "subscribe")
            {
                return Ok(challenge);
            }
            else
            {
                return Forbid();
            }
        }

        //WEBHOOKS POST endpoint
        [HttpPost("webhooks")]
        public async Task<HttpResponseMessage> Post([FromBody] JsonDataModel data)
        {
            try
            {
                var entry = data.Entry.FirstOrDefault();
                var change = entry?.Changes.FirstOrDefault();
                if (change == null) return new HttpResponseMessage(HttpStatusCode.BadRequest);

                string token = await _pageTokenService.GetPageTokenAsync();

                var formUrl = $"https://graph.facebook.com/v20.0/{change.Value.form_id}/leads?access_token={token}";

                if (!string.IsNullOrEmpty(token))
                {

                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetStringAsync(formUrl);
                        var leadResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LeadResponse>(response);

                        foreach (var lead in leadResponse.Data)
                        {

                            if (_leadRepository.GetByLeadId(lead.Id) != null)
                            {
                                continue;
                            }
                            var newlead = new Lead
                            {
                                LeadId = lead.Id,
                                CreatedTime = DateTime.Parse(lead.CreatedTime),
                                JsonResponse = JsonConvert.SerializeObject(lead.FieldData)
                            };

                            _leadRepository.AddLead(newlead);

                            foreach (var field in lead.FieldData)
                            {


                                var leadFieldData = new Leadfielddata
                                {
                                    LeadId = lead.Id,
                                    FieldName = field.Name,
                                    FieldValue = string.Join(",", field.Values),

                                };
                                _leadFieldDataRepository.AddLeadFieldData(leadFieldData);
                                Console.WriteLine("Saving.... " + $"LeadId: {lead.Id}, CreatedTime: {lead.CreatedTime}, FieldName: {field.Name}, FieldValue: {string.Join(",", field.Values)}");
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
                }

                await _context.SaveChangesAsync();
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HttpResponseMessage(HttpStatusCode.BadGateway);
            }
        }
    }
}
