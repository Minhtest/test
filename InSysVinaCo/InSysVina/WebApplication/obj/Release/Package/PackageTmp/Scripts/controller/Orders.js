(function (window, $) {
    window.Orders = {
        ui: {
            tblOrders: $("#tblOrders")
        },
        init: function () {
            Orders.CreateTable();
        },
        CreateTable: function () {
            Orders.ui.tblOrders.bootstrapTable({
                url: "/Orders/GetDataOders",
                method: "POST",
                ajax: function (config) {
                    app.component.Loading.Show();
                    _AjaxPost(
                        config.url,
                        {
                            obj: config.data,
                            WarehouseId: $("#drdWarehouse").val(),
                            getGuest: $("#checkboxSortGuest").prop('checked')
                        },
                        function (rs) {
                            app.component.Loading.Hide();
                            if (rs.success) {
                                config.success({
                                    total: rs.total,
                                    rows: rs.data
                                });
                            }
                            else {
                                notifyError("Lấy dữ liệu thất bại!");
                                console.log(rs.error);
                            }
                        }
                    );
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
                        formatter: function (value, row, index) {
                            if (row['Status'] === 0) {
                                return '<a href="/Orders/Edit/' + row.Id + '" title="Cập nhật đơn hàng"><i class="fas fa-edit"></i></a>' +
                                    '<a href="javascript:void(0)" class="btnDelete" style="margin-left:15px;" title="Hủy đơn hàng"><i class="fas fa-trash-alt"></i></a>';
                            }
                            else {
                                return '<a target="_blank" href="/Orders/Detail/' + row.Id + '" title="Xem đơn hàng"><i class="fas fa-eye"></i></a>';
                            }
                        },
                        events: {
                            'click .btnDelete': function (e, value, row, index) {
                                modal.DeleteComfirm({
                                    message: "BẠN CÓ CHẮC MUỐN HỦY ĐƠN HÀNG?",
                                    callback: function () {
                                        _AjaxPost("/Orders/DeleteOrder", { Id: row['Id'] }, function (rs) {
                                            if (rs.kq) {
                                                notifySuccess("Xóa đơn hàng THÀNH CÔNG!");
                                                Orders.ui.tblOrders.bootstrapTable("refresh");
                                            }
                                            else {
                                                notifyError("Xóa đơn hàng THẤT BẠI!");
                                            }
                                        });
                                    }
                                })
                            }
                        }
                    },
                    {
                        field: 'OrderCode',
                        title: 'Mã đơn hàng',
                        align: 'left',
                        valign: 'middle',
                        searchable: false,
                        sortable: true
                    },
                    {
                        field: 'Status',
                        title: 'Trạng Thái',
                        align: 'center',
                        valign: 'middle',
                        searchable: false,
                        sortable: true,
                        formatter: function (value, row, index) {
                            if (value === 0) {
                                return '<b><a href="/Orders/Edit/' + row.Id + '" style="color:red"><u>Chờ Thanh Toán</u></a></b>';
                            }
                            else {
                                return '<i><a target="_blank" href="/Orders/Detail/' + row.Id + '" style="color:green">Đã Thanh Toán</a></i>';
                            }

                        }
                    },
                    {
                        field: 'OrderDate',
                        title: 'Thời Gian',
                        align: 'center',
                        valign: 'middle',
                        searchable: false,
                        sortable: true,
                        formatter: function (value, row, index) {
                            if (value == null) return formatToDateTime(row.CreatedDate);
                            else return formatToDateTime(value);
                        }
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
                        field: 'ProductTotal',
                        title: 'Thành Tiền',
                        align: 'right',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            return formatMoney(value);
                        },
                    },
                    {
                        field: 'CouponCode',
                        title: 'Coupon',
                        align: 'center',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            if (value != null) {
                                var txt = value;
                                if (row.CouponType == 1) {
                                    txt += ' [' + row.CouponValue + ' VND]';
                                }
                                if (row.CouponType == 2) {
                                    txt += ' [' + row.CouponValue + '% ~ ' + formatNumber(row.Discount) + ']';
                                }
                                return txt;
                            }
                            else {
                                return "";
                            }
                        }
                    },
                    {
                        field: 'Voucher',
                        title: 'Voucher',
                        align: 'right',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            if (value != null) {
                                return formatMoney(value);
                            }
                            else {
                                return '';
                            }
                        }
                    },
                    {
                        field: 'PointUsed',
                        title: 'Trả bằng điểm',
                        align: 'right',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            if (value == null) return '';
                            else return formatNumber(value);
                        },
                    },
                    {
                        field: 'GrandTotal',
                        title: 'Tổng tiền',
                        align: 'right',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            return formatMoney(value);
                        },
                    },
                    {
                        field:'PayCash',
                        title: 'Trả tiền mặt',
                        align: 'right',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            if (row.PayCash == null) return '';
                            else return formatMoney(value);
                        },
                    },
                    {
                        field: 'PayByCard',
                        title: 'Trả thẻ',
                        align: 'right',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            if (row.PayByCard == null) return '';
                            else return formatMoney(value);
                        },
                    },
                    {
                        field: 'RefundMoney',
                        title: 'Tiền trả khách',
                        align: 'right',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            return formatNumber(value);
                        },
                    }
                ]

            });
        }
    }

})(window, jQuery);
$(document).ready(function () {
    Orders.init();
    $("#drdWarehouse").select2();
    $("#drdWarehouse").on("change", function () {
        Orders.ui.tblOrders.bootstrapTable("refresh");
    });
    $("#btnSortNoPay").on("click", function () {
        Orders.ui.tblOrders.bootstrapTable("refreshOptions", {
            sortName: "Status",
            sortOrder: "asc"
        });
    });
    $("#btnSortTotal").on("click", function () {
        Orders.ui.tblOrders.bootstrapTable("refreshOptions", {
            sortName: "GrandTotal",
            sortOrder: "desc"
        });
    });
    $("#checkboxSortGuest").on("change", function () {
        Orders.ui.tblOrders.bootstrapTable("refresh");
    });
    $("#btnSync").on('click', function () {
        app.component.Loading.Show();
        _AjaxPost('/Orders/Sync', { WarehouseId: $("#drdWarehouse").val() }, function (rs) {
            app.component.Loading.Hide();
            notify(rs);
        })
    })
});