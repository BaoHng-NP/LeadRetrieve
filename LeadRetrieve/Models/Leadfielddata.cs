using System;
using System.Collections.Generic;

namespace LeadRetrieve.Models;

public partial class Leadfielddata
{
    public int Id { get; set; }

    public string? LeadId { get; set; }

    public string? FieldName { get; set; }

    public string? FieldValue { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Lead? Lead { get; set; }
}
