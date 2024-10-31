﻿
namespace LibraryManagementFront.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        public int CategoryId { get; set; } // Foreign key property

        public Category Category { get; set; }
    }
}
