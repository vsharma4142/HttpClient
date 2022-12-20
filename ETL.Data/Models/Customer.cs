using System;
using System.Collections.Generic;

namespace ETL.Data.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public int AccountId { get; set; }

    public string? CustomerName { get; set; }

    public string? Ssn { get; set; }

    public string? CustomerRegion { get; set; }

    public string? Phone { get; set; }
}
