var OrderReport = {
    load: function () {
    },
    component: {
    }
}
$(document).ready(function () {
    app.component.DatePicker();
    $("#btnPrint").click(function () {
        _AjaxPost("/Orders/PrintOrderReport", {
            startDate: GetValueDate($("#startdate").datepicker('getDate')),
            endDate: GetValueDate($("#enddate").datepicker('getDate')),
            opType: $("input[name='opType']:checked").val(),
            WarehouseId: $("#drdWarehouse").val(),
            CustomerSearchId: $("#checkQuest[name='opQuest']").prop("checked") == true ? (-1) : $("#selectCustomer").val(),
            CreatedSearchId: $("#selectCreated").val(),
            opPayCash: $("#checkPayCash[name='opPayCash']").prop("checked"),
            opPayByCard: $("#checkPayByCard[name='opPayByCard']").prop("checked"),
        }, function (rs) {
            $("#BillContent").html(rs);
            app.component.FormattxtMore();
            //$("#barcodecontainer #barcode").html(DrawHTMLBarcode_Code128A($("#barcodecontainer #barcode").text(), "yes", "in", 0, 2.5, 1, "bottom", "center", "", "black", "white"));
            app.component.Loading.Show();
            var newWin = window.open('', 'In Báo Cáo Hóa Đơn');
            newWin.document.open();
            newWin.document.write('<html><head><title>In Báo Cáo Hóa Đơn</title></head><body onload="window.print()">' + $("#BillContent").html() + '</body></html>');
            newWin.document.close();
            setTimeout(function () { newWin.close(); app.component.Loading.Hide(); }, 10);
        });
    });
    $("#drdWarehouse").select2().on("change", function () {
        $("#tblOrdersReport").bootstrapTable("refresh");
    });
    $("#checkQuest[name='opQuest']").on('change', function () {
        $("#selectCustomer").val("null").trigger('change');
        $("#tblOrdersReport").bootstrapTable("refresh");
    });
    $("#checkPayCash[name='opPayCash']").on('change', function () {
        $("#tblOrdersReport").bootstrapTable("refresh");
    });
    $("#checkPayByCard[name='opPayByCard']").on('change', function () {
        $("#tblOrdersReport").bootstrapTable("refresh");
    });
    $("#selectCustomer").select2({
        placeholder: 'Nhập tên | Mã khách hàng | Số điện thoại khách hàng',
        minimumInputLength: 1,
        allowClear: true,
        width: "100%",
        ajax: {
            delay: 500,
            type: 'POST',
            url: '/Customer/AutoCompletedCustomer',
            data: function (params) {
                var query = {
                    term: params.term,
                    page: params.page || 1
                };
                return query;
            },
            dataType: 'json',
            processResults: function (data, params) {
                params.page = params.page || 1;
                return {
                    results: $.map(data.results, function (item) {
                        return {
                            id: item.Id + "",
                            text: item.Name,
                            CustomerId: item.Id,
                            Name: item.Name,
                            CustomerCode: item.CustomerCode,
                            Phone: item.Phone,
                            Point: item.Point
                        };
                    }),
                    pagination: {
                        more: params.page * 10 < data.total
                    }
                };
            }
        },
        "pagination": {
            "more": true
        },
        templateResult: function (data) {
            if (data.loading)
                return data.text;
            var $result = $("<span>" + data.Name + "</span>");
            $result.append(" <span class='badge'>" + data.Phone + "</span>");
            $result.append(" <span class='label label-primary'>" + data.CustomerCode + "</label>");
            return $result;
        }
    })
        .on('select2:select', function (evt) {

            var item = evt.params.data;
            if (item != null) {
                $("#checkQuest[name='opQuest']").prop("checked", false);
                $("#tblOrdersReport").bootstrapTable("refresh");
            }
        })
        .on('change', function (evt) {
            if ($(this).val() == null) {
                $("#tblOrdersReport").bootstrapTable("refresh");
            }
        })
    $("#selectCreated").select2({
        placeholder: 'Nhập tên | Tài khoản | Mã người dùng',
        minimumInputLength: 1,
        allowClear: true,
        width: "100%",
        ajax: {
            delay: 500,
            type: 'POST',
            url: '/Users/AutoCompletedUsers',
            data: function (params) {
                var query = {
                    term: params.term,
                    page: params.page || 1
                };
                return query;
            },
            dataType: 'json',
            processResults: function (data, params) {
                params.page = params.page || 1;
                return {
                    results: $.map(data.results, function (item) {
                        return {
                            id: item.Id + "",
                            text: item.FullName,
                            UserName: item.UserName,
                            UserCode: item.UserCode,
                        };
                    }),
                    pagination: {
                        more: params.page * 10 < data.total
                    }
                };
            }
        },
        "pagination": {
            "more": true
        },
        templateResult: function (data) {
            if (data.loading)
                return data.text;
            var $result = $("<span>" + data.text + "</span>");
            $result.append(" <span class='badge'>" + data.UserName + "</span>");
            $result.append(" <span class='label label-primary'>" + data.UserCode + "</label>");
            return $result;
        }
    })
        .on('select2:select', function (evt) {

            var item = evt.params.data;
            if (item != null) {
                $("#tblOrdersReport").bootstrapTable("refresh");
            }
        })
        .on('change', function (evt) {
            if ($(this).val() == null) {
                $("#tblOrdersReport").bootstrapTable("refresh");
            }
        })
    $("input[name='opType']").on("change", function () {
        var value = $(this).val();
        if (value == 3) {
            $("#customDate").show();
            $("#char").hide();
            $("#tblOrdersReport").bootstrapTable("refresh");
        }
        else {
            $("#char").show();
            $("#customDate").hide();
            $("#tblOrdersReport").bootstrapTable("refresh");
        }
    });

    $("#startdate").datepicker().on('changeDate', function (selected) {
        var minDate = new Date(selected.date.valueOf());
        if ($('#enddate').datepicker('getDate') <= minDate) {
            $("#enddate").datepicker("update", minDate);
        }
        $('#enddate').datepicker('setStartDate', minDate);
        $("#tblOrdersReport").bootstrapTable("refresh");
    });

    $("#enddate").datepicker().on('changeDate', function (selected) {
        var maxDate = new Date(selected.date.valueOf());
        if ($('#startdate').datepicker('getDate') >= maxDate) {
            $("#startdate").datepicker("update", maxDate);
        }
        $('#startdate').datepicker('setEndDate', maxDate);
        $("#tblOrdersReport").bootstrapTable("refresh");
    });

    var d = new Date();
    var firstMonth = "01/" + (d.getMonth() + 1) + "/" + d.getFullYear();
    var today = d.getDay() + "/" + (d.getMonth() + 1) + d.getFullYear();
    $("#startdate").datepicker("update", firstMonth);
    $("#enddate").datepicker("update", today);

    $("#startdate").datepicker().on('show', function (selected) {
        var minDate = new Date(selected.date.valueOf());
        $('#enddate').datepicker('setStartDate', minDate);
        $('#startdate').datepicker('setEndDate', $("#enddate").datepicker('getDate'));
    });

    $("#enddate").datepicker().on('show', function (selected) {
        var maxDate = new Date(selected.date.valueOf());
        $('#startdate').datepicker('setEndDate', maxDate);
        $('#enddate').datepicker('setStartDate', $("#startdate").datepicker('getDate'));
    });

    OrderReport.load();
    $("#tblOrdersReport").bootstrapTable({
        url: "/Orders/GetDataOrdersReport",
        method: "POST",
        ajax: function (config) {
            config.data.search = $("#txtsearch").val();

            var params = {
                obj: config.data,
                opType: $("input[name='opType']:checked").val(),
                startDate: GetValueDate($("#startdate").datepicker('getDate')),
                endDate: GetValueDate($("#enddate").datepicker('getDate')),
                CustomerSearchId: $("#checkQuest[name='opQuest']").prop("checked") == true? (-1) : $("#selectCustomer").val(),
                CreatedSearchId: $("#selectCreated").val(),
                opPayCash: $("#checkPayCash[name='opPayCash']").prop("checked"),
                opPayByCard: $("#checkPayByCard[name='opPayByCard']").prop("checked"),
                WarehouseId: $("#drdWarehouse").val()
            };
            _AjaxPost(config.url, params, function (rs) {
                if (rs.success) {
                    config.success({
                        total: rs.total,
                        rows: rs.data
                    });
                    $("#sumGrandTodal").text(formatMoney(rs.summary));
                }
                else {
                    notifyError("Lấy dữ liệu thất bại!");
                    console.log(rs.status);
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
        search: false,
        showColumns: false,
        showRefresh: false,
        minimumCountColumns: 2,
        toolbar: "#toolbar",
        //detailView: true,
        //detailFormatter: function (index, row, element) {
        //    return '';
        //},
        columns: [
            {
                field: 'OrderCode',
                title: 'Mã hóa đơn',
                align: 'center',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    return '<div class="orderdetail" style="color:green;cursor:pointer;text-decoration:underline" data-toggle="modal" data-target="#OrderDetailModal">' + value + '</div>';
                },
                events: {
                    'click .orderdetail': function (e, value, row, index) {
                        $("#OrderDetailModal .modal-header .modal-title").html("Chi tiết hóa đơn <span style='color:red'>" + row.OrderCode + "</span> - " + formatToDateTime(row.OrderDateTime));
                        _AjaxPost("/Orders/GetOrderDetailByOrderId", { OrderId: row.OrderId }, function (rs) {
                            $("#OrderDetailModal .tblOrderDetail").bootstrapTable("load", rs.datas);
                            if (rs.dataspro.length == 0) {
                                $("#OrderDetailModal .order-pro").hide();
                            }
                            else $("#OrderDetailModal .order-pro").show();
                            console.log(rs.dataspro);
                            $("#OrderDetailModal .tblProductPromotion").bootstrapTable("load", rs.dataspro);
                        })
                    }
                }
            },
            {
                field: 'OrderDateTime',
                title: 'Thời gian',
                align: 'center',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    if (value == null) {
                        return '';
                    }
                    else {
                        return formatToDateTime(value);
                    }
                },
            },
            {
                field: 'CustomerName',
                title: 'Khách hàng',
                align: 'left',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    if (value == null) {
                        return 'Khách Vãng Lai';
                    }
                    else return value;
                },
            },
            {
                field: 'CreatedByName',
                title: 'Người tạo',
                align: 'left',
                valign: 'middle',
                sortable: true
            },
            {
                field: 'ProductTotal',
                title: 'Thành tiền',
                align: 'right',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    return formatMoney(value);
                },
            },
            {
                field: 'Discount',
                title: 'Giảm giá',
                align: 'right',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    if (value == 0) {
                        return '';
                    }
                    else return formatMoney(value);
                },
            },
            {
                field: 'PointUsed',
                title: 'Trả bằng điểm',
                align: 'right',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    if (value == null) {
                        return '';
                    }
                    else return formatMoney(value);
                },
            },
            {
                field: 'GrandTotal',
                title: 'Tổng tiền',
                align: 'right',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    return formatNumber(value);
                },
            },
            {
                field: 'PayCash',
                title: 'Trả tiền mặt',
                align: 'right',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    if (value == null) {
                        return '';
                    }
                    else {
                        return formatMoney(value);
                    }
                },
            },
            {
                field: 'PayByCard',
                title: 'Trả thẻ',
                align: 'right',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    if (value == null) {
                        return '';
                    }
                    else {
                        return formatMoney(value);
                    }
                },
            },
            {
                field: 'RefundMoney',
                title: 'Tiền trả khách',
                align: 'right',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    if (value > 0) {
                        return formatMoney(value);
                    }
                    else {
                        return formatNegInt(value);
                    }
                },
            },
            {
                field: 'TotalPriceReturn',
                title: 'Tổng tiền trả hàng',
                align: 'right',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    if (value == null) {
                        return '';
                    }
                    else {
                        return '<a href="/Return/Edit/' + row.ReturnId + '" target="_blank" style="color:green;cursor:pointer;text-decoration:underline">' + formatMoney(value) + '</a>';
                    }
                },
            }
            
        ]
    });

    $("#btnSearch").on("click", function () {
        $("#tblOrdersReport").bootstrapTable("refresh");
    });
    $("#btnExportExcel").on("click", function () {
        _AjaxPost("/Orders/ExportReport", {
            txtSearch: $("#txtsearch").val(),
            startDate: GetValueDate($("#startdate").datepicker('getDate')),
            endDate: GetValueDate($("#enddate").datepicker('getDate')),
            opType: $("input[name='opType']:checked").val(),
            WarehouseId: $("#drdWarehouse").val(),
            CustomerSearchId: $("#checkQuest[name='opQuest']").prop("checked") == true ? (-1) : $("#selectCustomer").val(),
            CreatedSearchId: $("#selectCreated").val(),
            opPayCash: $("#checkPayCash[name='opPayCash']").prop("checked"),
            opPayByCard: $("#checkPayByCard[name='opPayByCard']").prop("checked"),
        }, function (rs) {
            if (rs.success) {
                notifySuccess("Xuất báo cáo thành công!");
                SaveFileAs(rs.urlFile, rs.fileName);
            }
            else {
                notifyError("Xuất báo cáo thất bại!");
                console.log(rs.message);
            }
        });
    });
    $("#OrderDetailModal .tblOrderDetail").bootstrapTable({
        showFooter: true,
        columns: [
            {
                field: 'ProductName',
                title: 'Tên sản phẩm',
                align: 'left',
                valign: 'middle',
                sortable: true
            },
            {
                field: 'Barcode',
                title: 'Barcode',
                align: 'center',
                valign: 'middle',
                sortable: true
            },
            {
                field: 'Quantity',
                title: 'Số lượng',
                align: 'center',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    return formatNumber(value);
                },
            },
            {
                field: 'SellPrice',
                title: 'Giá bán',
                align: 'right',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    return formatMoney(value);
                },
            },
            {
                field: 'Discount',
                title: 'Chiết khấu',
                align: 'right',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    return formatPercent(value);
                },
                footerFormatter: function () {
                    return 'Tổng thành tiền: ';
                }
            },
            {
                field: 'TotalPrice',
                title: 'Thành tiền',
                align: 'right',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    return formatNumber(value);
                },
                footerFormatter: function (data) {
                    var totalPrice = 0;
                    data.forEach(function (item) {
                        totalPrice += item.TotalPrice;
                    });
                    return '<b>' + formatMoney(totalPrice) + '</b>';
                }
            }
        ]
    });
    $("#OrderDetailModal .tblProductPromotion").bootstrapTable({

        columns: [
            {
                field: 'ProductName',
                title: 'Sản phẩm khuyến mại',
                align: 'left',
                valign: 'middle',
                sortable: true
            },
            {
                field: 'Barcode',
                title: 'Barcode',
                align: 'center',
                valign: 'middle',
                sortable: true
            },
            {
                field: 'Quantity',
                title: 'Số lượng',
                align: 'center',
                valign: 'middle',
                sortable: true
            }
        ]
    });
});