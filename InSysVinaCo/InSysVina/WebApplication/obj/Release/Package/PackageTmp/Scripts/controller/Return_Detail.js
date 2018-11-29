$(document).ready(function () {
    $("#tblDataProduct").bootstrapTable({
        url: "/Return/GetReturnDetail",
        method: "POST",
        ajax: function (config) {
            app.component.Loading.Show();
            _AjaxPost(config.url, {
                ReturnId: $("#Id").val()
            }, function (rs) {
                app.component.Loading.Hide();
                config.success({
                    total: rs.total,
                    rows: rs.data
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
        showColumns: true,
        showRefresh: true,
        minimumCountColumns: 2,
        columns: [
            {
                field: 'ProductName',
                title: 'Tên sản phẩm',
                align: 'left',
                valign: 'middle',
                sortable: false
            },
            {
                field: 'Barcode',
                title: 'Barcode',
                align: 'left',
                valign: 'middle',
                sortable: false
            },

            {
                field: 'SellPrice',
                title: 'Giá bán (VND)',
                align: 'right',
                valign: 'middle',
                sortable: false,
                formatter: function (value, row, index) {
                    return formatMoney(value);
                }
            },
            {
                field: 'Discount',
                title: 'Chiết khấu (VND)',
                align: 'right',
                valign: 'middle',
                sortable: false,
                formatter: function (value, row, index) {
                    return formatPercent(value);
                }
            },

            {
                field: 'QuantityReturn',
                title: 'Số lượng trả',
                align: 'center',
                valign: 'middle',
                sortable: false,


            },
            {
                field: 'PriceReturn',
                title: 'Giá Trả (VND)',
                align: 'right',
                valign: 'middle',
                sortable: false,
                formatter: function (value, row, index) {
                    return formatMoney(row.PriceReturn / row.QuantityReturn);
                },

            },
            {
                field: 'TotalPriceReturn',
                title: 'Thành Tiền Trả (VND)',
                align: 'right',
                valign: 'middle',
                sortable: false,
                formatter: function (value, row, index) {
                    return '<span class="totalpricereturn">' + formatMoney(row.PriceReturn) + '</span>';
                },

            },
            {
                field: 'Reason',
                title: 'Lý do trả hàng',
                valign: 'middle',
                sortable: false,
                
            }
        ],

    });
});