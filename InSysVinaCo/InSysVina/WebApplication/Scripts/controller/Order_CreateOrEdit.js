var OrderUpdate = {
    load: function () {
        OrderUpdate.component.selectCustomer();
        OrderUpdate.component.selectProduct();
        OrderUpdate.component.selectProductPromotion();
        OrderUpdate.component.tblProductOrder();
        OrderUpdate.component.tblProductPromotion();
        app.component.DatePicker();
        if ($("#Id").val() != null && $("#Id").val() != '') {
            //update
            if ($("#CustomerId").val() != null && $("#CustomerId").val() != '') {
                _AjaxPost("/Customer/GetCustomerByID", {
                    CustomerId: $("#CustomerId").val()
                }, function (rs) {
                    var data = rs.customer;
                    var item = {
                        id: data.Id + "",
                        text: data.Name,
                        CustomerId: data.Id,
                        Name: data.Name,
                        CustomerCode: data.CustomerCode,
                        Phone: data.Phone,
                        Point: data.Point
                    }
                    $("#drdCustomer").select2("trigger", "select", {
                        data: item
                    });
                });
            }
            else {
                OrderUpdate.component.getInfoCustomer();
            }
            _AjaxPost("/Orders/GetOrderDetailByOrderId", { OrderId: $("#Id").val() }, function (rs) {
                if (rs.success) {
                    var configData = $.map(rs.datas, function (item) {
                        return {
                            'id': item.ProductId,
                            'text': item.ProductName,
                            'ProductId': item.ProductId,
                            'ProductName': item.ProductName,
                            'Barcode': item.Barcode,
                            'Quantity': item.Quantity,
                            'InventoryNumber': item.InventoryNumber,
                            'Price': item.SellPrice,
                            'SellPrice': item.SellPrice,
                            'Discount': item.Discount,
                            'TotalPrice': item.SellPrice * item.Quantity - item.SellPrice * item.Quantity * item.Discount / 100,
                        };
                    });
                    $("#tblDataProduct").bootstrapTable("load", configData);
                    OrderUpdate.component.checkShowPromotion();
                    OrderUpdate.component.SetAutoPrice();
                }
                else {
                    notifyError("Lỗi Lấy dữ liệu chi tiết hóa đơn!");
                }
            });

            _AjaxPost("/Orders/GetProductPromotionByOrderId", { OrderId: $("#Id").val() }, function (rs) {
                if (rs.success) {
                    var data = rs.datas;
                    if (data.length > 0) {
                        $('.promotion').show();
                        var configData = $.map(data, function (item) {
                            return {
                                'id': item.ProductId,
                                'text': item.ProductName,
                                'ProductId': item.ProductId,
                                'ProductName': item.ProductName,
                                'Barcode': item.Barcode,
                                'Quantity': item.Quantity,
                                'InventoryNumber': item.InventoryNumber
                            }
                        });
                        $("#tblProductPromotion").bootstrapTable("load", configData);
                    }
                }
                else {
                    notifyError("Lấy dữ liệu khuyến mại thất bại!");
                }
            });

            if ($("#CouponCode").val() != '') {
                OrderUpdate.component.checkCoupon();
            }
            else {
                if ($("#Voucher").val() != '') {
                    $("input[name='opDis'][value='2']").trigger('click');
                }
            }
        }
        else {
            //tạo mới
            OrderUpdate.component.getInfoCustomer();
            $("#drdProduct").select2("open");
        }
        OrderUpdate.component.event();
        OrderUpdate.component.Select2Warehouse();

    },
    component: {
        selectCustomer: function () {
            $("#drdCustomer").select2({
                placeholder: 'Nhâp tên, Mã thẻ, Mã khách hàng, Số điện thoại, Email.',
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
                                    //Point: item.Point,
                                    //CardNumber: item.CardNumberId
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
                        $("#CustomerId").val(item.CustomerId);
                        OrderUpdate.component.getInfoCustomer();
                        OrderUpdate.component.SetAutoPrice();
                    }
                })
                .on('change', function (evt) {
                    if ($(this).val() == null) {
                        $("#CustomerId").val("");
                        $("#CouponCode").val("")
                        OrderUpdate.component.getInfoCustomer();
                        OrderUpdate.component.SetAutoPrice();
                    }
                })
        },
        selectProduct: function () {
            $("#drdProduct").select2({
                placeholder: 'Nhập barcode, Mã sản phẩm, Tên sản phẩm.',
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
                        console.log(rs);
                        if (rs.results.length == 1) {
                            setTimeout(function () {
                                $("#drdProduct").select2("close");
                                $("#drdProduct").select2("open");
                            }, 100);
                        }
                    },
                    processResults: function (data, params) {
                        app.component.Loading.Hide();
                        params.page = params.page || 1;
                        var datas = $.map(data.results, function (item) {
                            if (item.InventoryNumber > 0) {
                                return {
                                    id: item.Id + "",
                                    text: item.ProductName,
                                    ProductId: item.Id,
                                    ProductName: item.ProductName,
                                    Barcode: item.Barcode,
                                    Quantity: 1,
                                    InventoryNumber: item.InventoryNumber,
                                    SellPrice: item.SellPrice,
                                    Discount: 0,
                                    TotalPrice: item.SellPrice,
                                    ExpiredDate: item.ExpiredDate
                                };
                            }
                        });
                        if (datas.length == 1) {
                            $("#drdProduct").select2("close");
                            $("#drdProduct").val(null).trigger('change')
                            $("#drdProduct").select2("open");

                            var item = datas[0];
                            if (item != null) {
                                var datatbl = $("#tblDataProduct").bootstrapTable("getData");
                                if (datatbl.length > 0) {
                                    var itemSearch = datatbl.filter(function (it) {
                                        if (it.ProductId == item.ProductId)
                                            return it;
                                    });
                                    if (itemSearch.length > 0) {
                                        if (itemSearch[0].Quantity < itemSearch[0].InventoryNumber) {
                                            itemSearch[0].Quantity = parseInt(itemSearch[0].Quantity) + 1;
                                            itemSearch[0].TotalPrice = (itemSearch[0].SellPrice * itemSearch[0].Quantity);
                                            $("#tblDataProduct").bootstrapTable("load", datatbl);
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
                                OrderUpdate.component.checkShowPromotion();
                                OrderUpdate.component.SetAutoPrice();
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
                    $result.append(" <span style='color:white; background-color:#5fa6ff; padding:2px 5px;'>" + data.Barcode + "</span>");
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
                        if (datas.length == 0) {
                            $("#tblDataProduct").bootstrapTable("append", item);
                        }
                        else {
                            var itemSearch = datas.filter(function (it) {
                                if (it.ProductId == item.ProductId) {
                                    return it;
                                }
                            });
                            if (itemSearch.length > 0) {
                                if (itemSearch[0].Quantity < itemSearch[0].InventoryNumber) {
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
                        OrderUpdate.component.checkShowPromotion();
                        OrderUpdate.component.SetAutoPrice();
                        $("#drdProduct").select2("close");
                        $("#drdProduct").val(null).trigger('change')
                        $("#drdProduct").select2("open");
                    }
                })
        },
        selectProductPromotion: function () {
            $("#drdProductPromotion").select2({
                placeholder: 'Nhập barcode, Mã sản phẩm, Tên sản phẩm.',
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
                                $("#drdProductPromotion").select2("close");
                                $("#drdProductPromotion").select2("open");
                            }, 10);
                        }
                    },
                    processResults: function (data, params) {
                        app.component.Loading.Hide();
                        params.page = params.page || 1;
                        var datas = $.map(data.results, function (item) {
                            if (item.InventoryNumber > 0) {
                                return {
                                    id: item.Id + "",
                                    text: item.ProductName,
                                    ProductId: item.Id,
                                    ProductName: item.ProductName,
                                    Barcode: item.Barcode,
                                    Quantity: 1,
                                    InventoryNumber: item.InventoryNumber,
                                    ExpiredDate: item.ExpiredDate
                                };
                            }
                        });
                        if (datas.length == 1) {
                            $("#drdProductPromotion").select2("close");
                            $("#drdProductPromotion").val(null).trigger('change')
                            $("#drdProductPromotion").select2("open");

                            var item = datas[0];
                            if (item != null) {
                                var datatbl = $("#tblProductPromotion").bootstrapTable("getData");
                                if (datatbl.length > 0) {
                                    var itemSearch = datatbl.filter(function (it) {
                                        if (it.ProductId == item.ProductId)
                                            return it;
                                    });
                                    if (itemSearch.length > 0) {
                                        if (itemSearch[0].Quantity < itemSearch[0].InventoryNumber) {
                                            itemSearch[0].Quantity = parseInt(itemSearch[0].Quantity) + 1;
                                            itemSearch[0].TotalPrice = (itemSearch[0].SellPrice * itemSearch[0].Quantity);
                                            $("#tblProductPromotion").bootstrapTable("load", datatbl);
                                        }
                                        else {
                                            notifyWarning("Đã dùng hết số lượng!")
                                        }
                                    }
                                    else {
                                        $("#tblProductPromotion").bootstrapTable("insertRow", { index: 0, row: item });
                                    }
                                }
                                else {
                                    $("#tblProductPromotion").bootstrapTable("append", item);
                                }
                                OrderUpdate.component.SetAutoPrice();
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
                    $result.append(" <span style='color:white; background-color:#5fa6ff; padding:2px 5px;'>" + data.Barcode + "</span>");
                    if (data.ExpiredDate != null) {
                        $result.append(" <span style='color:white; background-color:#f75350; padding:2px 5px;'>" + formatToDate(data.ExpiredDate) + "</span>");
                    }
                    return $result;
                }
            })
                .on('select2:select', function (evt) {
                    var item = evt.params.data;
                    if (item != null) {
                        var datas = $("#tblProductPromotion").bootstrapTable("getData");
                        if (datas.length == 0) {
                            $("#tblProductPromotion").bootstrapTable("append", item);
                        }
                        else {
                            var itemSearch = datas.filter(function (it) {
                                if (it.ProductId == item.ProductId) {
                                    return it;
                                }
                            });
                            if (itemSearch.length > 0) {
                                if (itemSearch[0].Quantity < itemSearch[0].InventoryNumber) {
                                    itemSearch[0].Quantity = parseInt(itemSearch[0].Quantity) + 1;
                                    itemSearch[0].TotalPrice = (itemSearch[0].SellPrice * itemSearch[0].Quantity);
                                    $("#tblProductPromotion").bootstrapTable("load", datas);
                                }
                                else {
                                    notifyWarning("Đã dùng hết số lượng!")
                                }
                            }
                            else {
                                $("#tblProductPromotion").bootstrapTable("insertRow", { index: 0, row: item });

                            }
                        }
                        OrderUpdate.component.SetAutoPrice();
                        $("#drdProductPromotion").select2("close");
                        $("#drdProductPromotion").val(null).trigger('change')
                        $("#drdProductPromotion").select2("open");
                    }
                })
        },
        getInfoCustomer: function () {
            _AjaxPost("/Customer/GetCustomerByID", {
                CustomerId: $("#CustomerId").val() != '' ? $("#CustomerId").val() : 0
            }, function (rs) {
                _AjaxPost("/Customer/InfoCustomer", { customer: rs.customer }, function (rsview) {
                    $("#tblCustomerInfo").html(rsview);

                    if ($("#CustomerId").val() != '' && $('#ipCardNumber').val() != '') {
                        app.component.FormatText();
                        //if (rs.customer.CardNumber != null && parseInt($('#maxpoint').val()) > 0) {
                        //    //if ($("#CouponCode").val() == '') {
                        //    //    $("#CouponCode").val(rs.customer.CardNumber);
                        //    //}
                        //    //OrderUpdate.component.checkCoupon();
                        //}
                        if (parseInt($('#maxpoint').val()) > 0) {
                            $("#ipMoneyAllPoint").val(rs.customer.MoneyAllPoint);
                            $("#customerPoint").attr("style", "display:table-row");
                        }
                        $("#ShowPoint").attr("style", "margin-left:5px; ");//display:inline-block
                    }
                    else {
                        $("#customerPoint").attr("style", "display:none");
                        $("#ShowPoint").attr("style", "display:none");
                    }
                    OrderUpdate.component.SetAutoPrice();
                });
            });
        },
        checkCoupon: function () {
            $("input[name='opDis'][value='1']").trigger('click');
            $("#CouponInfo").html('');
            $("#CouponType").val('');
            $("#CouponValue").val('');
            $("i#txtReducedCoupon").text("Giảm ?");

            if ($("#CouponCode").val() != '') {
                $("i#txtReducedCoupon").html('<img src="/Content/img/loading.gif" height="20px" />');
                var dataTblProduct = $("#tblDataProduct").bootstrapTable("getData");
                var totalPrice = 0;

                if (dataTblProduct.length > 0) {
                    dataTblProduct.forEach(function (item) {
                        totalPrice += (item.Quantity * item.SellPrice - item.Quantity * item.SellPrice * item.Discount / 100);
                    });
                }
                _AjaxPost("/Orders/CheckCoupon", {
                    couponCode: $("#CouponCode").val(),
                    TotalPrice: totalPrice
                }, function (rs) {
                    $("i#txtReducedCoupon").text("Giảm ?");
                    //SHOW THÔNG TIN MÃ GIẢM GIÁ

                    if (rs.checkValue == -1) {
                        notifyError("KHÔNG kiểm tra được mã giảm giá này!");
                    }
                    else if (rs.checkValue == 0) {
                        notifyError("Mã giảm giá KHÔNG TỒN TẠI!");
                    }
                    else if (rs.checkValue == 1) {
                        notifyWarning("Mã giảm giá HẾT HẠN!");
                    }
                    else {

                        if (rs.checkValue == 2) {
                            notifyWarning("Mã giảm giá CHƯA ÁP DỤNG!");
                        }
                        else if (rs.checkValue == 3) {
                            notifyWarning("CHƯA ĐỦ ĐIỀU KIỆN ÁP DỤNG Mã giảm giá này!");
                        }
                        else {
                            //=4
                            $("#CouponType").val(rs.discount.type);
                            $("#CouponValue").val(parseFloat(rs.discount.DiscountValue));
                            $("#MaxUseCoupon").val(parseFloat(rs.discountObj.MaxValue));
                            if (rs.discountObj.type == 1) {
                                $("i#txtReducedCoupon").text("Giảm: " + (formatMoney(rs.discount.DiscountValue) + " VND"));
                                $("#Discount").val(formatMoney($("#CouponValue").val()));
                            }
                            else {
                                $("i#txtReducedCoupon").text("Giảm: " + rs.discount.DiscountValue + " %");
                                var dataOrderDetail = $("#tblDataProduct").bootstrapTable("getData");
                                var TotalPrice = 0;
                                dataOrderDetail.forEach(function (item) {
                                    TotalPrice += parseFloat(item.TotalPrice);
                                });
                                var totalDiscount = TotalPrice * parseInt($("#CouponValue").val()) / 100;
                                if ($("#MaxUseCoupon").val() != "" && totalDiscount > $("#MaxUseCoupon").val()) {
                                    totalDiscount = $("#MaxUseCoupon").val();
                                }
                                $("#Discount").val(formatMoney(totalDiscount));
                            }
                            notifySuccess("Áp dụng mã giảm giá thành Công!");
                        }
                        var discountObj = rs.discountObj;
                        if (discountObj != null) {
                            discountObj.StartDate = formatToDate(discountObj.StartDate);
                            discountObj.EndDate = formatToDate(discountObj.EndDate);
                            _AjaxPost("/Orders/InfoCoupon", { discountObj: rs.discountObj }, function (rsview) {
                                $("#CouponInfo").html(rsview);
                            })
                        }
                    }
                    OrderUpdate.component.SetAutoPrice();
                })
            }
            else {
                OrderUpdate.component.SetAutoPrice();
            }
        },
        tblProductOrder: function () {
            $("#tblDataProduct").bootstrapTable({
                striped: true,
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
                        formatter: function (value, row, index) {
                            if (row.InventoryNumber <= 0) {
                                return '<b style="color:red">Đã Hết Hàng</b>'
                            }
                            else {
                                return '<div class="input-group">' +
                                    '<input type="text" ipPosInt class="form-control Quantity" value="' + value + '" data-v-Max=' + row.InventoryNumber + '>' +
                                    '<span class="input-group-addon" style="min-width:60px">< ' + (row.InventoryNumber + 1) + '</span ></div>';
                            }
                        },
                    },
                    {
                        field: 'SellPrice',
                        title: 'Giá bán (VND)',
                        align: 'right',
                        valign: 'middle',
                        sortable: false,
                        formatter: function (value, row, index) {
                            return '<div class="sellprice">' + formatMoney(value) + '</div>';
                        }
                    },
                    {
                        field: 'Discount',
                        title: 'Chiết Khấu (%)',
                        align: 'right',
                        valign: 'middle',
                        sortable: false,
                        formatter: function (value, row, index) {
                            return formatPercent(value);
                        },
                        editable: {
                            mode: 'popup',
                            inputclass: 'ipDiscount',
                            showbuttons: true,
                            type: 'text'
                        }
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
                    },
                    {
                        title: 'Loại bỏ',
                        align: 'center',
                        valign: 'middle',
                        formatter: function (value, row, index) {
                            return '<a class="remove" href="javascript:void(0)" style="color:red" title="Xóa"><i class="glyphicon glyphicon-remove"></i></a>';
                        },
                        events: {
                            'click .remove': function (e, value, row, index) {
                                if (row != null) {
                                    $("#tblDataProduct").bootstrapTable("remove", { field: 'id', values: [row.id] });
                                    OrderUpdate.component.SetAutoPrice();
                                    OrderUpdate.component.checkShowPromotion();
                                }
                            }
                        }
                    }
                ],
                onPostBody: function (data) {
                    app.component.FormatInput();
                },
                onLoadSuccess: function (data) {
                    OrderUpdate.component.SetAutoPrice();
                },
                onEditableShown: function (field, row, oldValue, $el) {
                    switch (field) {
                        case "Discount": new AutoNumeric('.ipDiscount', row[field], opPercent); break;
                    }
                },
                onEditableSave: function (field, row, oldValue, $el) {
                    switch (field) {
                        case "Discount":
                            if (row.Discount == '') {
                                row.Discount = 0;
                                break;
                            }
                            else {
                                row.Discount = parseFloat(getPercent_AutoNumeric(row.Discount));
                                break;
                            }
                    }
                    row.TotalPrice = row.Quantity * row.SellPrice - row.Quantity * row.SellPrice * row.Discount / 100;
                    $("#tblDataProduct").bootstrapTable("updateRow", row);
                    OrderUpdate.component.checkShowPromotion();
                    OrderUpdate.component.SetAutoPrice();
                }
            });
        },
        tblProductPromotion: function () {
            $("#tblProductPromotion").bootstrapTable({
                striped: true,
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
                            if (row.InventoryNumber <= 0) {
                                return '<b style="color:red">Đã Hết Hàng</b>'
                            }
                            else {
                                return '<div class="input-group">' +
                                    '<input type="text" ipPosInt class="form-control Quantity" value="' + value + '" data-v-Max=' + row.InventoryNumber + '>';
                                    //+ '<span class="input-group-addon" style="min-width:60px">< ' + (row.InventoryNumber + 1) + '</span ></div>';
                            }
                        },
                    },
                    {
                        title: 'Loại bỏ',
                        align: 'center',
                        valign: 'middle',
                        formatter: function (value, row, index) {
                            return '<a class="remove" href="javascript:void(0)" style="color:red" title="Xóa"><i class="glyphicon glyphicon-remove"></i></a>';
                        },
                        events: {
                            'click .remove': function (e, value, row, index) {
                                if (row != null) {
                                    $("#tblProductPromotion").bootstrapTable("remove", { field: 'id', values: [row.id] });
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
                }
            });
        },
        checkShowPromotion: function () {
            var dataOrderDetail = $("#tblDataProduct").bootstrapTable("getData");
            var QuotaPromotion = parseFloat($('#QuotaPromotion').val());
            var TotalPrice = 0;
            dataOrderDetail.forEach(function (item) {
                TotalPrice += item.TotalPrice;
            });
            console.log(TotalPrice);
            console.log(QuotaPromotion);
            console.log(TotalPrice >= QuotaPromotion);
            if (TotalPrice >= QuotaPromotion) {
                $('.promotion').show();
                return true;
            }
            else {
                $('#tblProductPromotion').bootstrapTable('removeAll');
                $('.promotion').hide();
                return false;
            }
        },
        event: function () {
            $("#btnAddCustomer").on("click", function () {
                app.component.Loading.Show();
                cusmodal.ShowView("/Customer/Create", function () {
                    app.component.DatePicker();
                    OrderUpdate.component.drdCardNumber();
                    app.component.ValidateInputPhone();
                    app.component.ValidateEmail();
                    app.component.Loading.Hide();
                    OrderUpdate.component.selectCardNumber();
                    $("#frmAddCustomer").on("submit", function (event) {
                        event.preventDefault();
                        var data = $(this).serializeObject();
                        if (data.Name.trim() == '') {
                            notifyWarning('Vui lòng không để trống Tên khách hàng');
                            return;
                        };
                        data.BirthDay = GetValueDate($('#BirthDay').datepicker('getDate'));
                        _AjaxPost("/Customer/Insert_Update", { data }, function (rs) {
                            if (rs.success && rs.customer != null) {
                                $("#mdlCustom").modal("hide");
                                notifySuccess("Thêm khách hàng thành công!");
                                var data = rs.customer;
                                $("#CustomerId").val(data.CustomerId);

                                var item = {
                                    id: data.Id + "",
                                    text: data.Name,
                                    CustomerId: data.Id,
                                    Name: data.Name,
                                    CustomerCode: data.CustomerCode,
                                    Phone: data.Phone,
                                    Point: data.Point,
                                    CardNumber: data.CardNumber
                                }
                                $("#drdCustomer").select2("trigger", "select", {
                                    data: item
                                });
                            }
                            else {
                                notifyError(rs.message);
                            }
                        });
                    });
                });
                
            });
            $("#btnSyncProduct").on("click", function () {
                app.component.Loading.Show();
                _AjaxPost("/Products/ProductSync", { WarehouseId: $("#drdWarehouse").val() }, function (rs) {
                    app.component.Loading.Hide();
                    notify(rs);
                });
            });
            $("#btnGetCardNumber").on("click", function () {
                _AjaxPost("/Customer/GetNewCardNumber", null, function (rs) {
                    if (rs.success) {
                        $("#createCustomerModal #CardNumber").val(rs.cardNumber);
                    }
                    else {
                        notifyError("Hết Thẻ!")
                    }
                });
            });
            $("#btnSaveOrder").on("click", function () {
                disable(this);
                OrderUpdate.component.OrderSubmit($(this).attr("data-status"));
                enable(this);
            });
            $("#btnPaymentOrder").on("click", function () {
                disable(this);
                OrderUpdate.component.OrderSubmit($(this).attr("data-status"));
                enable(this);
            });
            $("#frmAddCustomer").on("submit", function (ev) {
                app.component.Loading.Show();
            });
            $("#btnCancel").on("click", function () {
                if ($("#Id").val() != null) {
                    modal.DeleteComfirm({
                        message: "Bạn có muốn hủy đơn hàng?",
                        callback: function () {
                            _AjaxPost("/Orders/DeleteOrder", { Id: $("#Id").val() }, function (rs) {
                                if (rs.kq) {
                                    notifySuccess("Xóa Đơn Hàng Thành Công!");
                                    window.setTimeout(function () {
                                        window.location.href = "/Orders/Create";
                                    }, 800)
                                }
                                else {
                                    notifyError("Xóa Đơn Hàng Thất Bại!");
                                }
                            });
                        }
                    })
                }
            });
            $("#checkUsePoint").on('change', function () {
                var maxpoint = $('#maxpoint').val();
                if ($(this).prop('checked')) {
                    $("#usePoint").val(maxpoint);
                }
                OrderUpdate.component.SetAutoPrice();
            });
            $("#tblDataProduct").on("keyup", '.Quantity[ipPosInt]', function () {
                var value = 0;
                if ($(this).val() == 0 || $(this).val() == '') {
                    $(this).attr("style", "border-color:#d43f3a");
                    $(this).attr("data-error", true);
                    $("#btnSaveOrder").attr("disabled", "disabled");
                    $("#btnPaymentOrder").attr("disabled", "disabled");
                }
                else {
                    $(this).attr("style", "border-color:#ccc");
                    $(this).removeAttr("data-error");
                    $("#btnSaveOrder").removeAttr("disabled");
                    $("#btnPaymentOrder").removeAttr("disabled");
                    value = parseInt(getPosInt_AutoNumeric($(this).val()));
                }
                var index = $(this).closest('tr').data("index");
                var data = $("#tblDataProduct").bootstrapTable("getData");
                data[index].Quantity = getPosInt_AutoNumeric($(this).val());
                data[index].TotalPrice = data[index].Quantity * data[index].SellPrice - data[index].Quantity * data[index].SellPrice * data[index].Discount / 100;
                $(this).closest('tr').find(".totalprice").text(formatMoney(data[index].TotalPrice));
                OrderUpdate.component.checkShowPromotion();
                OrderUpdate.component.SetAutoPrice();
            });

            $("#tblDataProduct").on("mousewheel", '.Quantity[ipPosInt]', function () {
                if ($(this).val() == 0 || $(this).val() == '') {
                    $(this).attr("style", "border-color:#d43f3a");
                    $(this).attr("data-error", true);
                    $("#btnSaveOrder").attr("disabled", "disabled");
                    $("#btnPaymentOrder").attr("disabled", "disabled");
                }
                else {
                    $(this).attr("style", "border-color:#ccc");
                    $(this).removeAttr("data-error");
                    $("#btnSaveOrder").removeAttr("disabled");
                    $("#btnPaymentOrder").removeAttr("disabled");
                }
                var index = $(this).closest('tr').data("index");
                var data = $("#tblDataProduct").bootstrapTable("getData");
                data[index].Quantity = getPosInt_AutoNumeric($(this).val());
                data[index].TotalPrice = data[index].Quantity * data[index].SellPrice - data[index].Quantity * data[index].SellPrice * data[index].Discount / 100;
                $(this).closest('tr').find(".totalprice").text(formatMoney(data[index].TotalPrice));
                OrderUpdate.component.checkShowPromotion();
                OrderUpdate.component.SetAutoPrice();
            });
            $("#tblProductPromotion").on("keyup", '.Quantity[ipPosInt]', function () {
                if ($(this).val() == 0 || $(this).val() == '') {
                    $(this).attr("style", "border-color:#d43f3a");
                    $(this).attr("data-error", true);
                    $("#btnSaveOrder").attr("disabled", "disabled");
                    $("#btnPaymentOrder").attr("disabled", "disabled");
                }
                else {
                    $(this).attr("style", "border-color:#ccc");
                    $(this).removeAttr("data-error");
                    $("#btnSaveOrder").removeAttr("disabled");
                    $("#btnPaymentOrder").removeAttr("disabled");
                }
                var index = $(this).closest('tr').data("index");
                var data = $("#tblProductPromotion").bootstrapTable("getData");
                data[index].Quantity = getPosInt_AutoNumeric($(this).val());
            });
            $("#tblProductPromotion").on("mousewheel", '.Quantity[ipPosInt]', function () {
                if ($(this).val() == 0 || $(this).val() == '') {
                    $(this).attr("style", "border-color:#d43f3a");
                    $(this).attr("data-error", true);
                    $("#btnSaveOrder").attr("disabled", "disabled");
                    $("#btnPaymentOrder").attr("disabled", "disabled");
                }
                else {
                    $(this).attr("style", "border-color:#ccc");
                    $(this).removeAttr("data-error");
                    $("#btnSaveOrder").removeAttr("disabled");
                    $("#btnPaymentOrder").removeAttr("disabled");
                }
                var index = $(this).closest('tr').data("index");
                var data = $("#tblProductPromotion").bootstrapTable("getData");
                data[index].Quantity = getPosInt_AutoNumeric($(this).val());
            });

            $("#PayCash").on("keyup", function (e) {
                OrderUpdate.component.SetAutoPrice();
            });
            $("#PayCash").on("mousewheel", function (e) {
                OrderUpdate.component.SetAutoPrice();
            });
            $("#PayByCard").on("keyup", function (e) {
                OrderUpdate.component.SetAutoPrice();
            });
            $("#PayByCard").on("mousewheel", function (e) {
                OrderUpdate.component.SetAutoPrice();
            });
            $("#Voucher").on("keyup", function () {
                $("#Discount").text($(this).val());
                OrderUpdate.component.SetAutoPrice();
            });
            $("#Voucher").on("mousewheel", function () {
                $("#Discount").text($(this).val());
                OrderUpdate.component.SetAutoPrice();
            });
            $("#CouponCode").on("keyup", function (e) {
                OrderUpdate.component.checkCoupon();
            });
            $("input[name='opDis']").on("change", function () {
                if ($(this).val() == 1) {
                    $("#divCoupon").show();
                    $("#divVoucher").hide();
                    OrderUpdate.component.checkCoupon();
                }
                else {

                    $("#divCoupon").hide();
                    $("#divVoucher").show();
                    $("#Voucher").focus();
                    $("#CouponInfo").html('');
                    $("#CouponValue").val('');
                    $("#CouponType").val('');
                    $("#Discount").text(0);
                }
                OrderUpdate.component.SetAutoPrice();
            });
            $("input[name='checkboxpaybycard']").on('change', function () {
                if ($(this).prop('checked')) {
                    $("#cellPayCash").removeAttr('colspan');
                    $("#cellPayByCard").show();
                    $("#PayByCard").focus();
                }
                else {
                    $("#cellPayCash").attr('colspan', 2);
                    $("#cellPayByCard").hide();
                    $("#PayCash").focus();
                }
                OrderUpdate.component.SetAutoPrice();
            });
            $("#usePoint").on("keyup", function (e) {
                var usedpoint = parseInt($(this).val());
                var maxpoint = parseInt($('#maxpoint').val());
                if ($("#checkUsePoint").prop('checked')) {
                    $("#checkUsePoint").prop('checked', false);
                }
                if (usedpoint == maxpoint){
                    $("#checkUsePoint").trigger('click');
                }
                else if (usedpoint > maxpoint) {
                    notifyError("Số điểm tối đa là: " + maxpoint);
                    $("#checkUsePoint").trigger('click');
                }
                else {
                    OrderUpdate.component.SetAutoPrice();
                }
            });
        },
        OrderSubmit: function (status) {
            var dataOrderDetail = $("#tblDataProduct").bootstrapTable("getData");
            var usedpoint = parseInt($('#usePoint').val());
            var maxpoint = parseInt($('#maxpoint').val());
            if (dataOrderDetail.length <= 0) {
                notifyWarning("Chọn sản phẩm!", "Yêu cầu");
            }
            else if (usedpoint > maxpoint) {
                notifyWarning("Số điểm tối đa là:" + maxpoint, "Yêu cầu");
            }
            else if (status == 1 && getNumber_AutoNumeric($("#RefundMoney").text()) < 0) {
                notifyWarning("Khách đưa thiếu tiền!", "Yêu cầu");
            }
            else {
                var kt = 0;
                var it;
                var dataProductPromption = $("#tblProductPromotion").bootstrapTable("getData");
                var dataProductSell = $("#tblDataProduct").bootstrapTable("getData");
                var message = '';
                dataProductSell.forEach(function (itSell) {
                    dataProductPromption.forEach(function (itProm) {
                        if (itSell.Barcode == itProm.Barcode && (parseInt(itSell.Quantity) + parseInt(itProm.Quantity)) > (parseInt(itSell.InventoryNumber) + parseInt(itSell.Quantity) + parseInt(itProm.Quantity))) {
                            message += '*Số lượng bán + khuyến mại của sản phẩm ' + itSell.ProductName + ' - ' + itSell.Barcode + ' vượt quá giới hạn!';
                        }
                    })
                })
                dataProductSell.forEach(function (item) {
                    if (item.Quantity == 0 || item.Quantity == "") {
                        message += '*Số lượng của sản phẩm ' + item.ProductName + ' - ' + item.Barcode + ' phải > 0!';
                    }
                    if (item.Quantity > item.InventoryNumber) {
                        message += '*Số lượng của sản phẩm ' + item.ProductName + ' - ' + item.Barcode + ' vượt quá giới hạn!';
                    }
                });
                if (message != '') {
                    notifyError(message);
                }
                else {
                    app.component.Loading.Show();
                    var orderUpdate = {
                        OrderModel: {},
                        OrderDetail: dataProductSell.map(function (item) {
                            item.Discount = item.Discount.toString().replace('.', ',');
                            return item;
                        }),
                        OrderPromotion: dataProductPromption
                    };
                    var paybycard = $('#checkboxpaybycard').prop('checked') == true ? getMoney_AutoNumeric($("#PayByCard").val()) : null;
                    if ($("input[name='opDis']:checked").val() == 1) {
                        orderUpdate.Order = {
                            Id: $("#Id").val(),
                            Code: $("#Code").val(),
                            WarehouseId: $("#drdWarehouse").val(),
                            CustomerId: $("#CustomerId").val(),
                            CouponCode: $("#CouponCode").val(),
                            CouponType: $("#CouponType").val(),
                            PointUsed: $("#usePoint").val(),
                            PayCash: getMoney_AutoNumeric($("#PayCash").val()),
                            PayByCard: paybycard,
                            Status: status
                        }
                    }
                    else {
                        orderUpdate.Order = {
                            Id: $("#Id").val(),
                            Code: $("#Code").val(),
                            WarehouseId: $("#drdWarehouse").val(),
                            CustomerId: $("#CustomerId").val(),
                            Voucher: getMoney_AutoNumeric(($("#Voucher").val() == null || $("#Voucher").val() == "") ? 0 : $("#Voucher").val()),
                            PointUsed: $("#usePoint").val(),
                            PayCash: getMoney_AutoNumeric($("#PayCash").val()),
                            PayByCard: paybycard,
                            Status: status
                        }
                    }
                    _AjaxPost("/Orders/SubmitOrder", { orderRequest: orderUpdate }, function (rs) {

                        app.component.Loading.Hide();
                        if (rs.success) {
                            if (status == 0) {
                                notifySuccess("Lưu Đơn Hàng Thành Công!");
                                setTimeout(function () {
                                    window.location.href = "/Orders/Create";
                                }, 1000)
                            }
                            else {
                                notifySuccess("Thanh Toán Đơn Hàng Thành Công!");
                                _AjaxPost("/Orders/PrintBillOrder", { Id: rs.Id }, function (rs) {
                                    $("#BillContent").html(rs);
                                    app.component.FormattxtMore();
                                    $("#barcodecontainer #barcode").html(DrawHTMLBarcode_Code128A($("#barcodecontainer #barcode").text(), "yes", "in", 0, 2.5, 1, "bottom", "center", "", "black", "white"));
                                    $("body").append("<iframe style='display:none' id='printOrder' name = 'printOrder'>")

                                    var newWin = window.frames["printOrder"];
                                    newWin.document.write('<html><head><title>In Hóa Đơn</title></head><body onload="window.print()">' + $("#BillContent").html() + '</body></html>');
                                    newWin.document.close();
                                    setTimeout(function () {
                                        window.location.href = "/Orders/Create";
                                    }, 1000)
                                });
                            }
                        }
                        else {
                            notifyError(rs.message);
                        }
                    })
                }
            }
        },
        SetAutoPrice: function () {
            var dataOrderDetail = $("#tblDataProduct").bootstrapTable("getData");
            var TotalPrice = 0;
            dataOrderDetail.forEach(function (item) {
                TotalPrice += parseFloat(item.TotalPrice);
            });
            $("#ProductTotal").text(formatMoney(TotalPrice));
            var totalDiscount = 0;
            if ($("input[name='opDis']:checked").val() == 1) {
                if ($("#CouponValue").val() != '' && $("#CouponType").val() != '') {
                    if ($("#CouponType").val() == 1) {
                        TotalPrice = TotalPrice - $("#CouponValue").val();
                    }
                    if ($("#CouponType").val() == 2) {
                        var totalDiscount = TotalPrice * parseInt($("#CouponValue").val()) / 100;
                        if ($("#MaxUseCoupon").val() != "" && totalDiscount > $("#MaxUseCoupon").val()) {
                            totalDiscount = $("#MaxUseCoupon").val();
                        }
                        TotalPrice -= totalDiscount;
                    }
                }
            }
            else {
                if ($("#Voucher").val() != '') {
                    totalDiscount = getMoney_AutoNumeric($("#Voucher").val());
                    if (totalDiscount > TotalPrice) {
                        totalDiscount = TotalPrice;
                        TotalPrice = 0;
                    }
                    else {
                        TotalPrice -= getMoney_AutoNumeric($("#Voucher").val());
                    }
                }
            }
            $("#Discount").text(formatMoney(totalDiscount));
            //// get tiền giảm - tích điểm

      
            if ($("#CustomerId").val() != '') {
                var maxpoint = $('#maxpoint').val();
                if ($("#checkUsePoint").prop('checked')) {
                    if (parseFloat($("#ipMoneyAllPoint").val()) > TotalPrice) {
                        TotalPrice = 0;
                    }
                    else {
                        TotalPrice = TotalPrice - parseFloat($("#ipMoneyAllPoint").val());
                    }
                }
                else {
                    var usepoint = parseFloat($('#usePoint').val() == '' ? 0 : $('#usePoint').val()) * 1000;
                    if (usepoint > TotalPrice) {
                        TotalPrice = 0;
                    }
                    else {
                        TotalPrice = TotalPrice - usepoint;
                    }
                }
            }
            $("#GrandTotal").text(formatNumber(TotalPrice <= 0 ? 0 : TotalPrice));
            if (TotalPrice <= 0) {
                $("#newPoint").text(0);
                $("#RefundMoney").text(0);
                TotalPrice = 0;
            }
            else {
                if ($("#CustomerId").val() != '') {
                  
                    if (!($("#ckbVoucher").is(":checked")) && ($("#Voucher").val() == '' || $("#Voucher").val() == 0)) {
                   
                        _AjaxPost("/Orders/AccumulationPoint", { totalPrice: TotalPrice }, function (rs) {
                            $("#newPoint").text(formatNumber(rs));
                        });
              
                    }
                    else {
                        $("#newPoint").text(0);
                    }
                }
            }
            var payQuest = parseFloat(getMoney_AutoNumeric($("#PayCash").val()) == '' ? 0 : getMoney_AutoNumeric($("#PayCash").val()));
            if ($("input[name='checkboxpaybycard']").prop("checked")) {
                payQuest = payQuest + parseFloat(getMoney_AutoNumeric($("#PayByCard").val()) == '' ? 0 : getMoney_AutoNumeric($("#PayByCard").val()));
            }
            $("#RefundMoney").text(formatNumber(payQuest - TotalPrice));
        },
        Select2Warehouse: function () {
            $("#drdWarehouse").select2().on('change', function () {
                _AjaxPost('/Orders/GetQuotaPromotion', { WarehouseId: $(this).val() }, function (rs) {
                    $('#QuotaPromotion').val(rs);
                    $("#tblDataProduct").bootstrapTable('removeAll');
                    $('#tblProductPromotion').bootstrapTable('removeAll');
                    $('.promotion').hide();
                    OrderUpdate.component.SetAutoPrice();
                })
                //get lại quotaPromotion
            });
        },
        drdCardNumber: function () {
            $("#drdCardnumber").select2({
                placeholder: 'Nhâp mã thẻ khách hàng.',
                minimumInputLength: 0,
                allowClear: true,
                width: "100%",
                ajax: {
                    delay: 500,
                    type: 'POST',
                    url: '/Cardnumbers/GetDataExist',
                    dataType: 'json',
                    processResults: function (data, params) {
                        return {
                            results: $.map(data.data, function (item) {
                                return {
                                    id: item.CardNumberId,
                                    text: item.CardNumberId
                                };
                            })
                        };
                    }
                }
            });
            $('#CardNumberId').select2({
                placeholder: 'Chọn mã thẻ',
                minimumInputLength: 0,
                allowClear: true,
                width: "100%",
                ajax: {
                    delay: 500,
                    type: 'POST',
                    url: '/CardNumbers/CardNumber_AutoComplete',
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
                                    id: item.CardNumberId,
                                    text: item.CardNumberId,
                                    data: item,
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
                    var $result = $("<span>" + data.id + "</span>");

                    return $result;
                }
            })
                .on('select2:select', function (evt) {
                    var item = evt.params.data;
                    if (item != null) {
                        $("#CardNumberId").val(item.id);

                    }
                });
        },
        selectCardNumber: function () {
            $("#SelectCardNumber").select2({
                placeholder: 'Nhập mã thẻ.',
                minimumInputLength: 1,
                allowClear: true,
                width: "100%",
                ajax: {
                    delay: 500,
                    type: 'POST',
                    url: '/Cardnumbers/AutoCompleteCardNumber',
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
                                    id: item.CardNumberId + "",
                                    text: item.CardNumberId
                                    //CustomerId: item.Id,
                                    //Name: item.Name,
                                    //CustomerCode: item.CustomerCode,
                                    //Phone: item.Phone,
                                    //Point: item.Point,
                                    //CardNumber: item.CardNumberId
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
                    var $result = $("<span>" + data.id + "</span>");
                    //$result.append(" <span class='badge'>" + data.Phone + "</span>");
                    //$result.append(" <span class='label label-primary'>" + data.CustomerCode + "</label>");
                    return $result;
                }
            })
                .on('select2:select', function (evt) {
                    var item = evt.params.data;
                    if (item != null) {
                        $("#CardNumberId").val(item.id);
                    }
                })
                .on('change', function (evt) {
                    if ($(this).val() == null) {
                        $("#CardNumberId").val("");
                    }
                })
        }
    }
};
$(document).ready(function () {
    $("#ckbVoucher").change(function () {
        //alert("before reset :"+$("#Voucher").val());
        $("#Voucher").val("");
        //alert("after reset :" + $("#Voucher").val());
        ($("#ckbVoucher").is(":checked")) ? $("#Voucher").removeAttr("disabled") : $("#Voucher").attr("disabled", "disabled");
        OrderUpdate.component.SetAutoPrice();
    });
    OrderUpdate.load();
    $(document).on("keyup", function (e) {
        if (e.which == 113) {
            $("#drdProduct").select2("open");
        }
        if (e.which == 115) {
            $('#PayCash').focus();
        }
        if (e.which == 118) {
            //F7
            $("#checkboxpaybycard").trigger('click');
        }
        if (e.which == 119) {
            //F8
            $("#btnPaymentOrder").trigger('click');
        }
        if (e.which == 120) {
            if (OrderUpdate.component.checkShowPromotion()) {
                //F9
                if ($("#drdProduct").is(':visible')) {
                    $("#drdProductPromotion").select2("open");
                }
            } else {
                alert("Phím chức năng không được phép sử dụng.");
            }
        }
    });

    $(document).keydown(function (event) {
        if (event.ctrlKey && event.keyCode == 49) {
            $("#drdCustomer").select2("close");
            $("#drdProduct").select2("close");
            $("#drdWarehouse").select2("open");
            event.preventDefault();
        }
        else if (event.ctrlKey && event.keyCode == 50) {
            $("#drdProduct").select2("close");
            $("#drdWarehouse").select2("close");
            $("#drdCustomer").select2("open");
            event.preventDefault();
        }
        if (event.ctrlKey && event.keyCode == 51) {
            $("#Voucher").focus();
            event.preventDefault();
        } if (event.ctrlKey && event.keyCode == 52) {
            $("#btnSaveOrder").trigger('click');
            event.preventDefault();
        }
    });
});