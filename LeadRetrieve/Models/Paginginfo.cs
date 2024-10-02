using System;
using System.Collections.Generic;

namespace LeadRetrieve.Models;

public partial class Paginginfo
{
    public int Id { get; set; }

    public string? LeadId { get; set; }

    public string? CursorBefore { get; set; }

    public string? CursorAfter { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Lead? Lead { get; set; }
}
