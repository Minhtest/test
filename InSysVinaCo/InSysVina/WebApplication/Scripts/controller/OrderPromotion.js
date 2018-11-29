$(document).ready(function () {
    function hidedrdProduct() {
        if ($("#drdOrder").val() != null) {
            $(".product").show();
        }
        else {
            $(".product").hide();
        }
    }
    $("#drdWarehouse").select2();
    $("#drdWarehouse").on("change", function () {
        $("#tblData").bootstrapTable("refresh");
        $("#tblDataProduct").bootstrapTable("removeAll");
        $("#drdProduct").val(null).trigger('change');
        $("#drdOrder").val(null).trigger('change');
        $("#tblDataProductSell").bootstrapTable("removeAll");
        hidedrdProduct();
    });
    $("#tblData").bootstrapTable({
        url: "/OrderPromotion/GetData",
        method: "POST",
        ajax: function (config) {
            app.component.Loading.Show();
            _AjaxPost(config.url, {
                obj: config.data,
                WarehouseId: $("#drdWarehouse").val()
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
                    var htmlCell = '';
                    if (row.PromotedDate === null) {
                        htmlCell = '<a href="/OrderPromotion/Edit/' + row.Id + '"><i class="fas fa-edit"></i></a>';
                    }
                    else {
                        htmlCell = '<a  href="/OrderPromotion/Detail/' + row.Id + '"><i class="fas fa-eye"></i></a>';
                    }

                    if (row.PromotedDate == null) {
                        htmlCell += '<a href="javascript:void(0)" style="margin-left:15px;" class="btnRemove"><i class="fas fa-trash-alt"></i></a>'
                    }



                    return htmlCell;
                },
                events: {
                    'click .btnRemove': function (e, value, row, index) {
                        modal.DeleteComfirm({
                            callback: function () {
                                console.log(row.Id);
                                _AjaxPost("/OrderPromotion/Delete", { Id: row.Id }, function (rs) {
                                    if (rs.success) {

                                        notifySuccess("Xóa thành công!")
                                        $('#tblData').bootstrapTable('refresh')
                                    }
                                    else notifyError("Xóa thất bại!");
                                });
                            }
                        });
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
                field: 'PromotedDate',
                title: 'Trạng Thái',
                align: 'center',
                valign: 'middle',
                searchable: false,
                sortable: true,
                formatter: function (value, row, index) {
                    if (value === null) {
                        return '<b style="color:red">Chưa tặng khuyến mại</b>';
                    }
                    else {
                        return '<i style="color:green">Đã tặng khuyến mãi</i>';
                    }

                }
            },
            {
                field: 'OrderDate',
                title: 'Thời Gian Order',
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
                field: 'PromotedDate',
                title: 'Thời gian khuyến mại',
                align: 'center',
                valign: 'middle',
                searchable: false,
                sortable: true,
                formatter: function (value, row, index) {
                    if (value == null) return '';
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

                    return formatMoney(value);
                },
            },
            {
                field: 'PaidGuests',
                title: 'Khách Đưa',
                align: 'right',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    return formatMoney(row.PayCash + row.PayByCard);
                },
            },
            {
                field: 'RefundMoney',
                title: 'Trả Lại',
                align: 'right',
                valign: 'middle',
                sortable: true,
                formatter: function (value, row, index) {
                    return formatNumber(value);
                },
            }
        ]
    })
    $("#drdProduct").select2({
        placeholder: 'Tìm bằng barcode, tên sản phẩm và code sản phẩm...',
        multiple: false,
        minimumInputLength: 1,
        width: "100%",
        closeOnSelect: false,
        selectOnClose: false,
        allowClear: true,
        ajax: {
            delay: 500,
            type: 'POST',
            url: '/Products/AutoCompletedProduct',
            data: function (params) {
                app.component.Loading.Show();
                var query = {
                    term: params.term,
                    page: params.page || 1
                };
                return {
                    param: query,
                    WarehouseId: $("#drdWarehouse").val()
                };
            },
            success: function (rs) {
                if (rs.results.length == 1) {
                    setTimeout(function () {
                        $("#drdProduct").select2("close");
                        $("#drdProduct").select2("open");
                    }, 10);
                }
            },
            processResults: function (data, params) {
                app.component.Loading.Hide();
                params.page = params.page || 1;
                var datas = $.map(data.results, function (item) {
                    return {
                        id: item.Id + "" + item.Barcode,
                        text: item.ProductName,
                        ProductId: item.Id,
                        ProductName: item.ProductName,
                        Barcode: item.Barcode,
                        Quantity: 1,
                        TotalQuantity: item.InventoryNumber,
                        SellPrice: 0,
                        Discount: 0,
                        TotalPrice: 0,
                        ExpiredDate: item.ExpiredDate,
                        Type: false
                    };
                });
                if (datas.length == 1) {
                    var item = datas[0];
                    if (item != null) {
                        var datas = $("#tblDataProduct").bootstrapTable("getData");
                        if (datas.length > 0) {
                            var itemSearch = datas.filter(function (it) {
                                if (it.ProductId == item.ProductId)
                                    return it;
                            });
                            if (itemSearch.length > 0) {
                                if (itemSearch[0].Quantity < item.TotalQuantity) {
                                    itemSearch[0].Quantity = parseInt(itemSearch[0].Quantity) + 1;
                                    itemSearch[0].TotalPrice = (itemSearch[0].SellPrice * itemSearch[0].Quantity);
                                    $("#tblDataProduct").bootstrapTable("load", datas);
                                }
                                else {
                                    notifyWarning("Đã dùng hết số lượng!")
                                }
                            }
                            else {
                                $("#tblDataProduct").bootstrapTable("insertRow", { index: 0, row: item });
                            }
                        }
                        else {
                            $("#tblDataProduct").bootstrapTable("append", item);
                        }
                        OrderUpdate.component.SetAutoPrice();
                        $("#drdProduct").select2("close");
                        $("#drdProduct").val(null).trigger('change');
                        $("#drdProduct").select2("open");
                        return {
                            results: [],
                            pagination: {
                                more: params.page * 10 < data.total
                            }
                        };

                    }
                }
                else {
                    return {
                        results: datas,
                        pagination: {
                            more: params.page * 10 < data.total
                        }
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
            var $result = $("<span>" + data.ProductName + "</span>");
            $result.append(" <span style='color:white; background-color:#5fa6ff; padding:2px 5px;'>" + data.Barcode + "");
            if (data.ExpiredDate != null) {
                $result.append(" <span style='color:white; background-color:#f75350; padding:2px 5px;'>" + formatToDate(data.ExpiredDate) + "</span>");
            }
            return $result;
        }
    })
        .on('select2:select', function (evt) {
            var item = evt.params.data;
            if (item != null) {
                var datas = $("#tblDataProduct").bootstrapTable("getData");
                if (datas.length > 0) {
                    var itemSearch = datas.filter(function (it) {
                        if (it.ProductId == item.ProductId && it.Type == item.Type)
                            return it;
                    });
                    if (itemSearch.length > 0) {
                        if (itemSearch[0].Quantity < item.TotalQuantity) {
                            itemSearch[0].Quantity = parseInt(itemSearch[0].Quantity) + 1;
                            itemSearch[0].TotalPrice = (itemSearch[0].SellPrice * itemSearch[0].Quantity);
                            $("#tblDataProduct").bootstrapTable("load", datas);
                        }
                        else {
                            notifyWarning("Đã dùng hết số lượng!")
                        }
                    }
                    else {
                        $("#tblDataProduct").bootstrapTable("insertRow", { index: 0, row: item });
                    }
                }
                else {
                    $("#tblDataProduct").bootstrapTable("append", item);
                }
                $("#drdProduct").val(null).trigger('change.select2');
                $("#drdProduct").select2("close");
                $("#drdProduct").select2("open");
            }
        })
    $("#drdOrder").on('change', function () {
        hidedrdProduct();
    })
    $("#drdOrder").select2({
        placeholder: 'Nhập mã đơn hàng',
        multiple: false,
        minimumInputLength: 1,
        width: "100%",
        ajax: {
            delay: 500,
            type: 'POST',
            url: '/OrderPromotion/AutoCompletedPaidOrders',
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
                //lấy data OrderDetail
                _AjaxPost("/OrderPromotion/GetOrderDetailByOrderId", { OrderId: item.Id }, function (rs) {
                    if (rs.success) {
                        var configData = $.map(rs.datas, function (item) {
                            return {
                                id: item.ProductId + "",
                                text: item.ProductName,
                                ProductId: item.ProductId,
                                ProductName: item.ProductName,
                                Barcode: item.Barcode,
                                ProductCode: item.ProductCode,
                                Quantity: item.Quantity,
                                TotalQuantity: item.InventoryNumber,
                                SellPrice: item.SellPrice,
                                Discount: item.Discount,
                                TotalPrice: (item.SellPrice * item.Quantity - item.SellPrice * item.Quantity * item.Discount / 100),
                                ExpiredDate: item.ExpiredDate
                            };
                        });
                        var configDataSell = $.map(rs.datasell, function (item) {
                            return {
                                id: item.ProductId + "",
                                text: item.ProductName,
                                ProductId: item.ProductId,
                                ProductName: item.ProductName,
                                Barcode: item.Barcode,
                                ProductCode: item.ProductCode,
                                Quantity: item.Quantity,
                                TotalQuantity: item.InventoryNumber,
                                SellPrice: item.SellPrice,
                                Discount: item.Discount,
                                TotalPrice: (item.SellPrice * item.Quantity - item.SellPrice * item.Quantity * item.Discount / 100),
                                ExpiredDate: item.ExpiredDate
                            };
                        });
                        $("#tblDataProduct").bootstrapTable("load", configData);
                        $("#tblDataProductSell").bootstrapTable("load", configDataSell);
                        if (rs.isTang == null) { $(".product").show(); }

                    }
                    else {
                        notifyError("Lỗi Lấy dữ liệu chi tiết hóa đơn!");
                    }
                });

            }
        })

    if ($("#Id").val() != '') {
        $("#drdOrder").select2("trigger", "select", {
            data: {
                Id: $("#Id").val(),
                id: $("#Id").val(),
                text: $("#OrderCode").val(),
                OrderCode: $("#OrderCode").val()
            }
        });
    }
    var x = parseInt($('#vbaction').val());
    $("#tblDataProduct").bootstrapTable({
        //sidePagination: 'server',
        striped: true,
        //pagination: true,
        search: false,
        soft: false,
        showColumns: false,
        showRefresh: false,
        minimumCountColumns: 1,
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
                title: 'Số lượng',
                align: 'center',
                valign: 'middle',
                width: '200px',
                sortable: false,

                formatter: function (value, row, index) {
                    if (x != 3) {
                        return '<div class="input-group">' +
                            '<input type="text" ipPosInt class="form-control Quantity" value="' + value + '" data-v-Max=' + row.TotalQuantity + '>' +
                            '<span class="input-group-addon" style="min-width:60px">< ' + (row.TotalQuantity + 1) + '</span ></div>';
                    }
                    else return value;
                },
            },
            {
                title: 'Loại bỏ',
                field: 'remove',
                align: 'center',
                valign: 'middle',
                formatter: function (value, row, index) {
                    if (!row.Type)
                        return '<a class="remove" href="javascript:void(0)" style="color:red" title="Xóa"><i class="glyphicon glyphicon-remove"></i></a>';
                    else
                        return '';
                },
                events: {
                    'click .remove': function (e, value, row, index) {
                        if (row != null) {
                            $("#tblDataProduct").bootstrapTable("remove", { field: 'id', values: [row.id] });
                            if ($("[data-error]").length == 0) {
                                $("button[type='submit']").removeAttr("disabled");
                            }
                        }
                    }
                }
            }
        ],
        onPostBody: function (data) {
            app.component.FormatInput();
        },
        onLoadSuccess: function (data) {
        },
        onEditableShown: function (field, row, oldValue, $el) {
            //app.component.FormatInputNumber();
            switch (field) {
                case "SellPrice": new AutoNumeric('.ipMoney', row[field], opMoney); break;
                case "Discount": new AutoNumeric('.ipDiscount', row[field], opPercent); break;
            }
        },
        onEditableSave: function (field, row, oldValue, $el) {
            switch (field) {
                case "Quantity": row["Quantity"] = parseInt(getPosInt_AutoNumeric(row["Quantity"])); break;
                case "SellPrice": if (row["SellPrice"] == '') { row["SellPrice"] = 0 } else row["SellPrice"] = parseInt(getMoney_AutoNumeric(row["SellPrice"])); break;
                case "Discount": row["Discount"] = parseFloat(getPercent_AutoNumeric(row["Discount"])); break;
            }
            row["TotalPrice"] = row["Quantity"] * row["SellPrice"] - row["Quantity"] * row["SellPrice"] * row["Discount"] / 100;
            $("#tblDataProduct").bootstrapTable("updateRow", row);

        }
    });
   
    if (x == 3) {
        $("#tblDataProduct").bootstrapTable("hideColumn", 'remove');
    }
    $("#tblDataProduct").on("keyup", '.Quantity[ipPosInt]', function () {
        if ($(this).val() == 0 || $(this).val() == '') {
            $(this).attr("style", "border-color:#d43f3a");
            $(this).attr("data-error", true);
            $("button[type='submit']").attr("disabled", "disabled");
        }
        else {
            $(this).attr("style", "border-color:#ccc");
            $(this).removeAttr("data-error");
            $("button[type='submit']").removeAttr("disabled");
        }
        var index = $(this).closest('tr').data("index");
        var data = $("#tblDataProduct").bootstrapTable("getData");
        data[index].Quantity = getPosInt_AutoNumeric($(this).val());
        data[index].TotalPrice = data[index].Quantity * data[index].SellPrice - data[index].Quantity * data[index].SellPrice * data[index].Discount / 100;
        $(this).closest('tr').find(".totalprice").text(formatMoney(data[index].TotalPrice));

    });
    $("#btnSave").click(function () {
        var data = $("#tblDataProduct").bootstrapTable("getData");
        var OrderIds = $("#drdOrder").val();
        if (data.length > 0) {
            var listDetail = $.map(data, function (item) {
                if (item.Quantity > 0) {
                    return item
                }
            })
            if (listDetail.length > 0) {
                _AjaxPost("/OrderPromotion/UpdatePromotionOrder", { Id: OrderIds, listDetail, isTang: false }, function (rs) {
                    if (rs.success) {
                        notifySuccess("Tạo khuyến mại thành công!");
                        setTimeout(function () {
                            window.location.href = "/OrderPromotion";
                        }, 1000);
                    }
                    else notifyError("Tạo khuyến mại thất bại! " + rs.message);
                });
            }
            else {
                notifyError("Vui lòng chọn số lượng sản phẩm khuyến mại!");
            }
        }
        else {
            notifyError("Vui lòng chọn sản phẩm khuyến mại!");
        }
    });
    $("#btnOk").click(function () {
        var data = $("#tblDataProduct").bootstrapTable("getData");
        var OrderIds = $("#drdOrder").val();
        if (data.length > 0) {
            var listDetail = $.map(data, function (item) {
                if (item.Quantity > 0) {
                    return item
                }
            })
            if (listDetail.length > 0) {
                _AjaxPost("/OrderPromotion/UpdatePromotionOrder", { Id: OrderIds, listDetail, isTang: true }, function (rs) {
                    if (rs.success) {
                        notifySuccess("Khuyến mại thành công!");
                        setTimeout(function () {
                            window.location.href = "/OrderPromotion";
                        }, 1000);
                    }
                    else notifyError("Khuyến mại thất bại!");
                });
            }
            else {
                notifyError("Chưa có số lượng cho sản phẩm khuyến mại!");
            }
        }
        else {
            notifyError("Chưa chọn sản phẩm khuyến mại!");
        }
    });
    $("#btnCancel").click(function () {
        var data = $("#tblDataProduct").bootstrapTable("getData");
        var OrderIds = $("#drdOrder").val();
        if (data.length > 0) {
            var listDetail = $.map(data, function (item) {
                if (item.Quantity > 0) {
                    return item
                }
            })
            if (listDetail.length > 0) {
                modal.DeleteComfirm({
                    callback: function () {
                        _AjaxPost("/OrderPromotion/Delete", { Id: OrderIds }, function (rs) {
                            if (rs.success) {

                                notifySuccess("Hủy khuyến mại thành công!")
                                setTimeout(function () {
                                    window.location.href = "/OrderPromotion";
                                }, 1000);
                            }
                            else notifyError("Hủy khuyến mại thất bại!");
                        });
                    }
                });

            }
            else {
                notifyError("Vui lòng chọn sản phẩm khuyến mại!");
            }
        }
        else {
            notifyError("Chưa chọn hóa đơn!");
        }
    });
    $("#tblDataProductSell").bootstrapTable({
        //sidePagination: 'server',
        striped: true,
        //pagination: true,
        search: false,
        soft: false,
        showColumns: false,
        showRefresh: false,
        minimumCountColumns: 1,
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
                title: 'Số lượng',
                align: 'center',
                valign: 'middle',
                sortable: false,


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
                title: 'Chiết Khấu (%)',
                align: 'right',
                valign: 'middle',
                sortable: false,

            },
            {
                field: 'TotalPrice',
                title: 'Thành Tiền (VND)',
                align: 'right',
                valign: 'middle',
                sortable: false,
                formatter: function (value, row, index) {
                    return '<div class="totalprice">' + formatMoney(value) + '</div>';
                }
            }

        ],

    });

});