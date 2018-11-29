﻿using System.Collections.Generic;

namespace LIB
{
    public interface ICategory : IBaseServices<CategoryEntity, long>
    {
        List<CategoryEntity> GetDataWithPage(Select2Param obj, ref int total);
        List<CategoryEntity> GetAllWithLevel(string txtSearch);
        List<CategoryEntity> GetAllData();
        bool Insert_Update(CategoryEntity category);
        CategoryEntity GetByID(int Id);
        bool DeleteById(int Id);
        bool CheckExistCode(int? Id, string Code);
        bool SetOnHomePage(int cateId, int value);
        CategoryEntity GetByCode(string Code);
        List<CategoryEntity> ListAll();
        List<CategoryEntity> ListAllNotTree();
        List<CategoryEntity> ListConfig();
        //List<ConfigCateHomePageViewModel> ListProductConfig(int cateId);
        List<CategoryEntity> ListAllPaging(int pageIndex, int pageSize, ref int totalRow);
        List<CategoryEntity> BindTreeview();
        CategoryEntity ViewDetail(int id);
        List<CategoryEntity> ParentCate();
        List<CategoryEntity> ChildCate(int id = 0);
        List<CategoryEntity> GetTopHot();
        List<CategoryEntity> GetHotProductTreeView();
        List<CategoryEntity> SearchCategory(string keyword);
        bool UpdateExport(int id, string Name, string Code);
        bool UpdateExport(int id, string Name);
        bool CheckExistCate(string check);
        //List<ConfigCateHomePageViewModel> ListProductConfigNew(int cateid, int pageIndex, int pageSize, ref int totalRow);
    }
}
