using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MMSite6.Models
{
    public class Document
    {
        [DisplayName("File ID")]
        [Key]
        public int fileId { get; set; }
        [DisplayName("Document Path")]
        public string documentPath { get; set; }
        [DisplayName("Item")]
        public Item item { get; set; }
        [DisplayName("Uploaded By")]
        public string uploadedBy { get; set; }
    }
}
