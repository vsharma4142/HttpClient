using System;
using System.Collections.Generic;

namespace ETL.Data.Models;

public partial class AccountDetail
{
    public string? CustomerName { get; set; }

    public string? CustomerRegion { get; set; }

    public string? Ssn { get; set; }

    public string? Phone { get; set; }

    public string? AccountType { get; set; }

    public string? AccountRegion { get; set; }

    public string? AccontCatageory { get; set; }

    public bool? IsLocked { get; set; }
}
