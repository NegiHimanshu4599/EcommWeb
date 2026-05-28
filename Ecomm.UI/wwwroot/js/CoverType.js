var coverTypeTable;
$(document).ready(function () {
    loadCoverTypeTable();
})
function loadCoverTypeTable() {
    if ($.fn.DataTable.isDataTable('#tbCData')) {
        $('#tbCData').DataTable().destroy();
    }
    coverTypeTable = $('#tbCData').DataTable({
        "ajax":
        {
            "url": "/Admin/CoverType/GetAllCoverType"
        },
        "columns": [
            { "data": "name", "width": "70%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
<div class="text-center">
    <a href="/Admin/CoverType/UpsertCoverType/${data}"
     class="btn btn-success me-2">
       <i class="fas fa-edit"></i>
    </a>
    <a class="btn btn-danger"
       onclick=DeleteCoverType('/Admin/CoverType/DeleteCoverType/${data}')>
       <i class="fas fa-trash-alt"></i>
    </a>
</div>
`;
                }
            }
        ]
    })
}
function DeleteCoverType(url) {
    swal({
        title: "wanna delete this data",
        icon: "warning",
        text: "Delete Information!!",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        coverTypeTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}