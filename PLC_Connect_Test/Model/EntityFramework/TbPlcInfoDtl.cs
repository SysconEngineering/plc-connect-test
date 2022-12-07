using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PLC_Connect_Test.Model.EntityFramework
{
    [Table("tb_plc_info_dtl")]
    public partial class TbPlcInfoDtl
    {
        [Key]
        [Column("idx")]
        public int Idx { get; set; }
        [Column("plc_info_idx")]
        public int PlcInfoIdx { get; set; }
        [Column("address")]
        public int Address { get; set; }
        [Column("data_type")]
        public int DataType { get; set; }
        [Column("desc")]
        public string Desc { get; set; }
        [NotMapped]
        public string plcName { get; set; }
    }
}
