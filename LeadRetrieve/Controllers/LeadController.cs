using LeadRetrieve.Models;
using Microsoft.AspNetCore.Mvc;
using LeadRetrieve.Repositories;
using Newtonsoft.Json;

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
            const string token = "EAAOZC8UI9SwwBOZBfbgiEuY2kPpNzCGEaz6pKZCqwkqyw84iCHvZCupSNLUeCSpfz979IlGc4DrMZC8ZADr4aY63cCIwRqzPqAQQZAjUYheNMeBV2pKqMgFth1E3EtoqnVsJyfXQ8OVsDAbcK55mFH2Agm0TalwQ2vrozOeUA12lzG5xFdv8VRnT0WZCahLIp5JxNBHARhc82ZAt72lkS4UdVZB9ZBtnQFZAP6JJauZCOULFZC";
            //form1: 1622460511944043
            //nonamedform: 1056720539243856
            const string apiUrl = "https://graph.facebook.com/v20.0/1056720539243856/leads?access_token=" + token;

        
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(apiUrl);
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
                        //var leadEntity = new LeadDataEntity
                        //{
                        //    LeadId = lead.Id,
                        //    CreatedTime = lead.CreatedTime,
                        //    FieldName = field.Name,
                        //    FieldValue = string.Join(",", field.Values)
                        //};

                        //_context.Leads.Add(leadEntity);

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
    }
}
