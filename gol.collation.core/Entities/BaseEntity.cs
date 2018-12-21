using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace gol.collation.core
{
    class BaseEntity
    {
        [Key]
        public int ID { get; set; }
        public string CreatedBy { get; set; } = "System";
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateModified { get; set; } = DateTime.Now;
    }
}
