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
    $('#tableWarehouse').bootstrapTable({
        url: "/Warehouses/GetDataWarehouses",
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
                title: 'Cập Nhật',
                align: 'center',
                valign: 'middle',
                searchable: false,
                formatter: function (value, row, index) {
                    return '\
                            <a class="edit ml10" href="'+ '/Warehouses/Edit/' + row.Id + '" title="Cập nhật showroom">\
                                <i class="glyphicon glyphicon-edit"></i>\
                            </a>';
                },
            },
            {
                field: 'Name',
                title: 'Showroom',
                align: 'left',
                valign: 'middle',
                sortable: true
            },
            {
                field: 'Address',
                title: 'Địa chỉ',
                align: 'left',
                valign: 'middle',
                sortable: true
            },
            {
                field: 'Phone',
                title: 'Số Điện Thoại',
                align: 'left',
                valign: 'middle',
                sortable: true
            },
            {
                field: 'Hotline',
                title: 'Hotline',
                align: 'left',
                valign: 'middle',
                sortable: true
            },
            {
                field: 'Website',
                title: 'Website',
                align: 'left',
                valign: 'middle',
                sortable: true,

            },
            {
                field: 'Email',
                title: 'Email nhận báo cáo',
                align: 'left',
                valign: 'middle',
                sortable: true,

            },
            {
                field: 'QuotaPromotion',
                title: 'Định mức khuyến mại sản phẩm',
                align: 'right',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    return formatMoney(value);
                }
            },
            {
                field: 'Logo',
                title: 'Logo',
                align: 'center',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    if (value == null) {
                        return '';
                    }
                    else return '<img src="' + value + '" height="50px"/>';
                }
            }
        ]
    });
    $('#btnSync').click(function () {
        app.component.Loading.Show();
        _AjaxPost("/Warehouses/Sync", null, function (rs) {
            app.component.Loading.Hide();
            console.log(rs);
            if (rs.success) {
                notifySuccess("Đồng bộ thành công!");
                $('#tableWarehouse').bootstrapTable('refresh');
            }
            else {
                notifyError("Đồng bộ thất bại");
            }
        })
    });
});