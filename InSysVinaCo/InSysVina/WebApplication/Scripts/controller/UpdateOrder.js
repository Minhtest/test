(function (window, $) {
    window.CreateOrder = {
        ui: {
            selectProduct: $("#selectProduct"),
            tblDataProduct: $("#tblDataProduct")
        },
        init: function () {
            CreateOrder.CreateSelectProduct();
            CreateOrder.CreateTableProductOrder();
        },
        CreateSelectProduct: function () {
            CreateOrder.ui.selectProduct.select2({
                placeholder: 'Tìm bằng barcode, tên sản phẩm và code sản phẩm...',
                minimumInputLength: 1,
                maximumSelectionLength: 2,
                width: '100%',
                ajax: {
                    delay: 250,
                    type: 'POST',
                    url: '/Orders/AutoCompletedProduct',
                    data: function (params) {
                        return {
                            warehouseId: 0,
                            filter: params.term,
                            page: params.page
                        };
                    },
                    dataType: 'json',
                    results: function (data, page) {
                        return { results: data.results };
                    },
                    processResults: function (data, params) {
                        params.page = params.page || 1;
                        return {
                            results: $.map(data.results, function (item) {
                                return {
                                    'id': item.Id,
                                    'text': item.ProductName,
                                    'ProductId': item.Id,
                                    'Name': item.ProductName,
                                    'ProductCode': item.ProductCode,
                                    'Barcode': item.Barcode,
                                    'Quantity': 1,
                                    'Price': item.SellPrice,
                                    'SellPrice': item.SellPrice,
                                    'Discount': 0,
                                    'TotalPrice': item.SellPrice
                                }
                            }),
                            pagination: {
                                more: (params.page * 10) < data.total
                            }
                        };
                    }
                },
                templateResult: function (data) {
                    if (data.loading)
                        return data.text;
                    var $result = $("<span>" + data.Name + "</span>");

                    $result.append(" <span class='badge'>" + data.ProductCode + "</span>");
                    $result.append(" <span class='label label-primary'>" + data.Barcode + "</label>");
                    return $result;
                }
            }).on('select2:select', function (evt) {
                var item = evt.params.data;
                if (item != null) {
                    $('#selectProduct').val(null).trigger("change");
                    var datas = CreateOrder.ui.tblDataProduct.bootstrapTable("getData");

                    var search = datas.filter(function (e) { return (e.id || e.id) == evt.params.data.id })[0];

                    if (search == null) {
                        CreateOrder.ui.tblDataProduct.bootstrapTable("append", item);
                    }
                    else {
                        datas.filter(function (e) { return (e.id || e.id) == evt.params.data.id })[0].Quantity += 1;
                        datas.filter(function (e) { return (e.id || e.id) == evt.params.data.id })[0].TotalPrice = search.SellPrice * (search.Quantity);
                        CreateOrder.ui.tblDataProduct.bootstrapTable("load", datas);
                    }
                    CreateOrder.SetTotalPrice();
                }
            });
        },
        CreateTableProductOrder: function () {
            CreateOrder.ui.tblDataProduct.bootstrapTable({
                sidePagination: 'server',
                ajax: function (config) {
                    $.ajax({
                        url: "/Orders/GetOrderDetailByOrderId",
                        type: "POST",
                        data: {
                            OrderId: $("#OrderId").val()
                        },
                        success: function (rs) {
                            if (rs.success) {
                                var configData = $.map(rs.datas, function (item) {
                                    return {
                                        'id': item.ProductId,
                                        'text': item.Name,
                                        'ProductId': item.ProductId,
                                        'Name': item.Name,
                                        'ProductCode': item.ProductCode,
                                        'Barcode': item.Barcode,
                                        'Quantity': item.Quantity,
                                        'Price': item.SellPrice,
                                        'SellPrice': item.SellPrice,
                                        'Discount': item.Discount,
                                        'TotalPrice': (item.SellPrice * item.Quantity) - (item.SellPrice * item.Quantity * item.Discount / 100)
                                    }
                                });
                                config.success({
                                    rows: configData,
                                    total: rs.total
                                });
                            }
                            else {
                                notifyError("Lỗi Lấy dữ liệu!")
                            }
                        },
                        error: function () {
                            notifyError("Lỗi truyền dữ liệu!")
                        }
                    });
                },
                sidePagination: 'server',
                striped: true,
                pagination: true,
                search: false,
                soft: false,
                showColumns: false,
                showRefresh: false,
                minimumCountColumns: 1,
                columns: [
                    {
                        field: 'id',
                        title: 'ID',
                        align: 'left',
                        valign: 'top',
                        sortable: false
                    },
                    {
                        field: 'Name',
                        title: 'Tên sản phẩm',
                        align: 'left',
                        valign: 'top',
                        sortable: false
                    },
                    {
                        field: 'Barcode',
                        title: 'Barcode',
                        align: 'left',
                        valign: 'top',
                        sortable: false
                    },
                    {
                        field: 'Quantity',
                        title: 'Số lượng',
                        align: 'right',
                        valign: 'top',
                        sortable: false,
                        editable: {
                            mode: 'popup',
                            inputclass: 'money',
                            showbuttons: true,
                            type: 'text'
                        },
                        formatter: function (value, row, index) {
                            return formatFloat(value);
                        },
                        footerFormatter: function (datas) {
                            var total = 0;
                            datas.map(function (e) { total += e.quantity });
                            return (total || 0);
                        },
                    },
                    {
                        field: 'SellPrice',
                        title: 'Giá bán (VND)',
                        align: 'right',
                        valign: 'top',
                        sortable: false,
                        //formatter: function (value, row, index) {
                        //    return '<span class="money">' + value + '</span>';
                        //},
                        editable: {
                            mode: 'popup',
                            inputclass: 'money',
                            showbuttons: true,
                            type: 'text'
                        }
                    },
                    {
                        field: 'Discount',
                        title: 'Chiết Khấu (%)',
                        align: 'right',
                        valign: 'top',
                        sortable: false,
                        //formatter: function (value, row, index) {
                        //    return value;
                        //},
                        editable: {
                            mode: 'popup',
                            //inputclass: 'input-width-grid',
                            inputclass: "money",
                            showbuttons: true,
                            type: 'text',
                        }
                    },
                    {
                        field: 'TotalPrice',
                        title: 'Thành Tiền (VND)',
                        align: 'right',
                        valign: 'top',
                        sortable: false,
                        //class:"money",
                        //formatter: function (value, row, index) {
                        //    return formatMoney(value)
                        //},
                        //footerFormatter: function (datas) {
                        //    var total = 0;
                        //    datas.map(function (e) { total += (e.quantity * e.sellPrice) });

                        //    $("#lblProductTotal").text((total || 0).toVND())
                        //    $("#lblGrandTotal").text((total || 0).toVND())
                        //    $("#GrandTotal").val(total);
                        //    $("#ProductTotal").val(total);
                        //    return (total || 0).toVND();
                        //}
                    },
                ],
                onLoadSuccess: function (data) {
                    CreateOrder.SetTotalPrice();
                },
                onEditableShown: function () {
                    //const optionAutonumeric = {
                    //    allowDecimalPadding: false,
                    //    decimalPlaces: 2,
                    //    decimalCharacter: '.',
                    //    digitGroupSeparator: ','

                    //}
                    //new AutoNumeric('.money', optionAutonumeric);
                },
                onEditableSave: function (field, row, oldValue, $el) {
                    
                    row["TotalPrice"] = (row["Quantity"] * row["SellPrice"]) - (row["Quantity"] * row["SellPrice"] * row["Discount"] / 100);
                    CreateOrder.ui.tblDataProduct.bootstrapTable("updateRow", row);
                    CreateOrder.SetTotalPrice();
                }
            });
        },
        addProduct: function () {
            var dataProductGet = CreateOrder.ui.tblDataProduct.bootstrapTable("getData");
        },
        SetTotalPrice: function () {
            var datatblProduct = CreateOrder.ui.tblDataProduct.bootstrapTable("getData");
            var TotalPrice = 0;
            datatblProduct.forEach(function (item) {
                TotalPrice += parseFloat(item.TotalPrice);
            });
            $("#ProductTotal").val(TotalPrice);
            var CouponCode = $("#CouponCode").val();
            if (CouponCode != null) {
                //alert("check");
            }
            //check mã giảm giá
            // set tổng tiền
        }
    };
})(window, $);
$(document).ready(function () {
    CreateOrder.init();
    //new AutoNumeric('#PaidGuests', AutoNumeric.getPredefinedOptions().float);
    //new AutoNumeric('#RefundMoney', AutoNumeric.getPredefinedOptions().float);
    $("#frmUpdateOrder").on("submit", function (form, data) {

        var orderUpdate = {
            OrderEntity: {
                OrderId: $("#OrderId").val(),
                CustomerId: $("#CustomerId").val(),
                CustomerCode: $("#CustomerCode").val(),
                ProductTotal: $("#ProductTotal").val(),
                CouponCode: $("#CouponCode").val(),
                GrandTotal: $("#GrandTotal").val(),
                PaidGuests: $("#PaidGuests").val(),
                RefundMoney: $("#RefundMoney").val()
            },
            OrderDetailEntities: CreateOrder.ui.tblDataProduct.bootstrapTable("getData")
        }
        $.ajax({
            url: "/Orders/SubmitOrder",
            type: "POST",
            data: {
                order: orderUpdate
            },
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (rs) {
                console.log(rs);
                if (rs.success) {
                    notifySuccess("Order Thành Công!");
                }
                else {
                    notifyError("Xảy ra lỗi trong quá trình xử lý");
                }
            },
            error: function () {
                notifyError('Gặp lỗi truyền dữ liệu!');
            }
        });
        return false;
    });

    $('#dateBirthDay').datepicker({
        format: "dd/mm/yyyy",
        todayBtn: true,
        clearBtn: true,
        language: "vi",
        calendarWeeks: true,
        autoclose: true,
        todayHighlight: true
    });
    $("#frmAddCustomer").on("submit", function () {
        var data = $(this).serialize();
        $.ajax({
            url: "/Customer/CreateOrEdit",
            type: "POST",
            data: data,
            success: function (rs) {
                if (rs.success) {
                    notifySuccess("Thêm khách hàng thành công!");
                    console.log(rs);
                    $("#Name").val(rs.Name);
                    $("#CustomerId").val(rs.CustomerId);
                    $("#CustomerCode").val(rs.CustomerCode);
                    $("#createCustomerModal").modal("hide");
                }
                else {
                    notifyError("Lỗi thêm mới khách hàng!");
                }
            },
            error: function () {
                notifyError("Lỗi truy cập!");
            }
        });
        return false;
    });
    $("#selectCustomer").select2({
        placeholder: 'Tìm Khách Hàng Theo Tên, Code, SDT...',
        minimumInputLength: 1,
        maximumSelectionLength: 2,
        width: '100%',
        ajax: {
            delay: 250,
            type: 'POST',
            url: '/Customer/AutoCompletedCustomer',
            data: function (params) {
                return {
                    filter: params.term
                };
            },
            dataType: 'json',
            results: function (data, page) {
                return { results: data.results };
            },
            processResults: function (data, params) {
                params.page = params.page || 1;
                return {
                    results: $.map(data.results, function (item) {
                        
                        return {
                            'id': item.CustomerId,
                            'text': item.Name,
                            'CustomerId': item.CustomerId,
                            'Name': item.Name,
                            'CustomerCode': item.CustomerCode,
                            'Phone': item.Phone
                        }
                    }),
                    pagination: {
                        more: (params.page * 10) < data.total
                    }
                };
            }
        },
        templateResult: function (data) {
            if (data.loading)
                return data.text;
            var $result = $("<span>" + data.Name + "</span>");

            $result.append(" <span class='badge'>" + data.Phone + "</span>");
            $result.append(" <span class='label label-primary'>" + data.CustomerCode + "</label>");
            return $result;
        }
    }).on('select2:select', function (evt) {
        var item = evt.params.data;
        if (item != null) {
            $("#CustomerId").val(item.CustomerId);
            $("#CustomerCode").val(item.CustomerCode);
            $("#Name").val(item.Name);
        }
    });
});
var formatMoney = function (text) {
    return text.toLocaleString();
}
var formatFloat = function (text) {
    return text.toLocaleString();
}