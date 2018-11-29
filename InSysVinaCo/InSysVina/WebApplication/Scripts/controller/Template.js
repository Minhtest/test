$(document).ready(function () {
    $('#tblTemplate').bootstrapTable({
        url: "/Template/GetData",
        method: "POST",
        ajax: function (config) {
            app.component.Loading.Show();
            _AjaxPost(config.url, null, function (rs) {
                app.component.Loading.Hide();
                if (rs.success) {
                    config.success({
                        total: rs.total,
                        rows: rs.data
                    });
                }
                else {
                    notifyError("Lỗi lấy dữ liệu!");
                    console.log(rs.ex);
                }
            });
        },
        striped: true,
        sidePagination: 'server',
        pagination: true,
        paginationVAlign: 'none',
        limit: 10,
        formatLoadingMessage: function () {
            return "Đang Tải";
        },
        pageSize: 10,
        pageList: [10, 25, 50, 100, 200],
        search: true,
        showColumns: true,
        showRefresh: true,
        minimumCountColumns: 2,
        columns: [
            {
                field: 'Name',
                title: 'Tên Template',
                align: 'left',
                valign: 'middle'
            },
            {
                field: 'DownLoad',
                title: 'Link DownLoad',
                align: 'center',
                valign: 'middle',
                formatter: function (value, row, index) {
                    if (row['Url'] == null) {
                        return '<i style="color:red">Chưa Upload</i>';
                    }
                    else
                        return '<a style="color:green;text-decoration: underline" href="' + row['Url'] + '">Download here</a>';
                },
            }
        ]
    })
});