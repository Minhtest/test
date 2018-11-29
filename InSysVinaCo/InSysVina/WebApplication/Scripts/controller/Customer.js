var Customer = {
    ui: {

    },
    load: function () {
        Customer.component.table();
       
        //app.component.DatePicker();
        $("#btnAddCustomer").click(function () {
            app.component.Loading.Show();
            cusmodal.ShowView("/Customer/Create", function () {
                //tạo drdCardNumber
                //Customer.component.drdShowroom();
                Customer.component.drdCardNumber();

                app.component.DatePicker();
                app.component.ValidateInputPhone();
                app.component.ValidateEmail();
                app.component.Loading.Hide();
                Customer.component.selectCardNumber();
               
                $("#frmAddCustomer").on("submit", function (event) {
                    event.preventDefault();
                    var data = $(this).serializeObject();
                    if (data.Name.trim() == '') {
                        notifyWarning('Vui lòng không để trống Tên khách hàng');
                        return;
                    };
                    data.BirthDay = GetValueDate($('#BirthDay').datepicker('getDate'));
                    //disable('#btnSubmit');
                    _AjaxPost("/Customer/Insert_Update", { data }, function (rs) {
                        if (rs.success) {
                            $("#tblCustomer").bootstrapTable("refresh");
                            $("#mdlCustom").modal("hide");
                            if (data.Id == '') notifySuccess("Thêm khách hàng thành công!");
                            else notifySuccess("Cập nhật khách hàng thành công!");
                            //if (rs.message != "") notifyWarning(rs.message);
                        }
                        else {
                            enable('#btnSubmit');
                            notifyError(rs.message);
                            console.log(rs.message)
                        }
                    })
                });
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
        $("#btnExportExcelCustomer").on("click", function () {
            _AjaxPost("/Customer/Export", {
                txtSearch: $("#tblCustomer").bootstrapTable("getOptions").searchText
            }, function (rs) {
                SaveFileAs(rs.urlFile, rs.fileName);
            });
        });
        $("#btnSortOrderMax").on("click", function () {
            $("#tblCustomer").bootstrapTable("refreshOptions", {
                sortName: "CountBuy",
                sortOrder: "desc"
            });
        });
        $("#btnSortTotalMax").on("click", function () {
            $("#tblCustomer").bootstrapTable("refreshOptions", {
                sortName: "TotalBuy",
                sortOrder: "desc"
            });
        });
        $("#btnImportExcel").on("click", function () {
            $("input#fileImportExcelCustomer").trigger("click");
        });
        $("#fileImportExcelCustomer").on("change", function () {
            app.component.Loading.Show();
            var formData = new FormData($('form.form-inline')[0]);
            $.ajax({
                url: "/Customer/ReadFileImportExcel",
                method: "POST",
                data: formData,
                cache: false,
                contentType: false,
                processData: false,
                success: function (rs) {
                    $("#fileImportExcelCustomer").val(null);
                    app.component.Loading.Hide();
                    if (rs.success) {
                        if (rs.listError.length > 0) {
                            $("#txtLstError").text("Có " + rs.listError.length + " lỗi trong file Excel.");
                            var htmlError = "";
                            rs.listError.forEach(function (item) {
                                htmlError += "<tr><td>" + item + "</td></tr>";
                            });
                            $("#tblImportError").html(htmlError);
                        }
                        else {
                            $("#txtLstError").text("File excel không có lỗi!");
                        }
                        $("#tblCustomersImport").bootstrapTable({
                            striped: true,
                            pagination: true,
                            limit: 10,
                            pageSize: 10,
                            pageList: [10, 25, 50, 100, 200],
                            search: true,
                            showColumns: true,
                            showRefresh: true,
                            minimumCountColumns: 2,
                            columns: [
                                {
                                    title: "Tên Khách Hàng",
                                    field: "Name",
                                    align: 'left',
                                    valign: 'middle',
                                    sortable: true,
                                },
                                {
                                    title: "Mã Thẻ",
                                    field: "CardNumberId",
                                    align: 'center',
                                    valign: 'middle',
                                    sortable: true,
                                    formatter: function (value, row, index) {
                                        return (value == null ? '' : value);
                                    }
                                },
                                {
                                    title: "Ngày Sinh",
                                    field: "BirthDay",
                                    align: 'center',
                                    valign: 'middle',
                                    sortable: true,
                                    formatter: function (value, row, index) {
                                        if (value == null) {
                                            return '';
                                        }
                                        else return formatToDate(value);
                                    }
                                },
                                {
                                    title: "Số Điện Thoại",
                                    field: "Phone",
                                    align: 'center',
                                    valign: 'middle',
                                },
                                {
                                    title: "Địa Chỉ",
                                    field: "Address",
                                    align: 'left',
                                    valign: 'middle',
                                },
                                {
                                    title: "Email",
                                    field: "Email",
                                    align: 'center',
                                    valign: 'middle',
                                },
                                {
                                    title: "Xóa",
                                    align: 'center',
                                    valign: 'middle',
                                    formatter: function (value, row, index) {
                                        return '<a class="remove" href="javascript:void(0)" style="color:red" title="Xóa"><i class="glyphicon glyphicon-remove"></i></a>';
                                    },
                                    events: {
                                        'click .remove': function (e, value, row, index) {
                                            if (row != null) {
                                                $("#tblCustomersImport").bootstrapTable("remove", { field: 'CardNumber', values: [row.CardNumber] });
                                            }
                                        }
                                    }
                                },
                                {
                                    title: "Thông tin Import",
                                    field: "ImportMessage",
                                    align: 'center',
                                    valign: 'middle',
                                    sortable: true,
                                    formatter: function (value, row, index) {
                                        if (value == "") {
                                            return 'Chưa thực hiện';
                                        }
                                        else if (value == "1") {
                                            return "<i style='color:green'>Thành Công</i>";
                                        }
                                        else {
                                            return "<i style='color:red'>" + value + "</i>";
                                        }
                                    }
                                }
                            ]
                        });
                        $("#tblCustomersImport").bootstrapTable('removeAll');
                        if (rs.lstCus.length > 0) {

                            $("#tblCustomersImport").bootstrapTable('load', rs.lstCus);
                        }
                        $('#btnAddCustomersImport').removeAttr('disabled');
                        $("#CustomersImport").modal("show");
                        $("#CustomersImport .modal-dialog").attr("style", "width:80%;");
                    }
                    else {
                        console.log(rs.message);
                        notifyError("Lỗi đọc file!");
                    }
                }
            });
        });
        $("#btnAddCustomersImport").on("click", function () {
            app.component.Loading.Show();
            var dataImport = $("#tblCustomersImport").bootstrapTable('getData');

            var data = $.map(dataImport, function (e) {
                if (e.ImportMessage == "") {
                    return {
                        Name: e.Name,
                        CardNumberId: e.CardNumberId,
                        Email: e.Email,
                        Phone: e.Phone,
                        Address: e.Address,
                        BirthDay: GetValueDate(e.BirthDay)
                    }
                }
            });
            if (data.length > 0) {
                _AjaxPost("/Customer/UpdateImportExcel", { data }, function (rs) {
                    app.component.Loading.Hide();
                    notifySuccess("Import xong");
                    $("#tblCustomersImport").bootstrapTable('load', rs.data);
                    $("#tblCustomer").bootstrapTable("refresh");
                });
            }
            else {
                app.component.Loading.Hide();
                notifyWarning("Không có dữ liệu đúng để Import");
            }
            $(this).attr('disabled', 'true');
        });
 
    },
    component: {
        table: function () {
            $("#tblCustomer").bootstrapTable({
                url: "/Customer/GetDataCustomer",
                method: "POST",
                ajax: function (config) {
                    app.component.Loading.Show();
                    _AjaxPost(config.url, config.data, function (rs) {
                        app.component.Loading.Hide();
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
                        title: 'Thao Tác',
                        align: 'center',
                        valign: 'middle',
                        formatter: function (value, row, index) {
                            var htmlCell = '<a href="javascript:void(0)" class="btnEdit" data-toggle="modal" data-target="#createCustomerModal"><i class="fas fa-edit"></i></a>';
                            if (row['AllowDelete']) {
                                htmlCell += '<a href="javascript:void(0)" style="margin-left:15px;" class="btnRemove"><i class="fas fa-trash-alt"></i></a>'
                            }
                            return htmlCell;
                        },
                        events: {
                            'click .btnEdit': function (e, value, row, index) {
                                app.component.Loading.Show();
                                cusmodal.ShowView("/Customer/Edit/" + row.Id, function () {
                                    //tạo drdCardNumber
                                    //Customer.component.drdShowroom();
                                    Customer.component.drdCardNumber();
                                    Customer.component.selectCardNumber();
                                    if (row.CardNumberId != null) {
                                        var newOption = new Option(row.CardNumberId, row.CardNumberId, true, true);
                                        // Append it to the select
                                        $('#SelectCardNumber').append(newOption).trigger('change');
                                    }
                                    app.component.DatePicker();
                                    app.component.ValidateInputPhone();
                                    app.component.Loading.Hide();
                                    
                                    $("#frmAddCustomer").on("submit", function (event) {
                                        event.preventDefault();
                                        var data = $(this).serializeObject();
                                        data.BirthDay = GetValueDate($('#BirthDay').datepicker('getDate'));
                                        if (data.Name.trim() == '') {
                                            notifyWarning('Vui lòng không để trống Tên khách hàng');
                                            return;
                                        };
                                        _AjaxPost("/Customer/Insert_Update", { data }, function (rs) {
                                            if (rs.success) {
                                                $("#tblCustomer").bootstrapTable("refresh");
                                                $("#mdlCustom").modal("hide");
                                                if (data.Id == '') notifySuccess("Thêm khách hàng thành công!");
                                                else notifySuccess("Cập nhật khách hàng thành công!");
                                         
                                            }
                                            else {
                                                notifyError(rs.message);
                                                console.log(rs.message)
                                            }
                                        })
                                    });
                                });
                            },
                            'click .btnRemove': function (e, value, row, index) {
                                modal.DeleteComfirm({
                                    tilte: "Xóa",
                                    callback: function () {
                                        _AjaxPost('/Customer/DeleteCustomer', { CustomerId: row['Id'] }, function (rs) {
                                            if (rs.success) {
                                                notifySuccess("Xóa khách hàng thành công!");
                                                $("#tblCustomer").bootstrapTable("refresh");
                                            }
                                            else notifyError("Xóa khách hàng không thành công!")
                                        });
                                    }
                                })
                            }
                        }
                    },
                    {
                        field: 'CustomerCode',
                        title: 'Mã Khách Hàng',
                        align: 'center',
                        valign: 'middle',
                        sortable: true
                    },
                    {
                        field: 'Name',
                        title: 'Họ Tên',
                        align: 'left',
                        valign: 'middle',
                        sortable: true
                    },
                    {
                        field: 'CardNumberId',
                        title: 'Mã Thẻ',
                        align: 'center',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            return (value == null ? '' : value);
                        }
                    },
                    {
                        field: 'BirthDay',
                        title: 'Ngày Sinh',
                        align: 'center',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            if (value == null) {
                                return '';
                            }
                            else return formatToDate(value);
                        }
                    },
                    {
                        field: 'Phone',
                        title: 'Số Điện Thoại',
                        align: 'right',
                        valign: 'middle',
                        sortable: true,

                    },
                    {
                        field: 'Address',
                        title: 'Địa Chỉ',
                        align: 'center',
                        valign: 'middle',
                        searchable: false,
                        sortable: true,
                    },
                    {
                        field: 'Email',
                        title: 'Email',
                        align: 'center',
                        valign: 'middle',
                        searchable: false,
                        sortable: true,
                    },
                    {
                        field: 'CountBuy',
                        title: 'Số hóa đơn',
                        align: 'center',
                        valign: 'middle',
                        searchable: false,
                        sortable: true,
                        formatter: function (value, row, index) {
                            if (value == null) {
                                return '';
                            }
                            else return formatNumber(value);
                        }
                    },
                    {
                        field: 'TotalBuy',
                        title: 'Tổng Tiền',
                        align: 'center',
                        valign: 'middle',
                        searchable: false,
                        sortable: true,
                        formatter: function (value, row, index) {
                            if (value == null) {
                                return '';
                            }
                            else return formatMoney(value);
                        }
                    },
                    {
                        field: 'Point',
                        title: 'Điểm',
                        align: 'center',
                        valign: 'middle',
                        searchable: false,
                        sortable: true,
                        formatter: function (value, row, index) {
                            if (value == null) {
                                return '';
                            }
                            else return formatNumber(value);
                        }
                    },
                    {
                        field: 'CreateDate',
                        title: 'Ngày Tạo',
                        align: 'center',
                        valign: 'middle',
                        searchable: false,
                        sortable: true,
                        formatter: function (value, row, index) {
                            if (value == null) {
                                return '';
                            }
                            else return formatToDate(value);
                        }
                    }
                ]
            })
        },
        drdCardNumber: function () {
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
                            }),
                        };
                    }
                },
            })
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
                                    text: item.CardNumberId,
                                    disabled: !item.IsVerify,
                                    data: item,
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
                    console.log(data);
                    if (data.loading)
                        return data.text;
                    var $result;
                    if (data.data.IsVerify) {
                        $result = $('<span  style="color:blue">' + data.id + '</span>');
                    }
                    else {
                        $result  = $('<span style="color:red">' + data.id + " - Thẻ lỗi!" +'</span>');
                    }
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
}
$(document).ready(function () {
 
    Customer.load();

});
