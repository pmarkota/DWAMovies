using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.BLModels;

[Table("Image")]
public partial class BLImage
{
    [Key]
    public int Id { get; set; }

    public string Content { get; set; } = null!;
}
