using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.API
{
    public class APIModel
    {
    }
    public class API_DiscountCallModel
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public DiscountModel Data { get; set; }
    }
    public class API_DiscountCardCallModel
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public DiscoutCardModel Data { get; set; }
    }
    public class DiscoutCardModel
    {
        public string NumberId { get; set; }
        public int LocationId { get; set; }
        public decimal PercentDiscount { get; set; }
        public DateTime ActiveDate { get; set; }
        public bool IsVIP { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedDate { get; set; }
        public int total { get; set; }
        public string location { get; set; }
    }
    public class DiscountModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 1: giảm trực tiếp
        /// 2: giảm theo %
        /// </summary>
        public int? Type { get; set; }
        public int? ApplyType { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Percentage { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? LimitationTimes { get; set; }
        public bool Status { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? OrderValueCondition { get; set; }
    }
    public class DiscountResult
    {
        public decimal TotalPriceNew { get; set; }
        public int type { get; set; }
        public decimal? DiscountValue { get; set; }
    }
    public class ProductSyncModel
    {
        public int WarehouseId { get; set; }
        public string Barcode { get; set; }
        public int Allowcated { get; set; }
    }
    public class WarehouseSyncModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool Status { get; set; }
        public int UserId { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int ParentId { get; set; }
        public string PrefixCustomer { get; set; }
        public string PrefixInput { get; set; }
        public string PrefixOutput { get; set; }
        public string PrefixInputRef { get; set; }
        public string PrefixTrans { get; set; }
        public string PrefixStock { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string zoom { get; set; }
        public int ShoppingcartId { get; set; }
        public int ShopId { get; set; }
    }
    public class ProductSyncDataModel
    {
        public string Barcode { get; set; }
        public int WarehouseId { get; set; }
        public int? Allowcated { get; set; }
        public int? InventoryNumber { get; set; }
        public decimal Price { get; set; }
        public decimal SellPrice { get; set; }
    }
}
