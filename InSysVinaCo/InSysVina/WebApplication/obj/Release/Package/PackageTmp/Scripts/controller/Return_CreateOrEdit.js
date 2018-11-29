$(document).ready(function () {
    $("#drdWarehouse").select2();
    $("#drdWarehouse").on("change", function () {
        $("#drdOrder").val('').trigger('change');
        $("#tblDataProduct").bootstrapTable('removeAll');

    });
    $("#drdOrder").select2({
        placeholder: 'Nhập mã hóa đơn',
        multiple: false,
        minimumInputLength: 1,
        width: "100%",
        ajax: {
            delay: 500,
            type: 'POST',
            url: '/Return/AutoCompletedOrder',
            data: function (params) {
                var query = {
                    term: params.term,
                    page: params.page || 1
                };
                return {
                    param: query,
                    WarehouseId: $("#drdWarehouse").val()
                };
            },
            processResults: function (rs, params) {
                params.page = params.page || 1;
                var datas = $.map(rs.results, function (item) {
                    return {
                        Id: item.Id,
                        id: item.Id,
                        ProductId: item.Id,
                        text: item.OrderCode,
                        OrderCode: item.OrderCode,
                        CreatedDate: formatToDateTime(item.CreatedDate)
                    };
                });
                return {
                    results: datas,
                    pagination: {
                        more: params.page * 10 < rs.total
                    }
                }
            }
        },
        "pagination": {
            "more": true
        },
        templateResult: function (data) {
            if (data.loading)
                return data.text;
            var $result = $("<span>" + data.OrderCode + "</span>");
            $result.append(" <span class='label label-primary'>" + data.CreatedDate + "</label>");
            return $result;
        }

    })
        .on('select2:select', function (evt) {
            var item = evt.params.data;
            if (item != null) {
                _AjaxPost("/Return/GetReturnDetailByOrderId", {
                    OrderId: item.Id,
                    ReturnId: $("#Id").val(),
                    WarehouseId: $("#drdWarehouse").val()
                }, function (rs) {
                    if (rs.success) {
                        if (rs.returnEntity != null && rs.ReturnId == null) {
                            window.location.href = "/Return/Edit/" + rs.returnEntity.Id + "";
                        }
                        else {
                            var datas = $.map(rs.data, function (item) {
                                return {
                                    ProductId: item.ProductId,
                                    ProductName: item.ProductName,
                                    Barcode: item.Barcode,
                                    Quantity: item.Quantity,
                                    SellPrice: item.SellPrice,
                                    Discount: item.Discount,
                                    TotalPrice: item.Quantity * item.SellPrice - item.Quantity * item.SellPrice * item.Discount / 100,
                                    QuantityReturn: item.ReturnId == null ? 0 : item.QuantityReturn,
                                    PriceReturn: item.ReturnId == null ? item.SellPrice : item.PriceReturn,
                                    TotalPriceReturn: item.ReturnId == null ? 0 : (item.QuantityReturn * item.Quantity),
                                    Reason: item.Reason
                                }
                            });
                            $("#tblDataProduct").bootstrapTable("load", datas)
                        }
                    }
                    else {
                        notifyError("Đơn hàng không tồn tại!");
                    }
                });
            }
        })
    if ($("#OrderId").val() != 0) {
        $("#drdOrder").select2("trigger", "select", {
            data: {
                Id: $("#OrderId").val(),
                id: $("#OrderId").val(),
                text: $("#OrderCode").val(),
                OrderCode: $("#OrderCode").val()
            }
        });
    }
    $("#tblDataProduct").bootstrapTable({
        striped: true,
        pagination: true,
        paginationVAlign: 'none',
        limit: 10,
        pageSize: 10,
        pageList: [10, 25, 50, 100, 200],
        search: false,
        showColumns: false,
        showRefresh: false,
        showFooter: true,
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
                field: 'Quantity',
                title: 'Số lượng bán',
                align: 'center',
                valign: 'middle',
                formatter: function (value, row, index) {
                    return formatNumber(value);
                }
            }, {
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
                field: 'TotalPrice',
                title: 'Thành Tiền Bán (VND)',
                align: 'right',
                valign: 'middle',
                sortable: false,
                formatter: function (value, row, index) {
                    return formatMoney(value);
                }
            },
            {
                field: 'QuantityReturn',
                title: 'Số lượng trả',
                align: 'center',
                valign: 'middle',
                sortable: false,
                formatter: function (value, row, index) {
                    return '<div class="input-group">' +
                        '<input type="text" ipPosInt class="form-control QuantityReturn" value="' + (value == null ? 0 : value) + '" data-v-Max=' + row.Quantity + ' style="width: 50px">' +
                        '<span class="input-group-addon">< ' + (row.Quantity + 1) + '</span ></div>';
                },
                footerFormatter: function (data) {
                    return 'Tổng Tiền';
                }
            },

            //{
            //    field: 'PriceReturn',
            //    title: 'Giá Trả (VND)',
            //    align: 'right',
            //    valign: 'middle',
            //    sortable: false,
            //    formatter: function (value, row, index) {
            //        return '<div class="input-group"><input type="text" ipPosInt class="form-control PriceReturn" value="' + (value == null ? row.SellPrice : value) + '"></div>';
            //    },
            //    footerFormatter: function (data) {
            //        return 'Tổng Tiền';
            //    }
            //},
            {
                field: 'TotalPriceReturn',
                title: 'Thành Tiền Trả (VND)',
                align: 'right',
                valign: 'middle',
                sortable: false,
                formatter: function (value, row, index) {
                    return '<span class="totalpricereturn">' + formatMoney(row.QuantityReturn * row.TotalPrice / row.Quantity) + '</span>';
                },
                footerFormatter: function (data) {
                    var sum = 0;
                    data.forEach(function (item) {
                        sum += (item.QuantityReturn * item.TotalPrice / item.Quantity);
                    });
                    return '<b id="txtSummary">' + formatMoney(sum) + '</b>';
                }
            },
            {
                field: 'Reason',
                title: 'Lý do trả hàng',
                valign: 'middle',
                width: '200px',
                sortable: false,
                formatter: function (value, row, index) {
                    return '<div class="input-group">'
                        + '<textarea ipPosText class="form-control Reason">' + (value == null ? "" : value + "") + '</textarea>'
                        + '</div>';
                },
            }
        ],
        onPostBody: function (data) {
            app.component.FormatInput();
        },
        onEditableShown: function (field, row, oldValue, $el) {
            switch (field) {
                case "PriceReturn": new AutoNumeric('.ipMoney', row[field], opMoney); break;
            }
        },
        onEditableSave: function (field, row, oldValue, $el) {
            switch (field) {
                case "PriceReturn": if (row["PriceReturn"] == '') { row["PriceReturn"] = 0 } else row["PriceReturn"] = parseInt(getMoney_AutoNumeric(row["PriceReturn"])); break;
            }
            row["TotalPriceReturn"] = row["Quantity"] * row["PriceReturn"];
            $("#tblDataProduct").bootstrapTable("updateRow", row);
        },
        onLoadSuccess: function () {

        }
    });

    $("#tblDataProduct").on("keyup", '.QuantityReturn[ipPosInt]', function () {
        var index = $(this).closest('tr').data("index");
        var data = $("#tblDataProduct").bootstrapTable("getData");
        data[index].QuantityReturn = getPosInt_AutoNumeric($(this).val());
        data[index].TotalPriceReturn = data[index].QuantityReturn * data[index].SellPrice - data[index].QuantityReturn * data[index].SellPrice * data[index].Discount / 100;
        $(this).closest('tr').find(".totalpricereturn").text(formatMoney(data[index].TotalPriceReturn));
        $("#tblDataProduct").bootstrapTable("resetView");
    });
    $("#tblDataProduct").on("mousewheel", '.QuantityReturn[ipPosInt]', function () {
        var index = $(this).closest('tr').data("index");
        var data = $("#tblDataProduct").bootstrapTable("getData");
        data[index].QuantityReturn = getPosInt_AutoNumeric($(this).val());
        data[index].TotalPriceReturn = data[index].QuantityReturn * data[index].SellPrice - data[index].QuantityReturn * data[index].SellPrice * data[index].Discount / 100;
        $(this).closest('tr').find(".totalpricereturn").text(formatMoney(data[index].TotalPriceReturn));
        $("#tblDataProduct").bootstrapTable("resetView");
    });
    $("#tblDataProduct").on("keyup", '.PriceReturn[ipPosInt]', function () {
        var index = $(this).closest('tr').data("index");
        var data = $("#tblDataProduct").bootstrapTable("getData");
        data[index].PriceReturn = getPosInt_AutoNumeric($(this).val());
        data[index].TotalPriceReturn = data[index].QuantityReturn * data[index].PriceReturn;
        $(this).closest('tr').find(".totalpricereturn").text(formatMoney(data[index].TotalPriceReturn));
        $("#tblDataProduct").bootstrapTable("resetView");
    });
    $("#tblDataProduct").on("mousewheel", '.PriceReturn[ipPosInt]', function () {
        var index = $(this).closest('tr').data("index");
        var data = $("#tblDataProduct").bootstrapTable("getData");
        data[index].PriceReturn = getPosInt_AutoNumeric($(this).val());
        data[index].TotalPriceReturn = data[index].QuantityReturn * data[index].PriceReturn;

        $(this).closest('tr').find(".totalpricereturn").text(formatMoney(data[index].TotalPriceReturn));
        $("#tblDataProduct").bootstrapTable("resetView");
    });

    $("#tblDataProduct").on("keyup", '.Reason[ipPosText]', function () {
        var index = $(this).closest('tr').data("index");
        var data = $("#tblDataProduct").bootstrapTable("getData");
        data[index].Reason = $(this).val();
        console.log(data[index]);
        $("#tblDataProduct").bootstrapTable("resetView");
    });

    $("#btnOk").click(function () {
        $(this).attr('disabled', 'disabled');
        app.component.Loading.Show();
        var data = $("#tblDataProduct").bootstrapTable("getData");
        if (data.length > 0) {
            var listDetail = $.map(data, function (item) {
                if (item.QuantityReturn > 0) {
                    return {
                        ProductId: item.ProductId,
                        QuantityReturn: item.QuantityReturn,
                        PriceReturn: item.PriceReturn,
                        Discount: item.Discount,
                        Reason: item.Reason
                    }
                }
            })
            if (listDetail.length > 0) {
                _AjaxPost("/Return/InsertOrUpdate", {
                    Id: $("#Id").val(),
                    OrderId: $("#drdOrder").val(),
                    WarehouseId: $("#drdWarehouse").val(),
                    listDetail
                }, function (rs) {
                    app.component.Loading.Hide();
                    if (rs.success) {
                        notifySuccess("Trả hàng thành công!");
                        setTimeout(function () {
                            window.location.href = "/Return";
                        }, 1000);
                    }
                    else notifyError("Trả hàng thất bại!");
                });
            }
            else {
                app.component.Loading.Hide();
                $(this).removeAttr('disabled');
                notifyError("Không có sản phẩm nào trả!");
            }
        }
        else {
            app.component.Loading.Hide();
            $(this).removeAttr('disabled');
            notifyError("Chưa chọn sản phẩm!");
        }
    });
});