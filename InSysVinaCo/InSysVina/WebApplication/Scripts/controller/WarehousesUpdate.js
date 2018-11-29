$(document).ready(function () {
    $("#drdApi").select2({
        multiple: true,
        closeOnSelect: false,
        selectOnClose: false
    });
    $("#IsSync").on("change", function () {
        if ($("#IsSync").is(":checked")) {
            $("#drdApi").val(null).trigger('change');
            $("#containDrdApi").show();
          
        } else {
            $("#containDrdApi").hide();
            $("#drdApi").val(null).trigger('change');
        }
    });
    selectedApi();
    function selectedApi() {
        app.component.Loading.Show();
        _AjaxPost("/SyncSetting/GetByWarehouseId", { id: $('#Id').val() }, function (rs) {
            app.component.Loading.Hide();
            if (rs.success) {
                console.log(rs.datas);
                var itemselect = $.map(rs.datas, function (item) {
                    return item.Id
                });
                $('#drdApi').val(itemselect).trigger('change');
            }
        });
    }
    $("#inputImage").on("change", function () {
        var fileName =this.files[0].name;
        var oFReader = new FileReader();
        oFReader.readAsDataURL(this.files[0]);
        oFReader.onload = function (oFREvent) {
            $("#ImgAvatar").attr('src', this.result);
            app.component.Loading.Show();
            console.log(this);
            _AjaxPost("/Warehouses/UpdateLogoWarehouse", { base64Image: this.result, WarehouseId: $("#Id").val(), fileName}, function (rs) {
                app.component.Loading.Hide();
                if (rs.success) {
                    notifySuccess("Cập nhật ảnh thành công!");
                }
                else { notifyError("Cập nhật ảnh thất bại!"); window.location.reload(); }
            })
        }
    });

    $("#frmUpdateWarehouses").on("submit", function () {
        app.component.Loading.Show();
        var data = $(this).serializeObject();
        data.IsSync = $("#IsSync").is(":checked");
        data.QuotaPromotion = parseInt(data.QuotaPromotion.split('.').join(''));
        _AjaxPost("/Warehouses/UpdateWarehouse", { data: data, ApisId: $('#drdApi').val()}, function (rs) {
            app.component.Loading.Hide();
            if (rs.success) {
                notifySuccess("Cập nhật thành công!")
                setTimeout(function () {
                    window.location.href = "/Warehouses/Index";
                }, 1000);
            }
            else {
                notifyWarning("Cập nhật thất bại!")
            }
        });
        return false;
    });
});