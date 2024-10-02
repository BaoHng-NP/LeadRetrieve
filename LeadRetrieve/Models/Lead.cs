using System;
using System.Collections.Generic;

namespace LeadRetrieve.Models;

public partial class Lead
{
    public int Id { get; set; }

    public string LeadId { get; set; } = null!;

    public DateTime? CreatedTime { get; set; }

    public string? JsonResponse { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Leadfielddata> Leadfielddata { get; set; } = new List<Leadfielddata>();

    public virtual ICollection<Paginginfo> Paginginfos { get; set; } = new List<Paginginfo>();
}
