using BookMarket.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        
        public string Title { get; set; }
        public string Description { get; set; }

        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Display(Name ="List Price")]
        [Range(1,1000)]
        public double ListPrice { get; set; }
        [Display(Name = "Price for 1-50")]
        [Range(1, 50)]
        public double Price { get; set; }
        [Display(Name = "Price for 50+")]
        [Range(50, 100)]
        public double Price50 { get; set; }
        [Display(Name = "Price for 100+")]
        [Range(100, 1000)]
        public double Price100 { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        
        public string ImageUrl { get; set; }
    }
}
