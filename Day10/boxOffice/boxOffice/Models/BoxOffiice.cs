using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace boxOffice.Models
{
    public class BoxOffiice
    {
        public int Id { get; set; }
        public string Rnum { get; set; }
        public string Rank_ { get; set; }
        public string RankInten { get; set; }
        public string RankOldAndNew { get; set; }
        public string MovieCd { get; set; }
        public string MovieNm { get; set; }
        public DateTime OpenDt { get; set; }
    }
}
