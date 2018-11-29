$(document).ready(function () {
    $("#drdWarehouse").select2();
    $("#drdWarehouse").on("change", function () {
        $("#tblReturn").bootstrapTable("refresh");
    });
    $("#tblReturn").bootstrapTable({
        url: "/Return/GetData",
        method: "POST",
        ajax: function (config) {
            app.component.Loading.Show();
            _AjaxPost(config.url, {
                obj: config.data,
                WarehouseId: $("#drdWarehouse").val(),
            }, function (rs) {
                app.component.Loading.Hide();
                config.success({
                    total: rs.total,
                    rows: rs.datas
                });
            });
        },
        striped: true,
        sidePagination: 'server',
        pagination: true,
        paginationVAlign: 'both',
        limit: 10,
        pageSize: 10,
        pageList: [10, 25, 50, 100, 200],
        search: true,
        showColumns: true,
        showRefresh: true,
        minimumCountColumns: 2,
        toolbar: "#toolbar",
        columns: [
            {
                title: 'Thao Tác',
                align: 'center',
                valign: 'middle',
                formatter: function (value, row, index) {
                    var htmlCell = '<a href="/Return/Detail/' + row.Id + '"><i class="fas fa-eye"></i></a>';
                    return htmlCell;
                    //var htmlCell = '<a href="/Return/Edit/' + row.Id + '"><i class="fas fa-edit"></i></a>';
                    //if (row.IsDel != 0) {
                    //    htmlCell += '<a href="javascript:void(0)" style="margin-left:15px;" class="btnRemove"><i class="fas fa-trash-alt"></i></a>'
                    //}
                    //return htmlCell;
                },
                events: {
                    'click .btnRemove': function (e, value, row, index) {
                        modal.DeleteComfirm({
                            callback: function () {
                                console.log(row.Id);
                                _AjaxPost("/Return/Delete", { Id: row.Id }, function (rs) {
                                    if (rs.success) {
                                        
                                        notifySuccess("Xóa thành công!")
                                        $('#tblReturn').bootstrapTable('refresh')
                                    }
                                    else notifyError("Xóa thất bại!");
                                });
                            }
                        });
                    }
                }
            },
            {
                field: 'CreatedDate',
                title: 'Thời gian trả',
                align: 'center',
                valign: 'middle',
                searchable: false,
                sortable: true,
                formatter: function (value, row, index) {
                    if (value == null) {
                        return '';
                    }
                    else return formatToDateTime(value);
                }
            },
            {
                field: 'OrderCode',
                title: 'Mã hóa đơn',
                align: 'center',
                valign: 'middle',
                sortable: true
            },
            {
                field: 'CustomerName',
                title: 'Khách hàng',
                align: 'left',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    if (value === null) {
                        return 'Khách Vãng Lai';
                    }
                    else return value;
                }
            },
            {
                field: 'OrderDate',
                title: 'Thời gian mua hàng',
                align: 'center',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    if (value == null) {
                        return '';
                    }
                    else return formatToDateTime(value);
                }
            },
            {
                field: 'CreatedByName',
                title: 'Người thực hiện',
                align: 'left',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    if (value == null) {
                        return '';
                    }
                    else return formatNumber(value);
                }
            }
        ]
    })
});