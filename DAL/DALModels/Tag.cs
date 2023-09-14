using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DALModels;

[Table("Tag")]
public partial class Tag
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [InverseProperty("Tag")]
    public virtual ICollection<VideoTag> VideoTags { get; set; } = new List<VideoTag>();
}
