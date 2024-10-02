using LeadRetrieve.Models;

namespace LeadRetrieve.Repositories
{
    public class LeadRepository
    {
        private LeadAdContext _context;

        public void AddLead(Lead newLead)
        {
            _context = new();
            _context.Leads.Add(newLead);
            _context.SaveChanges();
        }

        public Lead GetByLeadId (string LeadId)
        {
            _context = new();
            return _context.Leads.FirstOrDefault(x => x.LeadId == LeadId);
        }

    }
}
