using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Shop.Application.DTOs.CategoryDTOs;

public class CategoryReadDTO
{
    public int Id { get; set; }

    public string? Name { get; set; }

    
    public string? Slug { get; set; }


 
    public string? Url { get; set; }

    
    public bool IsActive { get; set; } 

   
    public int? ParentId { get; set; }

    public ICollection<int>? Products { get; set; }

}
