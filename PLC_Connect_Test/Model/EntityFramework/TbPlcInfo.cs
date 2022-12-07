using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLC_Connect_Test.Model.EntityFramework
{
    [Table("tb_plc_info")]
    [Microsoft.EntityFrameworkCore.Index(nameof(PointName), Name = "point_name_UNIQUE", IsUnique = true)]
    public partial class TbPlcInfo
    {
        [Key]
        [Column("idx")]
        public int Idx { get; set; }
        [Required]
        [Column("point_name")]
        [StringLength(45)]
        public string PointName { get; set; }
        [Required]
        [Column("plc_ip")]
        [StringLength(45)]
        public string Ip { get; set; }
        [Column("plc_port")]
        public int Port { get; set; }
        [Column("read_loc")]
        public string ReadLoc { get; set; }
        [Column("plc_type")]
        public int PlcType { get; set; }
        [Column("work_type")]
        public int WorkType { get; set; }
        [Column("project_idx")]
        public int ProjectIdx { get; set; }
        [Column("desc")]
        public string Desc { get; set; }

        [NotMapped]
        public BindingList<TbPlcInfoDtl> Dtl { get; set; }
    }
}
