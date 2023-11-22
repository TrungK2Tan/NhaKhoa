using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NhaKhoa.Models
{
    public class SearchViewModel
    {
        public string Keyword { get; set; }
        public List<AspNetUser> NhaSiResults { get; set; }
        public List<TinTuc> TinTucResults { get; set; }
    }
}