$(document).ready(function () {

    $("#frmCreateEditApiConnect").on("submit", function () {
        app.component.Loading.Show();
        var data = $(this).serializeObject();
        data.Disabled = $("#Disabled:checked").val();
        _AjaxPost("/SyncSetting/UpdateApiConnect", data, function (rs) {
            app.component.Loading.Hide();
            if (rs.success) {
                notifySuccess(rs.message +" thành công!");
                setTimeout(function () {
                    window.location.href = "/SyncSetting/Index";
                }, 1000);
            }
            else {
                notifyWarning(rs.message +" thất bại!")
            }
        });
        return false;
    });
});