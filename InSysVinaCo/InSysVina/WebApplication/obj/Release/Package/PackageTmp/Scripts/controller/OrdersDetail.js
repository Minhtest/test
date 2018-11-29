$(document).ready(function () {
    _AjaxPost("/Orders/PrintBillOrder", { Id: $("#OrderId").val() }, function (rs) {
        $("#BillContent").html(rs);
        app.component.FormattxtMore();
        $("#barcodecontainer #barcode").html(DrawHTMLBarcode_Code128A($("#barcodecontainer #barcode").text(), "yes", "in", 0, 2.5, 1, "bottom", "center", "", "black", "white"));
    });
    $("#btnPrint").click(function () {
        app.component.Loading.Show();
        var newWin = window.open('', 'In Hóa Đơn');
        newWin.document.open();
        newWin.document.write('<html><head><title>In Hóa Đơn</title></head><body onload="window.print()">' + $("#BillContent").html() + '</body></html>');
        newWin.document.close();
        setTimeout(function () { newWin.close(); app.component.Loading.Hide(); }, 10);
    });
});