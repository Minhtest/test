var CardNumber = {
    //ui: {
    //    btnSync: $('#btnSync')
    //},
    load: function () {
        CardNumber.component.tbl();
        $("#ipIECardNumber").on("change", function () {
            app.component.Loading.Show();
            var formData = new FormData($('form.form-inline')[0]);
            $.ajax({
                url: "/CardNumbers/ReadFileImportExcel",
                method: "POST",
                data: formData,
                cache: false,
                contentType: false,
                processData: false,
                success: function (rs) {
                    $("#ipIECardNumber").val(null);
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
                        $("#tblCardNumberImport").bootstrapTable({
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
                                    title: "Mã thẻ",
                                    field: "CardNumberId",
                                    align: 'left',
                                    valign: 'middle',
                                    sortable: true,
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
                                                $("#tblCardNumberImport").bootstrapTable("remove", { field: 'CardNumberId', values: [row.CardNumberId] });
                                            }
                                        }
                                    }
                                },
                                {
                                    title: "Thông tin Import",
                                    field: "ImportSuccess",
                                    align: 'center',
                                    valign: 'middle',
                                    sortable: true,
                                    formatter: function (value, row, index) {
                                 
                                        if (value == "" || value == null) {
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
                        $("#tblCardNumberImport").bootstrapTable('removeAll');
                        if (rs.lstCard.length > 0) {
                            $("#tblCardNumberImport").bootstrapTable('load', rs.lstCard);
                        }
                       
                        $("#CardNumberImport").modal("show");
                        $("#CardNumberImport .modal-dialog").attr("style", "width:80%;");
                    }
                    else {
                        notifyError(rs.message);
                    }
                }
            });
        });

        $("#btnExportExcel").on("click", function () {
            _AjaxPost("/Cardnumbers/ExportReport", {
                txtSearch: $("#txtsearch").val(),
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
        $("#btnAddCardNumber").click(function () {
            cusmodal.ShowView("/CardNumbers/Create", function () {
                CardNumber.component.listenEvent();
            });
        });
        $("#btnAddCardNumberImport").on("click", function () {

            app.component.Loading.Show();
            var dataImport = $("#tblCardNumberImport").bootstrapTable('getData');
    
            if (dataImport.length > 0) {
                $.ajax({
                    method: 'post',
                    url: '/CardNumbers/UpdateImportExcel',
                    data: { data: dataImport },
                    success: function (rs) {
                        app.component.Loading.Hide();
                        if (rs.success) notifySuccess("Import xong");
                        $("#tblCardNumberImport").bootstrapTable('load', rs.lstCard);
                    }
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
        tbl: function () {
            $("#tblCardNumber").bootstrapTable({
                url: "/Cardnumbers/GetAllData",
                method: "POST",
                ajax: function (config) {
                    app.component.Loading.Show();
                    _AjaxPost(config.url, { obj: config.data }, function (rs) {
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
                        field: 'CardNumberId',
                        title: 'Mã thẻ',
                        align: 'center',
                        valign: 'middle',
                        sortable: true
                    },
                    {
                        field: 'CustomerName',
                        title: 'Tên khách hàng',
                        align: 'center',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            if (value != null) {
                                var htmlCell = '<a href="javascript:void(0)" class="btnEdit" data-toggle="modal" data-target="#createCustomerModal">' + value+'</a>';
                                return htmlCell;
                            }
                            return '';
                        },
                        events: {
                            'click .btnEdit': function (e, value, row, index) {
                                app.component.Loading.Show();
                                cusmodal.ShowView("/Customer/Edit/" + row.CustomerId, function () {
                                    //tạo drdCardNumber
                                    //Customer.component.drdShowroom();
                                    //CardNumber.component.selectCardNumber();
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
                                            }
                                        })
                                    });
                                });
                            }
                        }
                    },
                    {
                        field: 'CustomerCode',
                        title: 'Mã khách hàng',
                        align: 'center',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            if (value != null) {
                                var htmlCell = '<a href="javascript:void(0)" class="btnEdit" data-toggle="modal" data-target="#createCustomerModal">' + value + '</a>';
                                return htmlCell;
                            }
                            return '';
                        },
                        events: {
                            'click .btnEdit': function (e, value, row, index) {
                                app.component.Loading.Show();
                                cusmodal.ShowView("/Customer/Edit/" + row.CustomerId, function () {
                                    //tạo drdCardNumber
                                    //Customer.component.drdShowroom();
                                    //CardNumber.component.selectCardNumber();
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
                                            }
                                        })
                                    });
                                });
                            }
                        }
                    },
                    {
                        field: 'TotalPoint',
                        title: 'Tổng điểm',
                        align: 'center',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            return (value === null ? '' : value);
                        }
                    },
                    {
                        field: 'UsePoint',
                        title: 'Điểm đã sử dụng',
                        align: 'center',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            return (value === null ? '' : value);
                        }
                    },
                    {
                        field: 'Point',
                        title: 'Điểm có thể dùng',
                        align: 'center',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            return (value === null ? '' : value);
                        }
                    },
                    {
                        title: 'Trạng thái',
                        align: 'center',
                        valign: 'middle',
                        sortable: true,
                        formatter: function (value, row, index) {
                            if (row.IsVerify) {
                                if (row.CustomerId == null) {
                                    return '<b style="color:green">Có thể sử dụng</b>'
                                  
                                }
                                else {
                                    if (row.IsDelCustomer == false) {
                                        return '<b style="color:red">Khách hàng đã bị hủy</b>';
                                    }
                                    //else if (!row.IsActive) {
                                    //    return '<b style="color:orange">Thẻ đã bị huỷ</b>';
                                    //}
                                    else {
                                       
                                            return '<b style="color:blue">Đang sử dụng</b>';
                                    }
                                   
                                }
           
                            }
                            else {
                                return '<b>Chưa được phép sử dụng</b>';
                            }
                        }
                    }
                ]
            })
        },
        addCard: function () {
            var data = $("#CardNumberId").val().trim();
            if (data == '') {
                notifyWarning('Vui lòng không để trống Mã thẻ');
                return;
            };
            _AjaxPost('/CardNumbers/Insert_Update', { CardNumberId: data }, function (rs) {
                if (!rs.status) {

                    notifyWarning("Thêm thất bại: " + rs.message);
                }
                else {
                    $("#CardNumberId").val("");
                    notifySuccess('Thêm mới thành công!');
                    $("#mdlCustom").modal("hide");
                }
            });
        },
        listenEvent: function () {
            $("#btnSubmit").click(function () {
                CardNumber.component.addCard();

                
                $("#tblCardNumber").bootstrapTable("refresh");
            });
            $("#CardNumberId").on('keydown', function (e) {
                if (e.which == 13) {
                    CardNumber.component.addCard();
                }
            });
        }
    }
}
$(document).ready(function () {
    $("#btnImportExcelCardNumber").click(function () {
        $("#ipIECardNumber").trigger("click");
    });

    CardNumber.load();

    $('#CardNumberImport').on('hidden.bs.modal', function (e) {
        window.location.reload();
    })
});
