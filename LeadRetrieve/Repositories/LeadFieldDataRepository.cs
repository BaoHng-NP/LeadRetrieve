using LeadRetrieve.Models;

namespace LeadRetrieve.Repositories
{
    public class LeadFieldDataRepository
    {
        private LeadAdContext _context;

        public void AddLeadFieldData(Leadfielddata newLeadFieldData)
        {
            _context = new();
            _context.Leadfielddata.Add(newLeadFieldData);
            _context.SaveChanges();
        }
    }
}
