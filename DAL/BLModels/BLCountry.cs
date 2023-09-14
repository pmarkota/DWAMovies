using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.BLModels;

public partial class BLCountry
{
    public int Id { get; set; }
    
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<BLUser> Users { get; set; } = new List<BLUser>();
}
