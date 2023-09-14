using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DALModels;

[Table("Image")]
public partial class Image
{
    [Key]
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    [InverseProperty("Image")]
    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}
