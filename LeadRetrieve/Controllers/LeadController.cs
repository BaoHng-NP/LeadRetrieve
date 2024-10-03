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
            const string token = "EAAOZC8UI9SwwBO0MODcQ18I6ZAugIolIKNppx0dR5X8blBZBR2YsC1BCrB7WexNNITvCsXtobX8uSdjnDkTRBPofm8AuArB6ZAFLeSLvPA0cD5gVIKvLrGfzN48OKAv1oKBFpPClo9olZAaN9ixpuTNlIX29giBpQRJaOgTr1LCipTxYowXS0gELaxxij5zQ4fJs7gCg7awmuqeRpci7ACm1omWBOvXExgcsOKjyu";
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

        [HttpPost("webhook")]
        public async Task<HttpResponseMessage> Post([FromBody] JsonDataModel data)
        {
            try
            {
                var entry = data.Entry.FirstOrDefault();
                var change = entry?.Changes.FirstOrDefault();
                if (change == null) return new HttpResponseMessage(HttpStatusCode.BadRequest);

                //Generate user access token here https://developers.facebook.com/tools/accesstoken/
                const string token = "EAAOZC8UI9SwwBO0MODcQ18I6ZAugIolIKNppx0dR5X8blBZBR2YsC1BCrB7WexNNITvCsXtobX8uSdjnDkTRBPofm8AuArB6ZAFLeSLvPA0cD5gVIKvLrGfzN48OKAv1oKBFpPClo9olZAaN9ixpuTNlIX29giBpQRJaOgTr1LCipTxYowXS0gELaxxij5zQ4fJs7gCg7awmuqeRpci7ACm1omWBOvXExgcsOKjyu";


                var leadUrl = $"https://graph.facebook.com/v14.0/{change.Value.LeadGenId}?access_token={token}";
                var formUrl = $"https://graph.facebook.com/v14.0/{change.Value.FormId}?access_token={token}";


                if (!string.IsNullOrEmpty(token))
                {
                    using (var httpClientLead = new HttpClient())
                    {
                        var response = await httpClientLead.GetStringAsync(formUrl);
                        if (!string.IsNullOrEmpty(response))
                        {
                            var jsonObjLead = JsonConvert.DeserializeObject<LeadFormData>(response);
                            //jsonObjLead.Name contains the lead ad name

                            //If response is valid get the field data
                            using (var httpClientFields = new HttpClient())
                            {
                                var responseFields = await httpClientFields.GetStringAsync(leadUrl);
                                if (!string.IsNullOrEmpty(responseFields))
                                {
                                    var jsonObjFields = JsonConvert.DeserializeObject<LeadData>(responseFields);
                                    //jsonObjFields.FieldData contains the field value
                                }
                            }
                        }
                    }
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.BadGateway);
            }
        }


    }
}
