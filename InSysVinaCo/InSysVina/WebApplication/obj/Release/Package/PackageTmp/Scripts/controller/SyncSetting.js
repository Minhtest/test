$(document).ready(function () {

    $("#frmWarehouses").on("submit", function () {
        var data = $(this).serialize();
        $.ajax({
            url: "/Warehouses/CreateOrEdit",
            type: "POST",
            data: data,
            success: function (rs) {
                if (rs.success) {
                    notifySuccess("Thêm kho hàng thành công!");
                    $('#createWarehouseModal').modal('hide');
                    $('#tableWarehouse').bootstrapTable('refresh')

                }
                else {
                    notifyError("Lỗi thêm mới kho hàng!");
                }
            },
            error: function () {
                notifyError("Lỗi truy cập!");
            }
        });
        return false;
    });
    $('#tableApiConnect').bootstrapTable({
        url: "/SyncSetting/GetData",
        method: "POST",
        ajax: function (config) {
            _AjaxPost(config.url, config.data, function (rs) {
                if (rs.success) {
                    config.success({
                        total: rs.total,
                        rows: rs.datas
                    });
                }
                else {
                    notifyError("Lấy dữ liệu thất bại!");
                    console.log(rs.error);
                }
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
                title: 'Thao tác',
                align: 'center',
                valign: 'middle',
                searchable: false,
                formatter: function (value, row, index) {
                    return '\
                            <a class="edit ml10" href="'+ '/SyncSetting/Edit/' + row.Id + '" title="Cập nhật">\
                                <i class="glyphicon glyphicon-edit"></i>\
                            </a>'+ '<a> <i class="btnDelete	glyphicon glyphicon-trash" title="Xoá"></i> </a>';
                },
                events: {
                    'click .btnDelete': function (e, value, row, index) {
                        modal.DeleteComfirm({
                            message: "BẠN CÓ CHẮC MUỐN XOÁ API?",
                            callback: function () {
                                _AjaxPost("/SyncSetting/DeleteApiConnect", { Id: row['Id'] }, function (rs) {
                                    if (rs.success) {
                                        notifySuccess("Xóa THÀNH CÔNG!");
                                        $('#tableApiConnect').bootstrapTable("refresh");
                                    }
                                    else {
                                        notifyError("Xóa THẤT BẠI!");
                                    }
                                });
                            }
                        })
                    }
                }
            },
            {
                field: 'Name',
                title: 'Name',
                align: 'left',
                valign: 'middle',
                sortable: true
            },
            {
                field: 'Api',
                title: 'Địa chỉ',
                align: 'left',
                valign: 'middle',
                sortable: true
            },
            {
                field: 'UserName',
                title: 'UserName',
                align: 'left',
                valign: 'middle',
                sortable: true
            },
            {
                field: 'PassWord',
                title: 'Password',
                align: 'left',
                valign: 'middle',
                sortable: true
            },
            {
                field: 'Secret',
                title: 'Secret',
                align: 'left',
                valign: 'middle',
                sortable: true,

            },
            {
                field: 'Type',
                title: 'Type',
                align: 'right',
                valign: 'middle',
                sortable: true
            },
            {
                field: 'Client',
                title: 'Client',
                align: 'right',
                valign: 'middle',
                sortable: true
            },
            {
                field: 'Disabled',
                title: 'Trạng thái',
                align: 'center',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    if (!value) {
                        return 'Hoạt động';
                    }
                    else return 'Huỷ';
                }
            }
        ]
    });
   
});