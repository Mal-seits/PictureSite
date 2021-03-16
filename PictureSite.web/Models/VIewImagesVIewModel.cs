using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PictureSite.data;
namespace PictureSite.web.Models
{
    public class ViewImageViewModel
    {
        public int imageId { get; set; }
        public bool EnteredPassword { get; set; }
        public bool CorrectPassword { get; set; }
    
        public Image Image { get; set;}
    }
    public class ShareImageVM
    {
        public Image Image { get; set; }
    }
   
}
