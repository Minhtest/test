using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Authorize;

namespace WebApplication.Helpers
{
    public class MenuModel
    {
        public int? Id { get; set; }
        public string Text { get; set; }
        public string URL { get; set; }
        public int? ParentId { get; set; }
        public string ClassIcon { get; set; }
        public int? IdCategory { get; set; }
        public int Category { get; set; }
        public string Shortcut { get; set; }
    }
    public class GenMenu
    {
        public static List<MenuModel> GetListMenu()
        {
            List<MenuModel> listMenu = new List<MenuModel>();
            var Account = HttpContext.Current.User as CustomPrincipal;
            listMenu.Add(new MenuModel()
            {
                Text = "DANH MỤC",
                IdCategory = 1
            });
            listMenu.Add(new MenuModel()
            {
                Text = "BÁO CÁO THỐNG KÊ",
                IdCategory = 2
            });
            listMenu.Add(new MenuModel()
            {
                Text = "QUẢN TRỊ",
                IdCategory = 3
            });
            listMenu.Add(new MenuModel()
            {
                Text = "HỆ THỐNG",
                IdCategory = 4
            });
          
            if (AccesUser.checkAccess(new ActionModule[] { ActionModule.Order }, new ActionType[] { ActionType.View }, Account))
            {
                listMenu.Add(new MenuModel()
                {
                    Id = 1,
                    Text = "Danh sách đơn hàng",
                    URL = "/Orders/Index",
                    ParentId = null,
                    ClassIcon = "fas fa-list",
                    Category = 1,
                    Shortcut = "Ctrl+Q"
                });
            }
            if (AccesUser.checkAccess(new ActionModule[] { ActionModule.Order }, new ActionType[] { ActionType.Add }, Account))
            {
                listMenu.Add(new MenuModel()
                {
                    Id = 2,
                    Text = "Tạo đơn hàng",
                    URL = "/Orders/Create",
                    ParentId = null,
                    ClassIcon = "fas fa-edit",
                    Category = 1,
                    Shortcut = "Ctrl+B"
                });
            }
            if (AccesUser.checkAccess(new ActionModule[] { ActionModule.OrderPromotion }, new ActionType[] { ActionType.View }, Account))
            {
                listMenu.Add(new MenuModel()
                {
                    Id = 3,
                    Text = "Tặng hàng khuyến mại",
                    URL = "/OrderPromotion/Index",
                    ParentId = null,
                    ClassIcon = "fas fa-paw",
                    Category = 1
                });
            }
            if (AccesUser.checkAccess(new ActionModule[] { ActionModule.Category }, new ActionType[] { ActionType.View }, Account))
            {
                listMenu.Add(new MenuModel()
                {
                    Id = 3,
                    Text = "Quản lý danh mục",
                    URL = "/Category/Index",
                    ParentId = null,
                    ClassIcon = "fas fa-suitcase",
                    Category = 1
                });
            }              
            if (AccesUser.checkAccess(new ActionModule[] { ActionModule.Product }, new ActionType[] { ActionType.View }, Account))
            {
                listMenu.Add(new MenuModel()
                {
                    Id = 4,
                    Text = "Sản phẩm",
                    URL = "/Products/Index",
                    ParentId = null,
                    ClassIcon = "fas fa-th",
                    Category = 1
                });
            }
            if (AccesUser.checkAccess(new ActionModule[] { ActionModule.Customer }, new ActionType[] { ActionType.View }, Account))
            {
                listMenu.Add(new MenuModel()
                {
                    Id = 5,
                    Text = "Quản lý khách hàng",
                    URL = null,
                    ParentId = null,
                    ClassIcon = "fas fa-address-card",
                    Category = 1
                });
                listMenu.Add(new MenuModel()
                {
                    Id = 16,
                    Text = "Danh sách khách hàng",
                    URL = "/Customer/Index",
                    ParentId = 5,
                    ClassIcon = "",
                    Category = 1
                });
                listMenu.Add(new MenuModel()
                {
                    Id = 17,
                    Text = "Thẻ thành viên",
                    URL = "/Cardnumbers/Index",
                    ParentId = 5,
                    ClassIcon = "",
                    Category = 1
                });
            }
            if (AccesUser.checkAccess(new ActionModule[] { ActionModule.Return }, new ActionType[] { ActionType.View }, Account))
            {
                listMenu.Add(new MenuModel()
                {
                    Id = 6,
                    Text = "Trả hàng",
                    URL = "/Return/Index",
                    ParentId = null,
                    ClassIcon = "fas fa-angle-double-right",
                    Category = 1
                });
            }
            if (AccesUser.checkAccess(new ActionModule[] { ActionModule.Report }, new ActionType[] { ActionType.View }, Account))
            {
                listMenu.Add(new MenuModel()
                {
                    Id = 7,
                    Text = "Báo cáo chi tiết tổng hợp",
                    URL = "/Report/DetailGeneral",
                    ParentId = null,
                    ClassIcon = "fas fa-book-open",
                    Category = 2
                });
                listMenu.Add(new MenuModel()
                {
                    Id = 8,
                    Text = "Báo cáo hóa đơn",
                    URL = "/Orders/Report",
                    ParentId = null,
                    ClassIcon = "fas fa-book-open",
                    Category = 2
                });

                listMenu.Add(new MenuModel()
                {

                    Id = 10,
                    Text = "Báo cáo sản phẩm",
                    URL = "/Products/Report",
                    ParentId = null,
                    ClassIcon = "fas fa-book-open",
                    Category = 2
                });
            }
            if (AccesUser.checkAccess(new ActionModule[] { ActionModule.Template }, new ActionType[] { ActionType.View }, Account))
            {
                listMenu.Add(new MenuModel()
                {
                    Id = 11,
                    Text = "Template",
                    URL = "/Template",
                    ParentId = null,
                    ClassIcon = "fas fa-file-excel",
                    Category = 4
                });
            }
            
            if (AccesUser.checkAccess(new ActionModule[] { ActionModule.User }, new ActionType[] { ActionType.View }, Account))
            {
                listMenu.Add(new MenuModel()
                {
                    Id = 12,
                    Text = "Q.lý người dùng",
                    URL = "/Users",
                    ParentId = null,
                    ClassIcon = "fas fa-users-cog",
                    Category = 3
                });
            }
            if (AccesUser.checkAccess(new ActionModule[] { ActionModule.Role }, new ActionType[] { ActionType.View }, Account))
            {
                listMenu.Add(new MenuModel()
                {
                    Id = 13,
                    Text = "Phân quyền",
                    URL = "/RoleModule",
                    ParentId = null,
                    ClassIcon = "fas fa-check-square",
                    Category = 3
                });
            }
            if (AccesUser.checkAccess(new ActionModule[] { ActionModule.ShowRoom }, new ActionType[] { ActionType.View }, Account))
            {
                listMenu.Add(new MenuModel()
                {
                    Id = 14,
                    Text = "Quản lý showroom",
                    URL = "/Warehouses",
                    ParentId = null,
                    ClassIcon = "fas fa-store",
                    Category = 3
                });
            }
            if (AccesUser.checkAccess(new ActionModule[] { ActionModule.ShowRoom }, new ActionType[] { ActionType.Edit }, Account) && !Account.IsSupperAdmin)
            {
                listMenu.Add(new MenuModel()
                {
                    Id = 15,
                    Text = "My Showroom",
                    URL = "/Warehouses/Detail",
                    ParentId = null,
                    ClassIcon = "fas fa-store",
                    Category = 3
                });
            }
            if (AccesUser.checkAccess(new ActionModule[] { ActionModule.Template }, new ActionType[] { ActionType.View }, Account) &&( Account.IsSupperAdmin || Account.IsAdmin))
            {
                listMenu.Add(new MenuModel()
                {
                    Id = 16,
                    Text = "Đồng bộ",
                    URL = "/SyncSetting",
                    ParentId = null,
                    ClassIcon = "fas fa-sync",
                    Category = 4
                });
            }
            return listMenu;
        }
    }
}