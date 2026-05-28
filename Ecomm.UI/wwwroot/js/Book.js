var bookTable;
$(document).ready(function () {
    loadBookTable();
})
function loadBookTable() {

    if ($.fn.DataTable.isDataTable('#tblData')) {
        $('#tblData').DataTable().destroy();
    }
    bookTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Book/GetAllBooks"
        },
        "columns": [
            { "data": "title", "width": "25%" },
            { "data": "price", "width": "25%" },
            { "data": "stockQuantity", "width": "25%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
<div class="text-center">

    <a href="/Admin/Book/UpsertBook/${data}"
       class="btn btn-success me-2">

       <i class="fas fa-edit"></i>

    </a>

    <a class="btn btn-danger"
       onclick=DeleteBook('/Admin/Book/DeleteBook/${data}')>

       <i class="fas fa-trash-alt"></i>

    </a>

</div>
`;
                }
            }
        ]
    })
}

function DeleteBook(url) {

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
                        bookTable.ajax.reload();

                    } else {

                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}