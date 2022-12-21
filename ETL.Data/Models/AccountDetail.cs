using System;
using System.Collections.Generic;

namespace ETL.Data.Models;

public partial class AccountDetail
{


    public int AccountId { get; set; }
    public string? AccountType { get; set; }

    public string? AccountRegion { get; set; }

    public string? AccontCatageory { get; set; }

    public bool? IsLocked { get; set; }
}
