$(document).ready(function () {
    $("#ipSearch").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#tblCategory tbody tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    $("#btnImportExcelCategory").click(function () {
        $("#ipIECategory").trigger("click");
    });

    $("#ipIECategory").on("change", function () {
        app.component.Loading.Show();
        var formData = new FormData($('form.form-inline')[0]);
        $.ajax({
            url: "/Category/ReadFileImportExcel",
            method: "POST",
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (rs) {
                $("#ipIECategory").val(null);
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
                    $("#tblCategoryImport").bootstrapTable({
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
                                title: "Mã danh mục",
                                field: "Code",
                                align: 'left',
                                valign: 'middle',
                                sortable: true,
                            },
                            {
                                title: "Tên danh mục",
                                field: "Name",
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
                                            $("#tblCategoryImport").bootstrapTable("remove", { field: 'Code', values: [row.Code] });
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
                    $("#tblCategoryImport").bootstrapTable('removeAll');
                    if (rs.lstCate.length > 0) {
                        $("#tblCategoryImport").bootstrapTable('load', rs.lstCate);

                    }
                    $('#btnAddCategoryImport').removeAttr('disabled');
                    $("#CategoryImport").modal("show");
                    $("#CategoryImport .modal-dialog").attr("style", "width:80%;");
                }
                else {
                    notifyError(rs.message);
                }
            }
        });
    });


    $("#btnAddCategoryImport").on("click", function () {
        app.component.Loading.Show();
        var dataImport = $("#tblCategoryImport").bootstrapTable('getData');

        var data = $.map(dataImport, function (e) {
            if (e.ImportMessage == "") {
                return {
                    Name: e.Name,
                    Code: e.Code
                }
            }
        });

        if (data.length > 0) {
            _AjaxPost("/Category/UpdateImportExcel", { data: data }, function (rs) {
                app.component.Loading.Hide();
                notifySuccess("Import xong");
                $("#tblCategoryImport").bootstrapTable('load', rs.data);
            });
        }
        else {
            app.component.Loading.Hide();
            notifyWarning("Không có dữ liệu đúng để Import");
        }
        $(this).attr('disabled', 'true');
    });
    $('.btnEdit').on('click', function () {
        app.component.Loading.Show();
        cusmodal.ShowView('/Category/Edit/'+$(this).attr('data-id'), function () {
            $('#drdParentCategory').select2({
                placeholder: 'Nhâp tên, mã danh mục.',
                allowClear: true,
                width: '100%'
            });
            $('#frmCategory').on('submit', function (event) {
                event.preventDefault();
                var data = $(this).serializeObject();
                if (data.Code.trim() == '' || data.Name.trim() == '') {
                    notifyWarning('Vui lòng không nhập trống Mã hoặc Tên');
                    return;
                };
                _AjaxPost('/Category/Insert_Update', { category: data }, function (rs) {
                    if (rs.warning) {
                        notifyWarning(rs.message);
                    }
                    else {
                        if (rs.success) {
                            notifySuccess('Cập nhật thành công!');
                            $('#mdlCustom').modal('hide');
                            setTimeout(function () { window.location.reload(); }, 1000);
                        }
                        else {
                            notifyError('Cập nhật thất bại!');
                            console.log(rs.message);
                        }
                    }
                });
            });
        });
    });
    $('.btnDelete').on('click', function () {
        var id = $(this).attr('data-id');
        modal.DeleteComfirm({
            tilte: "Xóa",
            callback: function () {
                _AjaxPost('/Category/Delete', { Id: id }, function (rs) {
                    if (rs.success) {
                        notifySuccess("Xóa danh mục thành công!");
                        setTimeout(function () {
                            window.location.reload();
                        }, 1000);
                    }
                    else notifyError("Xóa danh mục không thành công!")
                });
            }
        })
    });
    $("#btnAddCategory").on('click', function () {
        cusmodal.ShowView('/Category/Create', function () {
            $('#drdParentCategory').select2({
                placeholder: 'Nhâp tên, mã danh mục.',
                allowClear: true,
                width: '100%'
            });
            $('#frmCategory').on('submit', function (event) {
                event.preventDefault();
                var data = $(this).serializeObject();
                if (data.Code.trim() == '' || data.Name.trim() == '') {
                    notifyWarning('Vui lòng không nhập trống Mã hoặc Tên');
                    return;
                };
                disable('#btnSubmit');
                _AjaxPost('/Category/Insert_Update', { category: data }, function (rs) {
                    if (rs.warning) {
                        enable('#btnSubmit');
                        notifyWarning(rs.message);
                    }
                    else {
                        if (rs.success) {
                            notifySuccess('Thêm mới thành công!');
                            $('#mdlCustom').modal('hide');
                            setTimeout(function () { window.location.reload(); }, 1000);
                        }
                        else {
                            enable('#btnSubmit');
                            notifyError('Thêm mới thất bại!');
                            console.log(rs.message);
                        }
                    }
                });
            });
        });
    })
    $('#CategoryImport').on('hidden.bs.modal', function (e) {
        window.location.reload();
    })
});