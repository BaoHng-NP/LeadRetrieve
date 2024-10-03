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
        private readonly LeadAdContext _context;

        public LeadController(LeadAdContext context)
        {
            _context = context;
        }

        [HttpGet("FetchLeads")]
        public async Task<IActionResult> FetchLeads()
        {
            const string token = "EAAOZC8UI9SwwBO4ihDTOI1fJB8zqrxNdfELVtmgmG0NZB7tvAOv1ybcK3f4aGIHbBiApRlcayZBm8S7l7DEiGcCZCxfd5z1JvS14ftzvfDnK6GZC8qlaMCR8xwD3nuOFcGQgm6AP3kQfBn9MVrDkYwsJSmtGdkvWzFwGneHuRNwPkitxfrxAlAjXRSULLwTYAd5ZCI3kQgQZAXmQg9fjnRQQ7b13s3wcx2YNzM2wZCq8";
            //form1: 1622460511944043
            //nonamedform: 1056720539243856
            const string formUrl = "https://graph.facebook.com/v20.0/1056720539243856/leads?access_token=" + token;


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


        [HttpPost("webhook")]
        public async Task<HttpResponseMessage> Post([FromBody] JsonDataModel data)
        {
            try
            {
                var entry = data.Entry.FirstOrDefault();
                var change = entry?.Changes.FirstOrDefault();
                if (change == null) return new HttpResponseMessage(HttpStatusCode.BadRequest);

                const string token = "EAAOZC8UI9SwwBO4ihDTOI1fJB8zqrxNdfELVtmgmG0NZB7tvAOv1ybcK3f4aGIHbBiApRlcayZBm8S7l7DEiGcCZCxfd5z1JvS14ftzvfDnK6GZC8qlaMCR8xwD3nuOFcGQgm6AP3kQfBn9MVrDkYwsJSmtGdkvWzFwGneHuRNwPkitxfrxAlAjXRSULLwTYAd5ZCI3kQgQZAXmQg9fjnRQQ7b13s3wcx2YNzM2wZCq8";

                var formUrl = $"https://graph.facebook.com/v20.0/{change.Value.FormId}/leads?access_token={token}";

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
                return new HttpResponseMessage(HttpStatusCode.BadGateway);
            }
        }
    }
}
