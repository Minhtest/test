using LIB.CardNumbers;
using LIB.Orders;
using LIB.Product;
using LIB.SyncSetting;
using LIB.Warehouses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LIB.API
{
    public interface IAPI : IBaseServices<ProductEntity, long>
    {
        /// <summary>
        /// 1: thành công
        /// 3: thất bại
        /// </summary>
        /// <param name="WarehouseId"> Id warehouse</param>
        /// <param name="apiConnectService">Link api</param>
        /// <param name="warehousesService"></param>
        /// <param name="productService"></param>
        /// <returns></returns>
        Task<ResultMessageModel> API_Warehouses_SyncNewProduct(int WarehouseId, IApiConnect apiConnectService, IWarehouses warehousesService, IProduct productService);
        /// <summary>
        /// 999: thành công
        /// 1: Không lấy được Token!
        /// 2: Không có sản phẩm cần đồng bộ!
        /// 3: có lỗi xảy ra bên warehouse
        /// 4: Lỗi call api
        /// 5: Exeption Có lỗi xảy ra
        /// </summary>
        /// <param name="WarehouseId"></param>
        /// <returns></returns>
        Task<ResultMessageModel> API_Warehouses_SyncDataProduct(int WarehouseId, IApiConnect apiConnectService, IWarehouses warehousesService, IOrder orderService);
        bool API_Warehouses_SyncWarehouse(IApiConnect apiConnectService);

        /// <summary>
        ///-1: Không kiểm tra được
        /// 0: Không tồn tại
        /// 1: Hết hạn
        /// 2: chưa áp dụng
        /// 3: chưa đủ điều kiện
        /// 4: dùng được
        /// </summary>
        /// <param name="couponCode"></param>
        /// <param name="TotalPrice"></param>
        /// <param name="TotalPriceNew"></param>
        /// <returns></returns>
        int API_Momart_GetDiscount(string couponCode, decimal TotalPrice, ref DiscountResult discount, ref DiscountModel discountObj);
        /// <summary>
        /// check cardnumber: 
        /// 0: chưa cấp tài khoản 
        /// 1: Sử dụng được
        /// 2: không sử dụng được
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        bool API_Momart_CheckCardNumber(CustomerEntity customer, ref object message);
        ResultMessageModel API_Momart_GetCardNumbers();
        bool API_Momart_SaveCardNumber(CustomerEntity customer, ref string message);
        bool API_Momart_CheckToInsert(string CardNumberId);


    }
}
