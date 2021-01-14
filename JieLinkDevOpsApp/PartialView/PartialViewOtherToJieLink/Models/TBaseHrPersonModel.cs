using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewOtherToJieLink.Models
{
    public class TBaseHrPersonModel
    {
        /// <summary>
        /// 人员ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 人员编号
        /// </summary>
        public string NO { get; set; }

        /// <summary>
        /// 人员名称
        /// </summary>
        public string NAME { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int SEX { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string IDENTITYTYPE { get; set; }

        /// <summary>
        /// 证件编号
        /// </summary>
        public string IdentityNo { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        public string Tel1 { get; set; }

        public string Tel2 { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 房间编号
        /// </summary>
        public string RoomNO { get; set; }

        public string Address { get; set; }

        public string Photo { get; set; }

        public int AreaId { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public string DeptId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


        /// <summary>
        /// 是否已删除
        /// </summary>
        public int DeleteFlag { get; set; }

        public string PWD { get; set; }

        public int PersonType { get; set; }

        public DateTime OptDate { get; set; }

        public string Organization { get; set; }

        public string CertificateAddr { get; set; }

        public string CertificateImage { get; set; }

        public string CertificateScanImage { get; set; }

        public int State { get; set; }

        public string ShortName { get; set; }

        public int CertificateSex { get; set; }

        public string CarPicture { get; set; }

        public int UserUIID { get; set; }

        public int TenementTypeNo { get; set; }

        public int ToMasterRelationNO { get; set; }

        public int LiveInHouseStateNO { get; set; }

        public int CertificationTypeNO { get; set; }

        public int PayTypeId { get; set; }

        public string BirthAddr { get; set; }

        public DateTime Birthday { get; set; }

        public string PetInfo { get; set; }

        public string NonCehicleInfo { get; set; }

        public string NewKeyCode { get; set; }

        public string OldKeyCode { get; set; }

        public int RoomId { get; set; }

        public string GUID { get; set; }

        public int CarCount { get; set; }

    }

    public enum EnumHrPersonStatus
    {
        /// <summary>
        /// 删除
        /// </summary>
        deleted = 1,
        /// <summary>
        /// 正常
        /// </summary>
        normal = 0
    }
}
