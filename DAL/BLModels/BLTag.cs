using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.DALModels;
using Microsoft.EntityFrameworkCore;

namespace DAL.BLModels;

[Table("Tag")]
public partial class BLTag
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    //[InverseProperty("Tag")]
    //public virtual ICollection<BLVideoTag> VideoTags { get; set; } = new List<BLVideoTag>();
}
